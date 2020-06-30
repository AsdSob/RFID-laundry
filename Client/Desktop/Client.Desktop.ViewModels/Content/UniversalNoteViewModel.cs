using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Documents;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;

namespace Client.Desktop.ViewModels.Content
{
    public class UniversalNoteViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly ILaundryService _laundryService;
        private readonly IResolver _resolverService;
        private readonly IMainDispatcher _dispatcher;

        private ObservableCollection<ClientEntityViewModel> _clients;
        private ClientEntityViewModel _selectedClient;
        private ObservableCollection<DepartmentEntityViewModel> _departments;
        private DepartmentEntityViewModel _selectedDepartment;
        private ObservableCollection<MasterLinenEntityViewModel> _masterLinens;
        private ObservableCollection<ClientLinenEntityViewModel> _clientLinens;
        private bool _showAllLinen;
        private ObservableCollection<RfidReaderEntityViewModel> _readers;
        private ObservableCollection<RfidAntennaEntityViewModel> _antennas;

        public ObservableCollection<RfidAntennaEntityViewModel> Antennas
        {
            get => _antennas;
            set => Set(ref _antennas, value);
        }
        public ObservableCollection<RfidReaderEntityViewModel> Readers
        {
            get => _readers;
            set => Set(ref _readers, value);
        }
        public bool ShowAllLinen
        {
            get => _showAllLinen;
            set => Set(ref _showAllLinen, value);
        }
        public ObservableCollection<ClientLinenEntityViewModel> ClientLinens
        {
            get => _clientLinens;
            set => Set(ref _clientLinens, value);
        }
        public ObservableCollection<MasterLinenEntityViewModel> MasterLinens
        {
            get => _masterLinens;
            set => Set(ref _masterLinens, value);
        }
        public DepartmentEntityViewModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }
        public ObservableCollection<DepartmentEntityViewModel> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public ClientEntityViewModel SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public ObservableCollection<ClientEntityViewModel> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }

        public ObservableCollection<DepartmentEntityViewModel> SortedStaffs =>
            Departments?.Where(x => x.ParentId != null && x.ClientId == SelectedClient?.Id).ToObservableCollection();

        public ObservableCollection<DepartmentEntityViewModel> SortedDepartments => Departments
            ?.Where(x => x.ClientId == SelectedClient?.Id && x.ParentId == null).ToObservableCollection();
        public ObservableCollection<ClientLinenEntityViewModel> SortedClientLinens => SortClientLinens();


        public RelayCommand InitializeCommand { get; }

        public UniversalNoteViewModel(ILaundryService dataService, IDialogService dialogService, IResolver resolver, IMainDispatcher dispatcher)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            InitializeCommand = new RelayCommand(Initialize);

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(()=> SortedDepartments);
            }else            
            
            if (e.PropertyName == nameof(SelectedDepartment))
            {
                RaisePropertyChanged(() => SortedClientLinens);
            }
            else

            if (e.PropertyName == nameof(ShowAllLinen))
            {
                RaisePropertyChanged(() => SortedClientLinens);
            }

        }

        private async void Initialize()
        {
            Clients = await _laundryService.Clients();
            Departments = await _laundryService.Departments();
            MasterLinens = await _laundryService.MasterLinens();

            GetClientLinens();
            GetRfidReaders();
        }

        private async void GetClientLinens()
        {
            ClientLinens = await _laundryService.ClientLinens();
        }
        private async void GetRfidReaders()
        {
            Readers = await _laundryService.RfidReaders();
            Antennas = await _laundryService.RfidAntennas();
        }

        private ObservableCollection<ClientLinenEntityViewModel> SortClientLinens()
        {
            var linens = new ObservableCollection<ClientLinenEntityViewModel>();

            if (SortedStaffs == null || SelectedDepartment == null) return linens;



            var staffs = ShowAllLinen
                ? SortedStaffs
                : SortedStaffs.Where(x => x.ParentId == SelectedDepartment?.Id).ToObservableCollection();

            foreach (var staff in staffs)
            {
                linens.AddRange(ClientLinens?.Where(x=> x.DepartmentId == staff.Id));
            }

            return linens;
        }


    }
}
