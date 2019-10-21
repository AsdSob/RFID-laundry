using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.LinenModel;
using PALMS.Data.Objects.NoteModel;
using PALMS.Settings.ViewModel.NoteLinenReplacement.EntityModels;
using PALMS.Settings.ViewModel.NoteSearchLinen.Windows;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Settings.ViewModel.NoteSearchLinen
{
    public class NoteSearchViewModel : ViewModelBase, ISettingsContent, IInitializationAsync
    {
        public string Name => "Notes";

        private readonly IDispatcher _dispatcher;
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private readonly IResolver _resolverService;

        private ObservableCollection<NoteHeaderViewModel> _noteHeaders;
        private ObservableCollection<NoteRowViewModel> _noteRows;
        private ObservableCollection<Client> _clients;


        private NoteHeaderViewModel _selectedNoteHeader;
        private NoteRowViewModel _selectedNoteRow;

        private List<UnitViewModel> _noteStatus;
        private List<UnitViewModel> _deliveryType;
        private List<UnitViewModel> _serviceType;
        private List<LinenListViewModel> _linenList;
        private List<Department> _departments;

        public List<Department> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public List<LinenListViewModel> LinenList
        {
            get => _linenList;
            set => Set(ref _linenList, value);
        }
        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }
        public List<UnitViewModel> NoteStatus
        {
            get => _noteStatus;
            set => Set(ref _noteStatus, value);
        }

        public List<UnitViewModel> ServiceType
        {
            get => _serviceType;
            set => Set(ref _serviceType, value);
        }

        public List<UnitViewModel> DeliveryType
        {
            get => _deliveryType;
            set => Set(ref _deliveryType, value);
        }
        public NoteRowViewModel SelectedNoteRow
        {
            get => _selectedNoteRow;
            set => Set(ref _selectedNoteRow, value);
        }
        public NoteHeaderViewModel SelectedNoteHeader
        {
            get => _selectedNoteHeader;
            set => Set(ref _selectedNoteHeader, value);
        }
        public ObservableCollection<NoteRowViewModel> NoteRows
        {
            get => _noteRows;
            set => Set(ref _noteRows, value);
        }
        public ObservableCollection<NoteHeaderViewModel> NoteHeaders
        {
            get => _noteHeaders;
            set => Set(ref _noteHeaders, value);
        }

        public ObservableCollection<NoteHeaderViewModel> SortedNoteHeaders { get; set; }
        public ObservableCollection<NoteRowViewModel> SortedNoteRows { get; set; }

        public RelayCommand DeleteNoteCommand { get; }
        public RelayCommand DeleteRowCommand { get; }
        public RelayCommand GetNotesCommand { get; }
        public RelayCommand RunCodeCommand { get; }
        public RelayCommand<object> ShowNoteCommand { get; }

        public NoteSearchViewModel(IDispatcher dispatcher, IDataService dataService, IDialogService dialogService, IResolver resolver)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));

            DeleteNoteCommand = new RelayCommand(DeleteNoteHeader, () => SelectedNoteHeader != null);
            DeleteRowCommand = new RelayCommand(DeleteNoteRow, () => SelectedNoteRow != null);
            GetNotesCommand = new RelayCommand(GetNotes);
            RunCodeCommand = new RelayCommand(RunCode);
            ShowNoteCommand = new RelayCommand<object>(GetNote);

            PropertyChanged += OnPropertyChanged;
        }

        public async Task InitializeAsync()
        {

            NoteStatus = EnumExtentions.GetValues<NoteStatusEnum>().ToList();
            DeliveryType = EnumExtentions.GetValues<DeliveryTypeEnum>().ToList();
            ServiceType = EnumExtentions.GetValues<ServiceTypeEnum>().ToList();
        }

        public async void GetNotes()
        {
            _dialogService.ShowBusy();

            try
            {
                var departments = await _dataService.GetAsync<Department>();
                _dispatcher.RunInMainThread(() => Departments = departments.ToList());

                var noteHeader = await _dataService.GetAsync<NoteHeader>(x => x.Client);
                var noteHeaders = noteHeader.Select(x => new NoteHeaderViewModel(x));
                _dispatcher.RunInMainThread(() => NoteHeaders = noteHeaders.ToObservableCollection());

                var noteRow = await _dataService.GetAsync<NoteRow>(x => x.LinenList.MasterLinen);
                var noteRows = noteRow.Select(x => new NoteRowViewModel(x));
                _dispatcher.RunInMainThread(() => NoteRows = noteRows.ToObservableCollection());

                var linen = await _dataService.GetAsync<LinenList>();
                var linens = linen.Select(x => new LinenListViewModel(x));
                _dispatcher.RunInMainThread(() => LinenList = linens.ToList());

                NoteRows.ForEach(x => x.SetName());
                NoteRows.ForEach(x => x.SetHasMasterLinen());
            }

            catch (Exception ex)
            {
                _dialogService.HideBusy();
                Helper.RunInMainThread(() => _dialogService.ShowErrorDialog($"Initialization error: {ex.Message}"));
            }

            finally
            {
                _dialogService.HideBusy();
            }

            RaisePropertyChanged(() => SortedNoteHeaders);
            RaisePropertyChanged(() => SortedNoteRows);
        }


        public bool HasChanges()
        {
            return false;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedNoteHeader))
            {
                DeleteNoteCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedNoteRow))
            {
                DeleteRowCommand.RaiseCanExecuteChanged();
            }
        }

        public void GetNote(object isNoteHeader)
        {
            string isNote = (string)isNoteHeader;

            if (isNote == "True" && SelectedNoteHeader != null)
            {
                ShowNote(SelectedNoteHeader);
                return;
            }
            if (isNote != "True" && SelectedNoteRow != null)
            {
                var note = NoteHeaders.FirstOrDefault(x => x.Id == SelectedNoteRow.NoteHeaderId);
                if (note != null)
                {
                    ShowNote(note);
                }
            }
        }

        public async void RunCode()
        {
            if(Departments == null || Departments.Count ==0) return;

            if(!_dialogService.ShowQuestionDialog("Run Update?")) return;

            var noteRows =new List<NoteRowViewModel>();

            //LinenList.ForEach(x=> x.AcceptChanges());
            //_dataService.AddOrUpdateAsync(LinenList.Select(x=> x.OriginalObject));

            foreach (var noteRow in NoteRows)
            {
                if(noteRow.Price > 0) continue;

                var linen = LinenList?.FirstOrDefault(x => x.Id == noteRow.LinenListId);
                if (linen != null && linen.Laundry > 0)
                {
                    noteRow.Weight = linen.Weight;

                    switch (noteRow.ServiceTypeId)
                    {
                        case (int) ServiceTypeEnum.Laundry:
                            noteRow.Price = linen.Laundry;
                            break;

                        case (int) ServiceTypeEnum.DryCleaning:
                            noteRow.Price = linen.DryCleaning;
                            break;

                        case (int) ServiceTypeEnum.Pressing:
                            noteRow.Price = linen.Pressing;
                            break;
                    }

                    if (noteRow.HasChanges())
                    {
                        noteRows.Add(noteRow);
                        noteRow.AcceptChanges();
                        //await _dataService.AddOrUpdateAsync(noteRow.OriginalObject);
                    }
                }
            }

            foreach (var row in noteRows)
            {
                //row.AcceptChanges();
                //_dataService.AddOrUpdateAsync(row.OriginalObject);
            }


            _dialogService.ShowInfoDialog("Update Complete");
        }

        public async void ShowNote(NoteHeaderViewModel note)
        {
            var noteH = note;

            if (note == null) return;

            var noteEditViewModel = _resolverService.Resolve<NoteEditViewModel>();
            var noteHeader = new NoteHeaderViewModel(noteH.OriginalObject);

            await noteEditViewModel.InitializeAsync(noteHeader);

            var showDialog = _dialogService.ShowDialog(noteEditViewModel);

            if (showDialog)
            {
            }
        }

        public async void DeleteNoteRow()
        {
            if (!_dialogService.ShowQuestionDialog($" Do you want to Delete Linen ?"))
                return;

            await _dataService.DeleteAsync(SelectedNoteRow.OriginalObject);

            NoteRows.Remove(SelectedNoteRow);
        }

        public async void DeleteNoteHeader()
        {
            if (!_dialogService.ShowQuestionDialog($" Do you want to Delete {SelectedNoteHeader.Name} ?"))
                return;

            await _dataService.DeleteAsync(SelectedNoteHeader.OriginalObject);
            var rows = NoteRows.Where(x => x.NoteHeaderId == SelectedNoteHeader.Id).ToList();

            foreach (var row in rows)
            {
                NoteRows.Remove(row);
                await _dataService.DeleteAsync(row.OriginalObject);
            }
            NoteHeaders.Remove(SelectedNoteHeader);
        }
    }
}
