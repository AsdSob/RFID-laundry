using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.LinenModel;
using PALMS.Data.Objects.NoteModel;
using PALMS.Notes.ViewModel.EntityViewModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;

namespace PALMS.Notes.ViewModel.Window
{
    public class NoteLinenReplacementViewModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IDispatcher _dispatcher;
        private readonly IDataService _dataService;

        private ObservableCollection<LinenListViewModel> _linenList;
        private ObservableCollection<NoteRowViewModel> _noteRows;
        private ObservableCollection<NoteHeaderViewModel> _noteHeaders;
        private LinenListViewModel _selectedLinenList;
        private NoteRowViewModel _selectedNoteRow;
        private NoteHeaderViewModel _selectedNoteHeader;
        private bool _showNullDepartment;
        private ObservableCollection<UnitViewModel> _noteStatuses;


        public ObservableCollection<UnitViewModel> NoteStatuses
        {
            get => _noteStatuses;
            set => Set(ref _noteStatuses, value);
        }
        public bool ShowNullDepartment
        {
            get => _showNullDepartment;
            set => Set(ref _showNullDepartment, value);
        }
        public NoteHeaderViewModel SelectedNoteHeader
        {
            get => _selectedNoteHeader;
            set => Set(ref _selectedNoteHeader, value);
        }
        public NoteRowViewModel SelectedNoteRow
        {
            get => _selectedNoteRow;
            set => Set(ref _selectedNoteRow, value);
        }
        public LinenListViewModel SelectedLinenList
        {
            get => _selectedLinenList;
            set => Set(ref _selectedLinenList, value);
        }
        public ObservableCollection<NoteHeaderViewModel> NoteHeaders
        {
            get => _noteHeaders;
            set => Set(ref _noteHeaders, value);
        }
        public ObservableCollection<NoteRowViewModel> NoteRows
        {
            get => _noteRows;
            set => Set(ref _noteRows, value);
        }
        public ObservableCollection<LinenListViewModel> LinenList
        {
            get => _linenList;
            set => Set(ref _linenList, value);
        }

        public ObservableCollection<NoteHeaderViewModel> SortedNoteHeaders => SortNoteHeaders();
        public ObservableCollection<NoteRowViewModel> SortedNoteRows => SortNoteRows();
        public ObservableCollection<LinenListViewModel> SortedLinenList => SortLinenList();
        public ObservableCollection<LinenListViewModel> SortedDepartmentLinenList => SortLinenList();

        public Action<bool> CloseAction { get; set; }

        public RelayCommand DeleteRowCommand { get; }
        public RelayCommand DeleteHeaderCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }

        public async Task InitializeAsync()
        {
            var linenList = await _dataService.GetAsync<LinenList>(x => x.MasterLinen);
            var linenLists = linenList.Select(x=> new LinenListViewModel(x));
            _dispatcher.RunInMainThread(() => LinenList = linenLists.ToObservableCollection());

            var noteHeader = await _dataService.GetAsync<NoteHeader>(x => x.Department.Client);
            var noteHeaders = noteHeader.Select(x => new NoteHeaderViewModel(x)).Where(x => x.InvoiceId == null);
            _dispatcher.RunInMainThread(() => NoteHeaders = noteHeaders.ToObservableCollection());
            NoteHeaders.ForEach(x => x.SetName());

            var noteRow = await _dataService.GetAsync<NoteRow>(x=> x.LinenList);
            var noteRows = noteRow.Select(x=> new NoteRowViewModel(x));
            _dispatcher.RunInMainThread(()=> NoteRows = noteRows.ToObservableCollection());

            NoteStatuses = EnumExtentions.GetValues<NoteStatusEnum>().ToObservableCollection();
        }

        public NoteLinenReplacementViewModel(IDialogService dialogService, IDataService dataService, IDispatcher dispatcher)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            CloseCommand = new RelayCommand(Close);
            SaveCommand = new RelayCommand(Save);
            DeleteRowCommand = new RelayCommand(DeleteRow, ()=> SelectedNoteRow != null);
            DeleteHeaderCommand = new RelayCommand(DeleteHeader, ()=> SelectedNoteHeader != null);

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedNoteRow))
            {
                RaisePropertyChanged(() => SortedDepartmentLinenList);
                DeleteRowCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedNoteHeader))
            {
                DeleteHeaderCommand.RaiseCanExecuteChanged();
            }
        }

        public async void Save()
        {
            if (!_dialogService.ShowQuestionDialog($" Do you want to Save changed Note linens?"))
                return;

            var noteRows = SortedNoteRows.Where(x => x.HasChanges());

            if(noteRows == null || noteRows.Count() <= 0) return;

            noteRows.ForEach(x=> x.AcceptChanges());
            await _dataService.AddOrUpdateAsync(noteRows.Select(x => x.OriginalObject));
        }

        public async void DeleteRow()
        {
            if (!_dialogService.ShowQuestionDialog($" Do you want to Delete Selected Note Linen ?"))
                return;

            await _dataService.DeleteAsync(SelectedNoteRow?.OriginalObject);

            NoteRows.Remove(SelectedNoteRow);
            RaisePropertyChanged(()=> SortedNoteRows);
        }

        public async void DeleteHeader()
        {
            if (!_dialogService.ShowQuestionDialog($" Do you want to Delete {SelectedNoteHeader.Name} ?"))
                return;

            await _dataService.DeleteAsync(SelectedNoteHeader?.OriginalObject);

            NoteHeaders.Remove(SelectedNoteHeader);
            RaisePropertyChanged(() => SortedNoteHeaders);
        }

        public ObservableCollection<NoteHeaderViewModel> SortNoteHeaders()
        {
            var noteHeaders = new ObservableCollection<NoteHeaderViewModel>();

            if (ShowNullDepartment)
            {
                noteHeaders = NoteHeaders.Where(x => x.OriginalObject.Department == null).ToObservableCollection();
            }
            else
            {
                foreach (var noteHeader in NoteHeaders)
                {
                    var noteRows = NoteRows.Where(x => x.NoteHeaderId == noteHeader.Id);

                    if (noteRows == null || !noteRows.Any())
                    {
                        noteHeaders.Add(noteHeader);
                    }
                }
            }

            return noteHeaders;
        }

        public ObservableCollection<NoteRowViewModel> SortNoteRows()
        {
            var noteRows = new ObservableCollection<NoteRowViewModel>();

            noteRows = NoteRows.Where(x => x.OriginalObject.LinenList == null).ToObservableCollection();

            return noteRows;
        }

        public ObservableCollection<LinenListViewModel> SortLinenList()
        {
            var linenList = new ObservableCollection<LinenListViewModel>();

            linenList = LinenList.Where(x => x.OriginalObject.MasterLinen == null).ToObservableCollection();

            return linenList;
        }

        public ObservableCollection<LinenListViewModel> SortDepartmentLinenList()
        {
            var linenList = new ObservableCollection<LinenListViewModel>();

            linenList = LinenList.Where(x => x.OriginalObject.MasterLinen == null).ToObservableCollection();

            return linenList;
        }

        public void Close()
        {
            if (!_dialogService.ShowQuestionDialog("Do you want to close window ?"))
                return;

            CloseAction?.Invoke(false);
        }

    }
}
