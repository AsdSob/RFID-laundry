using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Settings.ViewModel.EntityViewModels;
using PALMS.Settings.ViewModel.Windows;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Settings.ViewModel.ViewModels
{
    public class LinenViewModel : ViewModelBase
    {
        private readonly IDispatcher _dispatcher;
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private readonly IResolver _resolver;

        private ClientLinenEntityViewModel _selectedLinen;
        private ObservableCollection<ClientLinenEntityViewModel> _clientLinens;
        private List<ClientEntityViewModel> _clients;
        private List<DepartmentEntityViewModel> _departments;
        private List<ClientStaffEntityViewModel> _staff;
        private ObservableCollection<MasterLinenEntityViewModel> _masterLinens;
        private MasterLinenEntityViewModel _selectedMasterLinen;
        private ClientEntityViewModel _selectedClient;
        private DepartmentEntityViewModel _selectedDepartment;
        private ClientStaffEntityViewModel _selectedStaff;

        
        public ClientStaffEntityViewModel SelectedStaff
        {
            get => _selectedStaff;
            set => Set(() => SelectedStaff, ref _selectedStaff, value);
        }
        public DepartmentEntityViewModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(() => SelectedDepartment, ref _selectedDepartment, value);
        }
        public ClientEntityViewModel SelectedClient
        {
            get => _selectedClient;
            set => Set(() => SelectedClient, ref _selectedClient, value);
        }
        public MasterLinenEntityViewModel SelectedMasterLinen
        {
            get => _selectedMasterLinen;
            set => Set(() => SelectedMasterLinen, ref _selectedMasterLinen, value);
        }
        public ObservableCollection<MasterLinenEntityViewModel> MasterLinens
        {
            get => _masterLinens;
            set => Set(() => MasterLinens, ref _masterLinens, value);
        }
        public List<ClientStaffEntityViewModel> Staff
        {
            get => _staff;
            set => Set(() => Staff, ref _staff, value);
        }
        public List<DepartmentEntityViewModel> Departments
        {
            get => _departments;
            set => Set(() => Departments, ref _departments, value);
        }
        public List<ClientEntityViewModel> Clients
        {
            get => _clients;
            set => Set(() => Clients, ref _clients, value);
        }
        public ObservableCollection<ClientLinenEntityViewModel> ClientLinens
        {
            get => _clientLinens;
            set => Set(() => ClientLinens, ref _clientLinens, value);
        }
        public ClientLinenEntityViewModel SelectedLinen
        {
            get => _selectedLinen;
            set => Set(() => SelectedLinen, ref _selectedLinen, value);
        }

        public ObservableCollection<DepartmentEntityViewModel> SortedDepartments => SortDepartment();
        public ObservableCollection<ClientStaffEntityViewModel> SortedStaff => SortStaff();
        public ObservableCollection<ClientLinenEntityViewModel> SortedLinens => SortClientLinen();
        public ObservableCollection<MasterLinenEntityViewModel> SortedMasterLinens => MasterLinens;

        public RelayCommand AddMasterLinenCommand { get; }
        public RelayCommand DeleteMasterLinenCommand { get; }
        public RelayCommand AddClientLinenCommand { get; }
        public RelayCommand DeleteClientLinenCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand GetRfidTagCommand { get; }

        public async void GetData()
        {
            Clients = new List<ClientEntityViewModel>();
            Departments = new List<DepartmentEntityViewModel>();
            Staff = new List<ClientStaffEntityViewModel>();
            MasterLinens = new ObservableCollection<MasterLinenEntityViewModel>();
            ClientLinens = new ObservableCollection<ClientLinenEntityViewModel>();

            var client = await _dataService.GetAsync<Client>();
            var clients = client.Select(x => new ClientEntityViewModel(x));
            _dispatcher.RunInMainThread(() => Clients = clients.ToList());

            var department = await _dataService.GetAsync<Department>();
            var departments = department.Select(x => new DepartmentEntityViewModel(x));
            _dispatcher.RunInMainThread(() => Departments = departments.ToList());

            var staff = await _dataService.GetAsync<ClientStaff>();
            var staffs = staff.Select(x => new ClientStaffEntityViewModel(x));
            _dispatcher.RunInMainThread(() => Staff = staffs.ToList());

            var master = await _dataService.GetAsync<MasterLinen>();
            var masters = master.Select(x => new MasterLinenEntityViewModel(x));
            _dispatcher.RunInMainThread(() => MasterLinens = masters.ToObservableCollection());

            var linen = await _dataService.GetAsync<ClientLinen>();
            var linens = linen.Select(x => new ClientLinenEntityViewModel(x));
            _dispatcher.RunInMainThread(() => ClientLinens = linens.ToObservableCollection());

        }

        public LinenViewModel(IDispatcher dispatcher, IDataService dataService, IDialogService dialogService, IResolver resolver)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));

            GetData();

            AddMasterLinenCommand = new RelayCommand(AddMasterLinen, () => SelectedDepartment != null);
            DeleteMasterLinenCommand = new RelayCommand(RemoveMasterLinen, () => SelectedMasterLinen != null);

            AddClientLinenCommand = new RelayCommand(AddClientLinen);
            DeleteClientLinenCommand = new RelayCommand(RemoveClientLinen, () => SelectedLinen != null);

            SaveCommand = new RelayCommand(Save);
            GetRfidTagCommand = new RelayCommand(GetRfidTag, () => SelectedLinen != null);


            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(() => SortedDepartments);
            }

            if (e.PropertyName == nameof(SelectedDepartment))
            {
                RaisePropertyChanged(() => SortedStaff);
                AddMasterLinenCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedStaff))
            {
                RaisePropertyChanged(() => SortedLinens);
                RaisePropertyChanged(() => SortedMasterLinens);
            }

            if (e.PropertyName == nameof(SelectedMasterLinen))
            {
                DeleteMasterLinenCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedLinen))
            {
                DeleteClientLinenCommand.RaiseCanExecuteChanged();

                GetRfidTagCommand.RaiseCanExecuteChanged();
            }

        }

        private void Save()
        {
            if (!HasChanges()) return;
            if (!_dialogService.ShowQuestionDialog("Do you want to save all changes")) return;

            if (MasterLinens.Any(x => x.HasChanges()))
            {
                var masterLinen = MasterLinens.Where(x => x.HasChanges());

                masterLinen.ForEach(x => x.AcceptChanges());

                _dataService.AddOrUpdateAsync(masterLinen.Select(x => x.OriginalObject));
            }

            if (ClientLinens.Any(x => x.HasChanges()))
            {
                var linens = ClientLinens.Where(x => x.HasChanges());

                linens.ForEach(x => x.AcceptChanges());

                _dataService.AddOrUpdateAsync(linens.Select(x => x.OriginalObject));
            }
        }

        private bool HasChanges()
        {
            return ClientLinens.Any(x => x.HasChanges()) || MasterLinens.Any(x => x.HasChanges());
        }

        private async void GetRfidTag()
        {
            if(SelectedLinen == null)return;

            var readTag = _resolver.Resolve<ReadTagWindowViewModel>();

            await readTag.InitializeAsync();
            var showDialog = _dialogService.ShowDialog(readTag);
            if (!showDialog) return;

            var tag = readTag.GetSelectedTag();

            SelectedLinen.Tag = tag;
        }

        private ObservableCollection<DepartmentEntityViewModel> SortDepartment()
        {
            var department = new ObservableCollection<DepartmentEntityViewModel>();

            department = Departments.Where(x => x.ClientId == SelectedClient?.Id).ToObservableCollection();

            return department;
        }

        private ObservableCollection<ClientStaffEntityViewModel> SortStaff()
        {
            var staff = new ObservableCollection<ClientStaffEntityViewModel>();

            staff = Staff.Where(x => x.DepartmentId == SelectedDepartment?.Id).ToObservableCollection();

            return staff;
        }

        #region MasterLinen

        private ObservableCollection<MasterLinenEntityViewModel> SortMasterLinen()
        {
            var masterLinen = MasterLinens;

            foreach (var linen in SortedLinens)
            {
                masterLinen.Remove(MasterLinens.FirstOrDefault(x => x.Id == linen.MasterLinenId));
            }

            return masterLinen;
        }

        private void AddMasterLinen()
        {
            MasterLinens.Add(new MasterLinenEntityViewModel()
            {
                PackingValue = 1,
            });

            RaisePropertyChanged(()=> SortedMasterLinens);
        }

        private void RemoveMasterLinen()
        {
            if (SelectedMasterLinen == null) return;

            if (SelectedMasterLinen.OriginalObject != null)
            {
                if (!_dialogService.ShowQuestionDialog("Do you want to DELETE master linen")) return;

                _dataService.DeleteAsync(SelectedMasterLinen.OriginalObject);
            }
            MasterLinens.Remove(SelectedMasterLinen);
        }

        #endregion

        #region ClientLinen

        private ObservableCollection<ClientLinenEntityViewModel> SortClientLinen()
        {
            var linen = new ObservableCollection<ClientLinenEntityViewModel>();

            linen = ClientLinens.Where(x => x.StaffId == SelectedStaff?.Id).ToObservableCollection();

            return linen;
        }

        private void AddClientLinen()
        {
            if( SelectedDepartment == null) return;

            var newLinen = new ClientLinenEntityViewModel()
            {
               DepartmentId = SelectedDepartment.Id,
                StatusId  = 1,
            };

            if (SelectedMasterLinen != null)
            {
                newLinen.MasterLinenId = SelectedMasterLinen.Id;
                newLinen.PackingValue = SelectedMasterLinen.PackingValue;
            }

            if (SelectedStaff != null) newLinen.StaffId = SelectedStaff.Id;

            ClientLinens.Add(newLinen);

            RaisePropertyChanged(() => SortedLinens);
            RaisePropertyChanged(()=> MasterLinens);
        }

        private void RemoveClientLinen()
        {
            if (SelectedLinen == null) return;

            if (SelectedLinen.OriginalObject != null)
            {
                if (!_dialogService.ShowQuestionDialog("Do you want to DELETE master linen")) return;

                _dataService.DeleteAsync(SelectedLinen.OriginalObject);
            }
            ClientLinens.Remove(SelectedLinen);
        }

        #endregion




    }
}
