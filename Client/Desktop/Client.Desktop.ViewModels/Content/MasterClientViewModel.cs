using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Windows;

namespace Client.Desktop.ViewModels.Content
{
    public class MasterClientViewModel : ViewModelBase
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private readonly IResolver _resolverService;

        #region Parameters

        private ObservableCollection<ClientEntityViewModel> _clients;
        private ClientEntityViewModel _selectedClient;
        private ObservableCollection<DepartmentEntityViewModel> _departments;
        private List<UnitViewModel> _cities;
        private DepartmentEntityViewModel _selectedDepartment;
        private List<UnitViewModel> _departmentTypes;
        private DepartmentEntityViewModel _selectedStaff;
        private bool _showAllStaff;

        public bool ShowAllStaff
        {
            get => _showAllStaff;
            set => Set(ref _showAllStaff, value);
        }
        public DepartmentEntityViewModel SelectedStaff
        {
            get => _selectedStaff;
            set => Set(ref _selectedStaff, value);
        }
        public List<UnitViewModel> DepartmentTypes
        {
            get => _departmentTypes;
            set => Set(() => DepartmentTypes, ref _departmentTypes, value);
        }
        public DepartmentEntityViewModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(() => SelectedDepartment, ref _selectedDepartment, value);
        }
        public List<UnitViewModel> Cities
        {
            get => _cities;
            set => Set(() => Cities, ref _cities, value);
        }
        public ObservableCollection<DepartmentEntityViewModel> Departments
        {
            get => _departments;
            set => Set(() => Departments, ref _departments, value);
        }
        public ClientEntityViewModel SelectedClient
        {
            get => _selectedClient;
            set => Set(() => SelectedClient, ref _selectedClient, value);
        }
        public ObservableCollection<ClientEntityViewModel> Clients
        {
            get => _clients;
            set => Set(() => Clients, ref _clients, value);
        }

        public ObservableCollection<DepartmentEntityViewModel> SortedDepartments => SortDepartments();
        public ObservableCollection<DepartmentEntityViewModel> SortedStaffs => SortStaffs();

        #endregion

        public RelayCommand AddClientCommand { get; }
        public RelayCommand EditClientCommand { get; }

        public RelayCommand AddDepartmentCommand { get; }
        public RelayCommand EditDepartmentCommand { get; }

        public RelayCommand AddStaffCommand { get; }
        public RelayCommand EditStaffCommand { get; }

        public RelayCommand InitializeCommand { get; }



        public MasterClientViewModel(ILaundryService dataService, IDialogService dialogService, IResolver resolver)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));

            AddClientCommand = new RelayCommand(AddNewClient);
            EditClientCommand = new RelayCommand(EditClient, (() => SelectedClient != null));

            AddDepartmentCommand = new RelayCommand(AddNewDepartment, () => SelectedClient != null);
            EditDepartmentCommand = new RelayCommand(EditDepartment, () => SelectedDepartment != null);

            AddStaffCommand = new RelayCommand(AddNewStaff, () => SelectedDepartment != null);
            EditStaffCommand = new RelayCommand(EditStaff, () => SelectedStaff != null);
            InitializeCommand = new RelayCommand(Initialize);

            Cities = EnumExtensions.GetValues<CitiesEnum>();
            DepartmentTypes = EnumExtensions.GetValues<DepartmentTypeEnum>();

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                AddDepartmentCommand.RaiseCanExecuteChanged();
                EditClientCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged((() => SortedDepartments));
                SelectedDepartment = SortedDepartments.FirstOrDefault();
            }else

            if (e.PropertyName == nameof(SelectedDepartment))
            {
                EditDepartmentCommand.RaiseCanExecuteChanged();
                AddStaffCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged((() => SortedStaffs));
            }else

            if (e.PropertyName == nameof(SelectedStaff))
            {
                EditStaffCommand.RaiseCanExecuteChanged();
            }else 
            
            if (e.PropertyName == nameof(ShowAllStaff))
            {
                RaisePropertyChanged(()=> SortedStaffs);
            }
        }

        private async void Initialize()
        {
            await GetClients();
            await GetDepartments();
        }

        private async Task GetClients()
        {
            Clients = await _laundryService.Clients();
        }

        private async Task GetDepartments()
        {
            Departments = await _laundryService.Departments();
        }

        private ObservableCollection<DepartmentEntityViewModel> SortDepartments()
        {
            return Departments?.Where(x => x.ClientId == SelectedClient?.Id && x.ParentId == null).ToObservableCollection();
        }

        private ObservableCollection<DepartmentEntityViewModel> SortStaffs()
        {
            var staffs = new ObservableCollection<DepartmentEntityViewModel>();

            if (SelectedDepartment == null || ShowAllStaff)
            {
                if (SortedDepartments == null)
                    return staffs;

                foreach (var department in SortedDepartments)
                {
                    staffs.AddRange(Departments?.Where(x => x.ParentId == department?.Id));
                }
            }
            else
            {
                staffs.AddRange(Departments?.Where(x => x.ParentId == SelectedDepartment?.Id).ToObservableCollection());
            }

            return staffs;
        }

        private void EditClient()
        {
            ClientWindow(SelectedClient);
        }

        private void AddNewClient()
        {
           ClientWindow(null);
        }

        private async void ClientWindow(ClientEntityViewModel client)
        {
            var clientWindow = _resolverService.Resolve<MasterClientWindowModel>();
            clientWindow.Clients = Clients;

            clientWindow.SetSelectedClient(client);

            if (_dialogService.ShowDialog(clientWindow))
            {
                Clients.Clear();
                await GetClients();
            }
        }

        private void EditDepartment()
        {
            DepartmentWindow(SelectedDepartment);
        }

        private void AddNewDepartment()
        {
            DepartmentWindow(null);
        }

        private async void DepartmentWindow(DepartmentEntityViewModel department)
        {
            var departmentWindow = _resolverService.Resolve<MasterDepartmentWindowModel>();

            departmentWindow.Departments = Departments;
            departmentWindow.SetSelectedDepartment(department, SelectedClient);

            if (_dialogService.ShowDialog(departmentWindow))
            {
                Departments.Clear();
                await GetDepartments();
                RaisePropertyChanged((() => SortedDepartments));
            }
        }

        private void EditStaff()
        {
            StaffWindow(SelectedStaff);
        }

        private void AddNewStaff()
        {
            StaffWindow(null);
        }

        private async void StaffWindow(DepartmentEntityViewModel staff)
        {
            var staffWindow = _resolverService.Resolve<MasterStaffWindowModel>();

            staffWindow.Departments = Departments;
            staffWindow.SetSelectedStaff(staff, SelectedDepartment);

            if (_dialogService.ShowDialog(staffWindow))
            {
                Departments.Clear();
                await GetDepartments();
                RaisePropertyChanged((() => SortedStaffs));
            }
        }


    }
}
