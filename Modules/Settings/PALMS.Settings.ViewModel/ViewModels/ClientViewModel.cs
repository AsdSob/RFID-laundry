using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using PALMS.Settings.ViewModel.EntityViewModels;
using PALMS.ViewModels.Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;

namespace PALMS.Settings.ViewModel.ViewModels
{
    public class ClientViewModel : ViewModelBase
    {
        private readonly IDispatcher _dispatcher;
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<ClientEntityViewModel> _clients;
        private ClientEntityViewModel _selectedClient;
        private List<UnitViewModel> _cities;
        private ObservableCollection<DepartmentEntityViewModel> _departments;
        private DepartmentEntityViewModel _selectedDepartment;
        private ObservableCollection<ClientStaffEntityViewModel> _staff;
        private ClientStaffEntityViewModel _selectedStaff;

        public ClientStaffEntityViewModel SelectedStaff
        {
            get => _selectedStaff;
            set => Set(() => SelectedStaff, ref _selectedStaff, value);
        }
        public ObservableCollection<ClientStaffEntityViewModel> Staff
        {
            get => _staff;
            set => Set(() => Staff, ref _staff, value);
        }
        public DepartmentEntityViewModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(() => SelectedDepartment, ref _selectedDepartment, value);
        }
        public ObservableCollection<DepartmentEntityViewModel> Departments
        {
            get => _departments;
            set => Set(() => Departments, ref _departments, value);
        }
        public List<UnitViewModel> Cities
        {
            get => _cities;
            set => Set(() => Cities, ref _cities, value);
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

        public ObservableCollection<ClientEntityViewModel> SortedClients => SortClients(null);
        public ObservableCollection<ClientEntityViewModel> SortedLocations => SortClients(SelectedClient?.Id);

        public ObservableCollection<DepartmentEntityViewModel> SortedDepartments => SortDepartments();
        public ObservableCollection<ClientStaffEntityViewModel> SortedStaff => SortStaff();

        public RelayCommand SaveCommand { get; }
        public RelayCommand AddClientCommand { get; }
        public RelayCommand RemoveClientCommand { get; }
        public RelayCommand RemoveDepartmentCommand { get; }
        public RelayCommand AddDepartmentCommand { get; }
        public RelayCommand AddStaffCommand { get; }
        public RelayCommand RemoveStaffCommand { get; }


        public async void  GetData()
        {
            Clients = new ObservableCollection<ClientEntityViewModel>();
            Departments = new ObservableCollection<DepartmentEntityViewModel>();
            Staff = new ObservableCollection<ClientStaffEntityViewModel>();

            var client = await _dataService.GetAsync<Client>();
            var clients = client.Select(x => new ClientEntityViewModel(x));
            _dispatcher.RunInMainThread(() => Clients = clients.ToObservableCollection());

            var department = await _dataService.GetAsync<Department>();
            var departments = department.Select(x => new DepartmentEntityViewModel(x));
            _dispatcher.RunInMainThread(() => Departments = departments.ToObservableCollection());

            var staff = await _dataService.GetAsync<ClientStaff>();
            var staffs = staff.Select(x => new ClientStaffEntityViewModel(x));
            _dispatcher.RunInMainThread(() => Staff = staffs.ToObservableCollection());

            Cities = EnumExtentions.GetValues<Cities>();

            SelectedClient = null;
        }

        public ClientViewModel(IDispatcher dispatcher, IDataService dataService, IDialogService dialogService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            GetData();

            AddClientCommand = new RelayCommand(AddClient);
            RemoveClientCommand = new RelayCommand(RemoveClient, () => SelectedClient != null);

            AddDepartmentCommand = new RelayCommand(AddDepartment, () => SelectedClient != null);
            RemoveDepartmentCommand = new RelayCommand(RemoveDepartment, () => SelectedDepartment != null);

            AddStaffCommand = new RelayCommand(AddStaff, () => SelectedDepartment != null);
            RemoveStaffCommand = new RelayCommand(RemoveStaff, () => SelectedStaff != null);

            SaveCommand = new RelayCommand(Save);


            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(()=> SortedDepartments);

                RemoveClientCommand.RaiseCanExecuteChanged();
                AddDepartmentCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedDepartment))
            {
                RaisePropertyChanged(() => SortedStaff);

                RemoveDepartmentCommand.RaiseCanExecuteChanged();
                AddStaffCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedStaff))
            {
                RemoveStaffCommand.RaiseCanExecuteChanged();
            }

        }

        private void Save()
        {
            if(!HasChanges())return;
            if(!_dialogService.ShowQuestionDialog("Do you want to save all changes")) return;

            if(Clients.Any(x=> x.HasChanges()))
            {
                var clients = Clients.Where(x => x.HasChanges()).ToList();

                clients.ForEach(x=> x.AcceptChanges());

                _dataService.AddOrUpdateAsync(clients.Select(x => x.OriginalObject));
            }

            if (Departments.Any(x => x.HasChanges()))
            {
                var departments = Departments.Where(x => x.HasChanges()).ToList();

                departments.ForEach(x => x.AcceptChanges());

                _dataService.AddOrUpdateAsync(departments.Select(x => x.OriginalObject));
            }

            if (Staff.Any(x => x.HasChanges()))
            {
                var staff = Staff.Where(x => x.HasChanges()).ToList().ToList();

                staff.ForEach(x => x.AcceptChanges());

                _dataService.AddOrUpdateAsync(staff.Select(x => x.OriginalObject));
            }
        }

        private bool HasChanges()
        {
            return Clients.Any(x => x.HasChanges()) || Departments.Any(x => x.HasChanges()) || Staff.Any(x => x.HasChanges());
        }

        #region Client & Location

        private ObservableCollection<ClientEntityViewModel> SortClients(int? clientId)
        {
            var clients = new ObservableCollection<ClientEntityViewModel>();

            if (clientId != null)
            {
                clients = Clients?.Where(x => x.ParentId == clientId).ToObservableCollection();
            }
            else
            {
                clients = Clients?.Where(x => x.ParentId == null).ToObservableCollection();
            }
            return clients;
        }

        private void AddClient()
        {
            Clients.Add(new ClientEntityViewModel()
            {
                Active = true,
            });
        }

        private void RemoveClient()
        {
            if(SelectedClient == null) return;

            if (SelectedClient.OriginalObject != null)
            {
                if(!_dialogService.ShowQuestionDialog("Do you want to DELETE client"))return;

                _dataService.DeleteAsync(SelectedClient.OriginalObject);
            }
            Clients.Remove(SelectedClient);
        }

        #endregion

        #region Department

        private ObservableCollection<DepartmentEntityViewModel> SortDepartments()
        {
            if (SelectedClient == null) return null;

            var departments = Departments.Where(x => x.ClientId == SelectedClient.Id).ToObservableCollection();

            return departments;
        }

        private void AddDepartment()
        {
            var dep = new DepartmentEntityViewModel()
            {
                ParentId = 0,
                ClientId = SelectedClient.Id,
            };
            if (SelectedClient.IsNew)
            {
                dep.OriginalObject.Client = SelectedClient.OriginalObject;
            }

            dep.OriginalObject.Client = SelectedClient.OriginalObject;

            Departments.Add(dep);
            RaisePropertyChanged(() => SortedDepartments);
        }

        private void RemoveDepartment()
        {
            if (SelectedDepartment == null) return;

            if (SelectedDepartment.OriginalObject != null)
            {
                if (!_dialogService.ShowQuestionDialog("Do you want to DELETE department")) return;

                _dataService.DeleteAsync(SelectedDepartment.OriginalObject);
            }
            Departments.Remove(SelectedDepartment);
        }

        #endregion

        #region Staff

        private ObservableCollection<ClientStaffEntityViewModel> SortStaff()
        {
            if (SelectedDepartment == null) return null;

            var staff = Staff.Where(x => x.DepartmentId == SelectedDepartment.Id).ToObservableCollection();

            return staff;
        }

        private void AddStaff()
        {
            var staff = new ClientStaffEntityViewModel()
            {
                DepartmentId = SelectedDepartment.Id,
            };
            if (SelectedDepartment.IsNew)
            {
                staff.OriginalObject.Department = SelectedDepartment.OriginalObject;
            }

            Staff.Add(staff);
            RaisePropertyChanged(() => SortedStaff);
        }

        private void RemoveStaff()
        {
            if (SelectedStaff == null) return;

            if (SelectedStaff.OriginalObject != null)
            {
                if (!_dialogService.ShowQuestionDialog("Do you want to DELETE staff")) return;

                _dataService.DeleteAsync(SelectedStaff.OriginalObject);
            }
            Staff.Remove(SelectedStaff);
        }

        #endregion

    }
}
