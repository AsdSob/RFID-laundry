using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Services;
using Client.Desktop.ViewModels.Windows;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Content
{
    public class MasterStaffViewModel : ViewModelBase
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private readonly IResolver _resolverService;

        private ObservableCollection<ClientEntityViewModel> _clients;
        private ClientEntityViewModel _selectedClient;
        private ObservableCollection<DepartmentEntityViewModel> _departments;
        private DepartmentEntityViewModel _selectedDepartment;
        private ObservableCollection<StaffDetailsEntityViewModel> _staff;
        private StaffDetailsEntityViewModel _selectedStaffDetails;
        private ObservableCollection<MasterLinenEntityViewModel> _masterLinens;
        private ObservableCollection<ClientLinenEntityViewModel> _linens;
        private ClientLinenEntityViewModel _selectedClientLinen;
        private RfidTagViewModel _selectedTag;
        private RfidServiceTest _reader;

        public RfidTagViewModel SelectedTag
        {
            get => _selectedTag;
            set => Set(() => SelectedTag, ref _selectedTag, value);
        }
        public RfidServiceTest Reader
        {
            get => _reader;
            set => Set(ref _reader, value);
        }
        public ClientLinenEntityViewModel SelectedClientLinen
        {
            get => _selectedClientLinen;
            set => Set(() => SelectedClientLinen, ref _selectedClientLinen, value);
        }
        public ObservableCollection<ClientLinenEntityViewModel> Linens
        {
            get => _linens;
            set => Set(() => Linens, ref _linens, value);
        }
        public ObservableCollection<MasterLinenEntityViewModel> MasterLinens
        {
            get => _masterLinens;
            set => Set(() => MasterLinens, ref _masterLinens, value);
        }
        public StaffDetailsEntityViewModel SelectedStaffDetails
        {
            get => _selectedStaffDetails;
            set => Set(() => SelectedStaffDetails, ref _selectedStaffDetails, value);
        }
        public ObservableCollection<StaffDetailsEntityViewModel> Staff
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

        public ObservableCollection<DepartmentEntityViewModel> SortedDepartments =>
            Departments?.Where(x => x.ClientId == SelectedClient?.Id).ToObservableCollection();

        public ObservableCollection<StaffDetailsEntityViewModel> SortedStaff => SortStaff();

        public ObservableCollection<ClientLinenEntityViewModel> SortedLinens => SortLinen();

        public RelayCommand AddStaffCommand { get; }
        public RelayCommand AddLinenCommand { get; }
        public RelayCommand RfidReaderCommand { get; }
        public RelayCommand EditStaffCommand { get; }
        public RelayCommand EditLinenCommand { get; }

        public RelayCommand AddShowLinenByTagCommand { get; }
        public RelayCommand DeleteTagCommand { get; }

        public RelayCommand InitializeCommand { get; }
        

        public MasterStaffViewModel(RfidServiceTest rfidReader, ILaundryService dataService, IDialogService dialogService, IResolver resolver, IMainDispatcher dispatcher)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver)); 
            Reader = rfidReader ?? throw new ArgumentNullException(nameof(rfidReader));

            AddStaffCommand = new RelayCommand(AddNewStaff, (() => SelectedDepartment != null));
            EditStaffCommand = new RelayCommand(EditStaff, (() => SelectedStaffDetails != null));

            AddLinenCommand = new RelayCommand(() => LinenWindow(null));
            EditLinenCommand = new RelayCommand(() => LinenWindow(SelectedClientLinen), () => SelectedClientLinen != null);

            InitializeCommand = new RelayCommand(Initialize);

            RfidReaderCommand = new RelayCommand(RfidReaderSettings);
            AddShowLinenByTagCommand = new RelayCommand(AddShowLinenByTag, () => SelectedTag != null);
            DeleteTagCommand = new RelayCommand(DeleteTag, () => SelectedTag != null || SelectedClientLinen != null);

            Reader.SetLinens(Linens);
        }

        private async void Initialize()
        {
            _dialogService.ShowBusy();

            try
            {
                var client = await _laundryService.GetAllAsync<ClientEntity>();
                var clients = client.Select(x => new ClientEntityViewModel(x));
                Clients = clients.ToObservableCollection();

                var department = await _laundryService.GetAllAsync<DepartmentEntity>();
                var departments = department.Select(x => new DepartmentEntityViewModel(x));
                Departments = departments.ToObservableCollection();

                var masterLinen = await _laundryService.GetAllAsync<MasterLinenEntity>();
                var masterLinens = masterLinen.Select(x => new MasterLinenEntityViewModel(x));
                MasterLinens = masterLinens.ToObservableCollection();

                GetStaffs();
                GetClientLinens();
            }
            catch (Exception e)
            {
                _dialogService.HideBusy();
            }

            finally
            {
                _dialogService.HideBusy();
            }

            PropertyChanged += OnPropertyChanged;
        }

        private async void GetStaffs()
        {
            var staff = await _laundryService.GetAllAsync<StaffDetailsEntity>();
            var staffs = staff.Select(x => new StaffDetailsEntityViewModel(x));
            Staff = staffs.ToObservableCollection();
        }

        private async void GetClientLinens()
        {
            var linen = await _laundryService.GetAllAsync<ClientLinenEntity>();
            var linens = linen.Select(x => new ClientLinenEntityViewModel(x));
            Linens = linens.ToObservableCollection();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(() => SortedDepartments);
                SelectedDepartment = SortedDepartments.FirstOrDefault();
            }

            if (e.PropertyName == nameof(SelectedDepartment))
            {
                RaisePropertyChanged(()=> SortedStaff);
                RaisePropertyChanged(() => SortedLinens);

                AddStaffCommand.RaiseCanExecuteChanged();
                AddLinenCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedStaffDetails))
            {
                RaisePropertyChanged(() => SortedLinens);
                EditStaffCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedClientLinen))
            {
                EditLinenCommand.RaiseCanExecuteChanged();
                DeleteTagCommand.RaiseCanExecuteChanged();
            }

            if(e.PropertyName == nameof(SelectedTag))
            {
                AddShowLinenByTagCommand.RaiseCanExecuteChanged();
                DeleteTagCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<ClientLinenEntityViewModel> SortLinen()
        {
            var linens = new ObservableCollection<ClientLinenEntityViewModel>();

            if (Linens == null) return linens;

            if (SelectedDepartment == null)
            {
                linens = Linens.Where(x=> x.ClientId == SelectedClient?.Id).ToObservableCollection();
            }
            else
            {
                linens = Linens.Where(x=> x.DepartmentId == SelectedDepartment.Id).ToObservableCollection();
            }

            return linens;
        }

        private ObservableCollection<StaffDetailsEntityViewModel> SortStaff()
        {
            var staffs = new ObservableCollection<StaffDetailsEntityViewModel>();

            if (SelectedDepartment == null)
            {
                if (SortedDepartments == null)
                    return staffs;

                foreach (var department in SortedDepartments)
                {
                    staffs.AddRange(Staff?.Where(x => x.DepartmentId == department?.Id));
                }
            }
            else
            {
                staffs.AddRange(Staff?.Where(x => x.DepartmentId == SelectedDepartment?.Id).ToObservableCollection());
            }

            return staffs;
        }

        private void Save()
        {
            var linens = Linens.Where(x => x.HasChanges());

            foreach (var item in linens)
            {
                item.AcceptChanges();

                _laundryService.AddOrUpdateAsync(item.OriginalObject);
            }

            _dialogService.ShowInfoDialog("All changes saved");
        }

        private void EditStaff()
        {
            StaffWindow(SelectedStaffDetails);
        }

        private void AddNewStaff()
        {
            StaffWindow(null);
        }

        private void StaffWindow(StaffDetailsEntityViewModel staffDetails)
        {
            var staffWindow = _resolverService.Resolve<MasterStaffWindowModel>();

            //staffWindow.SetSelectedStaff(staffDetails, SelectedDepartment);
            //staffWindow.Staffs = Staff;

            if (_dialogService.ShowDialog(staffWindow))
            {
                Staff.Clear();
                GetStaffs();
                RaisePropertyChanged((() => SortedStaff));
            }
        }

        private void LinenWindow(ClientLinenEntityViewModel linen)
        {
            var linenWindow = _resolverService.Resolve<ClientLinenWindowModel>();

            linenWindow.Clients = Clients;
            linenWindow.Departments = Departments;
            linenWindow.MasterLinens = MasterLinens;
            linenWindow.ClientLinens = Linens;

            if (linen == null)
            {
                linen = new ClientLinenEntityViewModel()
                {
                    Tag = SelectedTag?.Tag,
                };
            }

            linenWindow.SetSelectedLinen(linen);

            if (_dialogService.ShowDialog(linenWindow))
            {
                GetClientLinens();
                Reader.SetLinens(Linens);
            }
        }

        private void RfidReaderSettings()
        {
            var window = _resolverService.Resolve<ClientLinenWindowModel>();

            var showDialog = _dialogService.ShowDialog(window);
        }



        private void AddShowLinenByTag()
        {
            if (SelectedTag.IsRegistered)
            {
                var linen = Linens.FirstOrDefault(x => x.Tag == SelectedTag.Tag);
                if (linen == null) return;
                LinenWindow(linen);
            }
            else
            {
                UseTag();
            }
        }

        private void DeleteTag()
        {
            var linen = Linens.FirstOrDefault(x => String.Equals(x.Tag, SelectedTag.Tag));
            if (linen == null)
            {
                return;
            }

            if (!_dialogService.ShowQuestionDialog($"Do you want to Delete \"{SelectedTag.Tag}\" ?"))
            {
                return;
            }

            linen.Tag = null;
            linen.AcceptChanges();

            _laundryService.AddOrUpdateAsync(linen.OriginalObject);
        }

        private void UseTag()
        {
            if (SelectedClientLinen == null || SelectedTag == null) return;

            SelectedClientLinen.Tag = SelectedTag.Tag;
            SelectedClientLinen.AcceptChanges();
            SelectedTag.IsRegistered = true;

            _laundryService.AddOrUpdateAsync(SelectedClientLinen.OriginalObject);
        }


    }
}
