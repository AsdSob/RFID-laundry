using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.NoteModel;
using PALMS.LinenList.ViewModel.EntityViewModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;
using UnitViewModel = PALMS.ViewModels.Common.Enumerations.UnitViewModel;

namespace PALMS.LinenList.ViewModel.Windows
{
    public class UnUsedLinenViewModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly IDataService _dataService;
        private readonly IDispatcher _dispatcher;
        private readonly IDialogService _dialogService;

        private List<Client> _clients;
        private Client _selectedClient;
        private List<Department> _departments;
        private Department _selectedDepartment;

        private List<UsedLinenViewModel> _linenLists;
        private UsedLinenViewModel _selectedLinenList;
        private List<UnitViewModel> _serviceTypes;
        private bool _invoicedNotes;
        private List<NoteHeader> _noteHeaders;
        private List<NoteRow> _noteRows;


        public List<NoteRow> NoteRows
        {
            get => _noteRows;
            set => Set(ref _noteRows, value);
        }
        public List<NoteHeader> NoteHeaders
        {
            get => _noteHeaders;
            set => Set(ref _noteHeaders, value);
        }
        public bool InvoicedNotes
        {
            get => _invoicedNotes;
            set => Set(ref _invoicedNotes, value);
        }
        public List<UnitViewModel> ServiceTypes
        {
            get => _serviceTypes;
            set => Set(ref _serviceTypes, value);
        }
        public List<UsedLinenViewModel> LinenLists
        {
            get => _linenLists;
            set => Set(ref _linenLists, value);
        }
        public UsedLinenViewModel SelectedLinenList
        {
            get => _selectedLinenList;
            set => Set(ref _selectedLinenList, value);
        }
        public Department SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }
        public List<Department> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public Client SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public List<Client> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }

        public List<Department> SortedDepartments => SortDepartments();
        public List<UsedLinenViewModel> SortedLinenList => SortLinenList();

        public Action<bool> CloseAction { get; set; }

        public RelayCommand GetAllDataCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand DeactivateCommand { get; }
        public RelayCommand DeactivateAllUnUsedCommand { get; }
        public RelayCommand CloseCommand { get; }

        public UnUsedLinenViewModel(IDataService dataService, IDispatcher dispatcher, IDialogService dialogService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            GetAllDataCommand = new RelayCommand(GetNoteLinens, () => SelectedClient != null);
            DeleteCommand = new RelayCommand(DeleteLinenList, () => SelectedLinenList != null);
            DeactivateCommand = new RelayCommand(DeactivateLinen, () => SelectedLinenList != null);
            DeactivateAllUnUsedCommand = new RelayCommand(DeactivateUnusedLinen);
            CloseCommand = new RelayCommand(Close);

            PropertyChanged += OnPropertyChanged;
        }

        public async Task InitializeAsync(Client selectedClient)
        {

            var clients = await _dataService.GetAsync<Client>();
            _dispatcher.RunInMainThread(() => Clients = clients.OrderBy(x => x.ShortName).ToList());

            var departments = await _dataService.GetAsync<Department>();
            _dispatcher.RunInMainThread(() => Departments = departments.ToList());

            var linenList = await _dataService.GetAsync<Data.Objects.LinenModel.LinenList>(x => x.MasterLinen);
            var linenLists = linenList.Select(x => new UsedLinenViewModel(x));
            _dispatcher.RunInMainThread(() => LinenLists = linenLists.ToList());

            ServiceTypes = EnumExtentions.GetValues<ServiceTypeEnum>();
            SelectedClient = selectedClient;
            SelectedDepartment = null;
        }

        public void Close()
        {
            if (_dialogService.ShowQuestionDialog($"Do you want to close window ? "))
                CloseAction?.Invoke(true);
        }

        private async void GetNoteLinens()
        {
            if (SelectedClient == null) return;

            _dialogService.ShowBusy();

            try
            {
                var notes = await _dataService.GetAsync<NoteHeader>(x => x.ClientId == SelectedClient.Id);
                _dispatcher.RunInMainThread(() => NoteHeaders = notes.ToList());

                var noteRows = new List<NoteRow>();

                foreach (var note in NoteHeaders)
                {
                    var noteRow = await _dataService.GetAsync<NoteRow>(x => x.NoteHeaderId == note.Id);
                    noteRows.AddRange(noteRow);
                }
                _dispatcher.RunInMainThread(() => NoteRows = noteRows.ToList());
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

            CalcLinenUsage();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                GetNoteLinens();
                RaisePropertyChanged(() => SortedDepartments);
            }

            if (e.PropertyName == nameof(SelectedDepartment))
            {
                RaisePropertyChanged(() => SortedLinenList);
            }

            if (e.PropertyName == nameof(SelectedLinenList))
            {
                DeleteCommand.RaiseCanExecuteChanged();
                DeactivateCommand.RaiseCanExecuteChanged();
            }
        }

        private List<Department> SortDepartments()
        {
            if (SelectedClient == null)
                return null;

            var departments = Departments.Where(x => x.ClientId == SelectedClient.Id).OrderBy(x => x.Name);
            return departments.ToList();
        }

        private List<UsedLinenViewModel> SortLinenList()
        {
            if (SelectedDepartment == null)
                return null;

            var linenList = LinenLists.Where(x => x.DepartmentId == SelectedDepartment.Id);
            return linenList.ToList();
        }

        private void CalcLinenUsage()
        {
            if (!NoteRows.Any() || NoteRows == null) return;

            foreach (var noteRow in NoteRows)
            {
                var linenList = LinenLists.FirstOrDefault(x => x.Id == noteRow.LinenListId);

                if (linenList != null) linenList.NumberOfUsage++;
            }
        }

        private async void DeleteLinenList()
        {
            if (SelectedLinenList == null || SelectedLinenList.OriginalObject == null) return;

            var noteRows = NoteRows.Where(x => x.LinenListId == SelectedLinenList.Id);
            if (noteRows.Any())
            {
                if (_dialogService.ShowWarnigDialog($" There is -- {noteRows.Count()} -- Linens in Notes will be Deleted \n Do you want to Delete it ?"))
                    return;
            }

            if (!_dialogService.ShowQuestionDialog($" Do you want to DELETE {SelectedLinenList.Name} quantity?"))
                return;
            await _dataService.AddOrUpdateAsync(SelectedLinenList.OriginalObject);
            await _dataService.DeleteAsync(SelectedLinenList.OriginalObject);

            _dialogService.ShowInfoDialog($" \"{SelectedLinenList.Name} \" was Deleted ");

            LinenLists.Remove(SelectedLinenList);
            RaisePropertyChanged(() => SortedLinenList);
        }

        private async void DeactivateLinen()
        {
            if (SelectedLinenList == null || SelectedLinenList.OriginalObject == null) return;

            if (!_dialogService.ShowQuestionDialog($" Do you want to DEACTIVATE {SelectedLinenList.Name} ?"))
                return;

            SelectedLinenList.OriginalObject.Active = false;

            await _dataService.AddOrUpdateAsync(SelectedLinenList.OriginalObject);

            _dialogService.ShowInfoDialog($" \"{SelectedLinenList.Name} \" was Deleted ");

            RaisePropertyChanged(() => SortedLinenList);
        }

        private async void DeactivateUnusedLinen()
        {
            var linens = SortedLinenList.Where(x => x.NumberOfUsage == 0);
            if (!linens.Any() ||
                !_dialogService.ShowQuestionDialog($" Do you want to DEACTIVATE All linen with \"0\" usage ?")) return;

            foreach (var linen in linens)
            {
                linen.OriginalObject.Active = false;
                await _dataService.AddOrUpdateAsync(linen.OriginalObject);
            }
            RaisePropertyChanged(() => SortedLinenList);
        }
    }
}
