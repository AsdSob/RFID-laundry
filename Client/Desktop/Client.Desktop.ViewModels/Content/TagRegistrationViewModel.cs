using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Windows;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Content
{
    public class TagRegistrationViewModel :ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly ILaundryService _laundryService;
        private readonly IResolver _resolverService;
        private readonly IMainDispatcher _dispatcher;

        private ObservableCollection<ClientEntity> _clients;
        private ClientEntity _selectedClient;
        private ObservableCollection<DepartmentEntity> _departments;
        private DepartmentEntity _selectedDepartment;
        private ObservableCollection<ClientStaffEntity> _staffs;
        private ClientStaffEntity _selectedStaff;
        private ObservableCollection<ClientLinenEntity> _linens;
        private ClientLinenEntity _selectedClientLinen;
        private List<MasterLinenEntity> _masterLinens;
        private Tuple<int, string> _selectedTag;

        public Tuple<int, string> SelectedTag
        {
            get => _selectedTag;
            set => Set(() => SelectedTag, ref _selectedTag, value);
        }
        public List<MasterLinenEntity> MasterLinens
        {
            get => _masterLinens;
            set => Set(() => MasterLinens, ref _masterLinens, value);
        }
        public ClientLinenEntity SelectedClientLinen
        {
            get => _selectedClientLinen;
            set => Set(() => SelectedClientLinen, ref _selectedClientLinen, value);
        }
        public ObservableCollection<ClientLinenEntity> Linens
        {
            get => _linens;
            set => Set(() => Linens, ref _linens, value);
        }
        public ClientStaffEntity SelectedStaff
        {
            get => _selectedStaff;
            set => Set(() => SelectedStaff, ref _selectedStaff, value);
        }
        public ObservableCollection<ClientStaffEntity> Staffs
        {
            get => _staffs;
            set => Set(() => Staffs, ref _staffs, value);
        }
        public DepartmentEntity SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(() => SelectedDepartment, ref _selectedDepartment, value);
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

        public RfidReaderWindowModel RfidReaderWindow { get; set; }
        public string SearchingTag { get; set; }

        public ObservableCollection<DepartmentEntity> SortedDepartments =>
            Departments?.Where(x => x?.ClientId == SelectedClient?.Id).ToObservableCollection();

        public ObservableCollection<ClientStaffEntity> SortedStaff => SortStaff();
            
        public ObservableCollection<ClientLinenEntity> SortedLinens =>
            Linens?.Where(x => x?.StaffId == SelectedStaff?.Id).ToObservableCollection();


        public RelayCommand NewStaffCommand { get; }
        public RelayCommand EditStaffCommand { get; }
        public RelayCommand DeleteStaffCommand { get; }

        public RelayCommand NewLinenCommand { get; }
        public RelayCommand EditLinenCommand { get; }
        public RelayCommand DeleteLinenCommand { get; }

        public RelayCommand InitializeCommand { get; }
        public RelayCommand SearchTagCommand { get; }
        public RelayCommand UseTagCommand { get; }
        public RelayCommand ClearSelectedDepartmentCommand { get; }


        public TagRegistrationViewModel(ILaundryService dataService, IDialogService dialogService, IResolver resolver, IMainDispatcher dispatcher)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            NewStaffCommand = new RelayCommand(AddNewStaff,(() => SelectedDepartment !=null));
            EditStaffCommand = new RelayCommand(EditStaff,(() => SelectedStaff !=null));
            DeleteStaffCommand = new RelayCommand(DeleteStaff,(() => SelectedStaff !=null));

            NewLinenCommand = new RelayCommand(()=> LinenWindow(null),() => SelectedStaff !=null);
            EditLinenCommand = new RelayCommand(() => LinenWindow(SelectedClientLinen),() => SelectedStaff !=null);
            DeleteLinenCommand = new RelayCommand(DeleteLinen,(() => SelectedClientLinen !=null));

            SearchTagCommand = new RelayCommand(SearchTag);
            ClearSelectedDepartmentCommand = new RelayCommand(ClearSelectedDepartment);

            UseTagCommand = new RelayCommand(UseTag, (() => SelectedTag != null));

            InitializeCommand = new RelayCommand(Initialize);
            RfidReaderWindow = _resolverService.Resolve<RfidReaderWindowModel>();

            PropertyChanged += OnPropertyChanged;
        }

        private async void Initialize()
        {
            var client = await _laundryService.GetAllAsync<ClientEntity>();
            Clients = client.ToObservableCollection();

            var departments = await _laundryService.GetAllAsync<DepartmentEntity>();
            Departments = departments.ToObservableCollection();

            var masterLinen = await _laundryService.GetAllAsync<MasterLinenEntity>();
            MasterLinens = masterLinen.ToList();

            var staff = await _laundryService.GetAllAsync<ClientStaffEntity>();
            Staffs = staff.ToObservableCollection();

            var linen = await _laundryService.GetAllAsync<ClientLinenEntity>();
            Linens = linen.ToObservableCollection();

        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(() => SortedDepartments);
            }

            if (e.PropertyName == nameof(SelectedDepartment))
            {
                NewStaffCommand?.RaiseCanExecuteChanged();

                RaisePropertyChanged(() => SortedStaff);
            }

            if (e.PropertyName == nameof(SelectedStaff))
            {
                EditStaffCommand.RaiseCanExecuteChanged();
                NewLinenCommand?.RaiseCanExecuteChanged();
                DeleteStaffCommand?.RaiseCanExecuteChanged();
                
                RaisePropertyChanged(() => SortedLinens);
            }

            if (e.PropertyName == nameof(SelectedClientLinen))
            {
                DeleteLinenCommand?.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedTag))
            {
                UseTagCommand?.RaiseCanExecuteChanged();

                SearchingTag = SelectedTag.Item2;
            }
        }

        private ObservableCollection<ClientStaffEntity> SortStaff()
        {
            var staff = new ObservableCollection<ClientStaffEntity>();

            if (SelectedDepartment == null)
            {
                foreach (var department in SortedDepartments.Where(x=> x.ClientId == SelectedClient?.Id))
                {
                    staff.AddRange(Staffs?.Where(x => x?.DepartmentId == department?.Id).ToObservableCollection());
                }
            }
            else
            {
                staff =Staffs?.Where(x => x?.DepartmentId == SelectedDepartment?.Id).ToObservableCollection();
            }

            return staff;
        }

        private void ClearSelectedDepartment()
        {
            SelectedDepartment = null;
        }

        private async void DeleteLinen()
        {
            var masterLinen = MasterLinens.FirstOrDefault(x => x.Id == SelectedClientLinen.MasterLinenId);

            if (!_dialogService.ShowQuestionDialog(
                    $"Do you want to DELETE {masterLinen?.Name} ?")) 
                return;

            await _laundryService.DeleteAsync(SelectedClientLinen);

            Linens.Remove(SelectedClientLinen);
        }

        private void EditStaff()
        {
            StaffWindow(SelectedStaff);
        }

        private void AddNewStaff()
        {
            StaffWindow(null);
        }

        private void StaffWindow(ClientStaffEntity staff)
        {
            var staffWindow = _resolverService.Resolve<MasterStaffWindowModel>();

            staffWindow.SetSelectedStaff(staff, SelectedDepartment);

            _dialogService.ShowDialog(staffWindow);

            if (staffWindow.HasChanges)
            {
                Staffs.Clear();
                Initialize();
                RaisePropertyChanged((() => SortedStaff));
            }
        }

        private void DeleteStaff()
        {
            var staff = SelectedStaff;

            if (staff == null) return;
            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {staff.Name}?")) return;

            _laundryService.DeleteAsync(staff);
            Staffs.Remove(staff);

            var linens = Linens.Where(x => x.StaffId == staff.Id).ToList();

            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {staff.Name} linens ?"))
            {
                foreach (var linen in linens)
                {
                    linen.StaffId = null;
                    _laundryService.AddOrUpdateAsync(linen);
                }
                return;
            }

            foreach (var linen in linens)
            {
                _laundryService.DeleteAsync(linen);
                Linens.Remove(linen);
            }
        }


        private void LinenWindow(ClientLinenEntity linen)
        {
            var linenWindow = _resolverService.Resolve<ClientLinenWindowModel>();

            linenWindow.SetSelectedLinen(linen);
            linenWindow.SelectedClient = SelectedClient;
            linenWindow.SelectedDepartment = SelectedDepartment;
            linenWindow.SelectedStaff = SelectedStaff;

            _dialogService.ShowDialog(linenWindow);

            if (linenWindow.HasChanges)
            {
                Linens.Clear();
                Initialize();
                RaisePropertyChanged((() => SortedLinens));
            }
        }

        private void SearchTag()
        {
            if(String.IsNullOrWhiteSpace(SearchingTag)) return;

            var linen = Linens.FirstOrDefault(x => x.Tag == SearchingTag);

            SelectedClient = Clients.FirstOrDefault(x => x.Id == linen?.ClientId);
            SelectedDepartment = SortedDepartments.FirstOrDefault(x => x.Id == linen?.DepartmentId);
            SelectedStaff = SortedStaff.FirstOrDefault(x => x.Id == linen?.StaffId);
        }

        private void UseTag()
        {
            if(SelectedClientLinen ==null || SelectedTag == null) return;

            if (Linens.Any(x => x.Tag == SelectedTag.Item2))
            {
                var existLinen = Linens.FirstOrDefault(x => Equals(x.Tag, SelectedTag.Item2));
                var staff = Staffs.FirstOrDefault(x => x.Id == existLinen.StaffId);

                if (!_dialogService.ShowQuestionDialog(
                    $"Tag {SelectedTag.Item2} already using by {staff?.Name} in <{existLinen.Id}> linen \n Do you want to shift Tag?")
                ) return;

                existLinen.Tag = null;
                SelectedClientLinen.Tag = SelectedTag.Item2;
                return;
            }

            if (!String.IsNullOrWhiteSpace(SelectedClientLinen.Tag))
            {
                if (_dialogService.ShowQuestionDialog($"Selected linen has tag, \n Do you want to replace it?"))
                {
                    SelectedClientLinen.Tag = SelectedTag.Item2;
                    return;
                }
            }

        }
    }
}
