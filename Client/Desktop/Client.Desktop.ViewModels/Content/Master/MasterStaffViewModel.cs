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
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Content.Master
{
    public class MasterStaffViewModel : ViewModelBase
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private readonly IResolver _resolverService;

        private List<ClientEntityViewModel> _clients;
        private ClientEntityViewModel _selectedClient;
        private List<DepartmentEntityViewModel> _departments;
        private DepartmentEntityViewModel _selectedDepartment;
        private ObservableCollection<ClientStaffEntity> _staff;
        private ClientStaffEntity _selectedStaff;
        private ObservableCollection<MasterLinenEntityViewModel> _masterLinens;
        private ObservableCollection<ClientLinenEntityViewModel> _linens;
        private ClientLinenEntityViewModel _selectedClientLinen;
        private ObservableCollection<Tuple<int, string>> _tags;
        private Tuple<int, string> _selectedTag;

        public RfidReaderWindowModel RfidReaderWindow { get; set; }

        public Tuple<int,string> SelectedTag
        {
            get => _selectedTag;
            set => Set(() => SelectedTag, ref _selectedTag, value);
        }
        public ObservableCollection<Tuple<int, string>> Tags
        {
            get => _tags;
            set => Set(() => Tags, ref _tags, value);
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
        public ClientStaffEntity SelectedStaff
        {
            get => _selectedStaff;
            set => Set(() => SelectedStaff, ref _selectedStaff, value);
        }
        public ObservableCollection<ClientStaffEntity> Staff
        {
            get => _staff;
            set => Set(() => Staff, ref _staff, value);
        }
        public DepartmentEntityViewModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(() => SelectedDepartment, ref _selectedDepartment, value);
        }
        public List<DepartmentEntityViewModel> Departments
        {
            get => _departments;
            set => Set(() => Departments, ref _departments, value);
        }
        public ClientEntityViewModel SelectedClient
        {
            get => _selectedClient;
            set => Set(() => SelectedClient, ref _selectedClient, value);
        }
        public List<ClientEntityViewModel> Clients
        {
            get => _clients;
            set => Set(() => Clients, ref _clients, value);
        }

        public ObservableCollection<DepartmentEntityViewModel> SortedDepartments =>
            Departments?.Where(x => x.ClientId == SelectedClient?.Id).ToObservableCollection();

        public ObservableCollection<ClientStaffEntity> SortedStaff =>
            Staff?.Where(x => x.DepartmentId == SelectedDepartment?.Id).ToObservableCollection();

        public ObservableCollection<ClientLinenEntityViewModel> SortedLinens => SortLinen();

        public RelayCommand AddStaffCommand { get; }
        public RelayCommand AddLinenCommand { get; }
        public RelayCommand RfidReaderCommand { get; }
        public RelayCommand EditStaffCommand { get; }
        public RelayCommand EditLinenCommand { get; }

        public RelayCommand AddSelectedTagCommand { get; }

        public RelayCommand InitializeCommand { get; }
        

        public MasterStaffViewModel(ILaundryService dataService, IDialogService dialogService, IResolver resolver, IMainDispatcher dispatcher)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));

            AddStaffCommand = new RelayCommand(AddNewStaff, (() => SelectedDepartment != null));
            EditStaffCommand = new RelayCommand(EditStaff, (() => SelectedStaff != null));

            AddLinenCommand = new RelayCommand(AddLinen, (() => SelectedDepartment != null));
            EditLinenCommand = new RelayCommand(AddLinen, (() => SelectedClientLinen != null));

            RfidReaderCommand = new RelayCommand(RfidReader);
            InitializeCommand = new RelayCommand(Initialize);
            AddSelectedTagCommand = new RelayCommand(AddSelectedTag, (() => SelectedTag != null));

            RfidReaderWindow = _resolverService.Resolve<RfidReaderWindowModel>();

            Tags = new ObservableCollection<Tuple<int, string>>();
        }

        private async void Initialize()
        {
            _dialogService.ShowBusy();

            try
            {
                var client = await _laundryService.GetAllAsync<ClientEntity>();
                var clients = client.Select(x => new ClientEntityViewModel(x));
                Clients = clients.ToList();

                var department = await _laundryService.GetAllAsync<DepartmentEntity>();
                var departments = department.Select(x => new DepartmentEntityViewModel(x));
                Departments = departments.ToList();

                var master = await _laundryService.GetAllAsync<MasterLinenEntity>();
                var masters = master.Select(x => new MasterLinenEntityViewModel(x));
                MasterLinens = masters.ToObservableCollection();

                var linen = await _laundryService.GetAllAsync<ClientLinenEntity>();
                var linens = linen.Select(x => new ClientLinenEntityViewModel(x));
                Linens = new ObservableCollection<ClientLinenEntityViewModel>();
                Linens = linens.ToObservableCollection();

                await GetStaffs();
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

        private async Task GetStaffs()
        {
            var staff = await _laundryService.GetAllAsync<ClientStaffEntity>();
            Staff = staff.ToObservableCollection();
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

            if (e.PropertyName == nameof(SelectedStaff))
            {
                RaisePropertyChanged(() => SortedLinens);
                EditStaffCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedClientLinen))
            {
                EditLinenCommand.RaiseCanExecuteChanged();
            }

            if(e.PropertyName == nameof(SelectedTag))
            {
                AddSelectedTagCommand.RaiseCanExecuteChanged();
            }            
        }

        private ObservableCollection<ClientLinenEntityViewModel> SortLinen()
        {
            var linens = new ObservableCollection<ClientLinenEntityViewModel>();

            if (SelectedDepartment == null) return linens;

            return Linens?.Where(x => x.DepartmentId == SelectedDepartment?.Id).ToObservableCollection();
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
            StaffWindow(SelectedStaff);
        }

        private void AddNewStaff()
        {
            StaffWindow(null);
        }

        private async void StaffWindow(ClientStaffEntity staff)
        {
            var staffWindow = _resolverService.Resolve<MasterStaffWindowModel>();

            staffWindow.SetSelectedStaff(staff, SelectedDepartment.OriginalObject);


            if (_dialogService.ShowDialog(staffWindow))
            {
                Staff.Clear();
                await GetStaffs();
                RaisePropertyChanged((() => SortedStaff));
            }
        }

        private void AddLinen()
        {
            if (SelectedDepartment == null) return;
            //if (!_dialogService.ShowQuestionDialog("Do you want to add new Linen?")) return;

            var newLinen = new ClientLinenEntityViewModel()
            {
                ClientId =  SelectedClient.Id,
                DepartmentId = SelectedDepartment.Id,
                MasterLinenId = 1,
                StatusId = (int) LinenStatusEnum.InUse,
            };

            if (SelectedStaff != null)
            {
                newLinen.OriginalObject.ClientStaffEntity = SelectedStaff;
                newLinen.StaffId = SelectedStaff.Id;
            }

            Linens.Add(newLinen);

            RaisePropertyChanged(()=> SortedLinens);
            SelectedClientLinen = newLinen;
        }

        private void RfidReader()
        {
            RfidReaderWindow.ReaderService.StopRead();

            var showDialog = _dialogService.ShowDialog(RfidReaderWindow);

        }

        private void AddSelectedTag()
        {
            if (SelectedClientLinen == null)
                AddLinen();

            if (SelectedClientLinen.Tag != null)
            {
                if (!_dialogService.ShowQuestionDialog("Linen already has tag \n Do you want to replace it?"))
                {
                    return;
                }
            }

            if (Linens.Any(x => Equals(x.Tag, SelectedTag.Item2)))
            {
                var existLinen = Linens.FirstOrDefault(x => Equals(x.Tag, SelectedTag.Item2));
                var staff = Staff.FirstOrDefault(x => x.Id == existLinen.StaffId);

                if (!_dialogService.ShowQuestionDialog(
                    $"Tag {SelectedTag.Item2} already using by {staff?.Name} in <{existLinen.Id}> linen \n Do you want to shift Tag?")
                ) return;

                existLinen.Tag = null;
            }

            SelectedClientLinen.Tag = SelectedTag.Item2;
        }
        
    }
}
