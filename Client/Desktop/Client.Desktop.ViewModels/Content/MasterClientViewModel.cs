using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Windows;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Content
{
    public class MasterClientViewModel : ViewModelBase
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private readonly IResolver _resolverService;

        #region Parameters

        private ObservableCollection<ClientEntity> _clients;
        private ClientEntity _selectedClient;
        private ObservableCollection<DepartmentEntity> _departments;
        private List<UnitViewModel> _cities;
        private DepartmentEntity _selectedDepartment;
        private List<UnitViewModel> _departmentTypes;
        private ObservableCollection<ClientStaffEntity> _staffs;
        private ClientStaffEntity _selectedStaff;

        public ClientStaffEntity SelectedStaff
        {
            get => _selectedStaff;
            set => Set(ref _selectedStaff, value);
        }
        public ObservableCollection<ClientStaffEntity> Staffs
        {
            get => _staffs;
            set => Set(ref _staffs, value);
        }
        public List<UnitViewModel> DepartmentTypes
        {
            get => _departmentTypes;
            set => Set(() => DepartmentTypes, ref _departmentTypes, value);
        }
        public DepartmentEntity SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(() => SelectedDepartment, ref _selectedDepartment, value);
        }
        public List<UnitViewModel> Cities
        {
            get => _cities;
            set => Set(() => Cities, ref _cities, value);
        }
        public ObservableCollection<DepartmentEntity> Departments
        {
            get => _departments;
            set => Set(() => Departments, ref _departments, value);
        }
        public ClientEntity SelectedClient
        {
            get => _selectedClient;
            set => Set(() => SelectedClient, ref _selectedClient, value);
        }
        public ObservableCollection<ClientEntity> Clients
        {
            get => _clients;
            set => Set(() => Clients, ref _clients, value);
        }

        public ObservableCollection<DepartmentEntity> SortedDepartments => SortDepartments();
        public ObservableCollection<ClientStaffEntity> SortedStaffs => SortStaffs();

        #endregion

        public RelayCommand AddClientCommand { get; }
        public RelayCommand EditClientCommand { get; }
        public RelayCommand DeleteClientCommand { get; }

        public RelayCommand AddDepartmentCommand { get; }
        public RelayCommand EditDepartmentCommand { get; }
        public RelayCommand DeleteDepartmentCommand { get; }

        public RelayCommand AddStaffCommand { get; }
        public RelayCommand EditStaffCommand { get; }
        public RelayCommand DeleteStaffCommand { get; }

        public RelayCommand InitializeCommand { get; }



        public MasterClientViewModel(ILaundryService dataService, IDialogService dialogService, IResolver resolver)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));

            AddClientCommand = new RelayCommand(AddNewClient);
            EditClientCommand = new RelayCommand(EditClient, (() => SelectedClient != null));
            DeleteClientCommand = new RelayCommand(DeleteClient, () => SelectedClient !=null);

            AddDepartmentCommand = new RelayCommand(AddNewDepartment, () => SelectedClient != null);
            EditDepartmentCommand = new RelayCommand(EditDepartment, () => SelectedDepartment != null);
            DeleteDepartmentCommand = new RelayCommand(DeleteDepartment, (() => SelectedDepartment != null));

            AddStaffCommand = new RelayCommand(AddNewStaff, () => SelectedDepartment != null);
            EditStaffCommand = new RelayCommand(EditStaff, () => SelectedStaff != null);
            DeleteStaffCommand = new RelayCommand(DeleteStaff, (() => SelectedStaff != null));
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
                DeleteClientCommand.RaiseCanExecuteChanged();
                EditClientCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged((() => SortedDepartments));
            }

            if (e.PropertyName == nameof(SelectedDepartment))
            {
                DeleteDepartmentCommand.RaiseCanExecuteChanged();
                EditDepartmentCommand.RaiseCanExecuteChanged();
                AddStaffCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedStaff))
            {
                EditStaffCommand.RaiseCanExecuteChanged();
                DeleteStaffCommand.RaiseCanExecuteChanged();
            }

        }

        private async void Initialize()
        {
            await GetClients();
            await GetDepartments();
            await GetStaffs();
        }

        private async Task GetClients()
        {
            var client = await _laundryService.GetAllAsync<ClientEntity>();
            Clients = client.ToObservableCollection();
        }

        private async Task GetDepartments()
        {
            var departments = await _laundryService.GetAllAsync<DepartmentEntity>();
            Departments = departments.ToObservableCollection();
        }

        private async Task GetStaffs()
        {
            var staff = await _laundryService.GetAllAsync<ClientStaffEntity>();
            Staffs = staff.ToObservableCollection();
        }

        private ObservableCollection<DepartmentEntity> SortDepartments()
        {
            return Departments?.Where(x => x.ClientId == SelectedClient?.Id).ToObservableCollection();
        }

        private ObservableCollection<ClientStaffEntity> SortStaffs()
        {
            return Staffs?.Where(x => x.DepartmentId == SelectedDepartment?.Id).ToObservableCollection();
        }

        private void EditClient()
        {
            ClientWindow(SelectedClient);
        }

        private void AddNewClient()
        {
           ClientWindow(null);
        }

        private async void ClientWindow(ClientEntity client)
        {
            var clientWindow = _resolverService.Resolve<MasterClientWindowModel>();

            clientWindow.SetSelectedClient(client);

            _dialogService.ShowDialog(clientWindow);

            if (clientWindow.HasChanges)
            {
                Clients.Clear();
                await GetClients();
            }
        }

        private void DeleteClient()
        {
            if(SelectedClient == null) return;
            if(!_dialogService.ShowQuestionDialog($"Do you want to DELETE {SelectedClient.Name} ?"))
                return;

            var client = SelectedClient;
            SelectedClient = null;

            _laundryService.DeleteAsync(client);

            Clients.Remove(client);
        }

        private void EditDepartment()
        {
            DepartmentWindow(SelectedDepartment);
        }

        private void AddNewDepartment()
        {
            DepartmentWindow(null);
        }

        private async void DepartmentWindow(DepartmentEntity department)
        {
            var departmentWindow = _resolverService.Resolve<MasterDepartmentWindowModel>();

            departmentWindow.SetSelectedDepartment(department, SelectedClient);

            _dialogService.ShowDialog(departmentWindow);

            if (departmentWindow.HasChanges)
            {
                Departments.Clear();
                await GetDepartments();
                RaisePropertyChanged((() => SortedDepartments));
            }
        }

        private void DeleteDepartment()
        {
            if (SelectedDepartment == null) return;
            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {SelectedDepartment.Name} ?"))
                return;

            var department = SelectedDepartment;
            SelectedDepartment = null;

            _laundryService.DeleteAsync(department);

            Departments.Remove(department);
        }


        private void EditStaff()
        {
            StaffWindow(SelectedStaff);
        }

        private void AddNewStaff()
        {
            StaffWindow(null);
        }

        private async void StaffWindow(ClientStaffEntity staff)
        {
            var staffWindow = _resolverService.Resolve<MasterStaffWindowModel>();

            staffWindow.SetSelectedStaff(staff, SelectedDepartment);

            _dialogService.ShowDialog(staffWindow);

            if (staffWindow.HasChanges)
            {
                Staffs.Clear();
                await GetStaffs();
                RaisePropertyChanged((() => SortedStaffs));
            }
        }

        private void DeleteStaff()
        {
            if (SelectedStaff == null) return;
            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {SelectedStaff.Name} ?"))
                return;

            var staff = SelectedStaff;
            SelectedStaff = null;

            _laundryService.DeleteAsync(staff);

            Staffs.Remove(staff);
        }

    }
}
