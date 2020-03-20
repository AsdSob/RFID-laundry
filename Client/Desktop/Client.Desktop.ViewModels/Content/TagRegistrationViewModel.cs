using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Client.Desktop.ViewModels.Common.EntityViewModels;
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
        private ObservableCollection<LinenEntityViewModel> _linens;
        private LinenEntityViewModel _selectedLinen;
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
        public LinenEntityViewModel SelectedLinen
        {
            get => _selectedLinen;
            set => Set(() => SelectedLinen, ref _selectedLinen, value);
        }
        public ObservableCollection<LinenEntityViewModel> Linens
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

        public ObservableCollection<ClientStaffEntity> SortedStaff =>
            Staffs?.Where(x => x?.DepartmentId == SelectedDepartment?.Id).ToObservableCollection();

        public ObservableCollection<LinenEntityViewModel> SortedLinens =>
            Linens?.Where(x => x?.StaffId == SelectedStaff?.Id).ToObservableCollection();


        public RelayCommand SaveCommand { get; }
        public RelayCommand NewStaffCommand { get; }
        public RelayCommand NewLinenCommand { get; }
        public RelayCommand DeleteLinenCommand { get; }
        public RelayCommand InitializeCommand { get; }
        public RelayCommand SearchTagCommand { get; }
        public RelayCommand FindTagCommand { get; }
        public RelayCommand UseTagCommand { get; }


        public TagRegistrationViewModel(ILaundryService dataService, IDialogService dialogService, IResolver resolver, IMainDispatcher dispatcher)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            SaveCommand = new RelayCommand(Save);
            NewStaffCommand = new RelayCommand(NewStaff,(() => SelectedDepartment !=null));

            NewLinenCommand = new RelayCommand(NewLinen,(() => SelectedStaff !=null));
            DeleteLinenCommand = new RelayCommand(DeleteLinen,(() => SelectedLinen !=null));

            SearchTagCommand = new RelayCommand(SearchTag);
            FindTagCommand = new RelayCommand(FindTag, (() => SelectedTag != null));

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

            var staff = await _laundryService.GetAllAsync<ClientStaffEntity>();
            Staffs = staff.ToObservableCollection();

            var masterLinen = await _laundryService.GetAllAsync<MasterLinenEntity>();
            MasterLinens = masterLinen.ToList();

            var linen = await _laundryService.GetAllAsync<ClientLinenEntity>();
            var linens = linen.Select(x => new LinenEntityViewModel(x));
            Linens = linens.ToObservableCollection();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(() => SortedDepartments);
            }

            if (e.PropertyName == nameof(SelectedDepartment))
            {
                RaisePropertyChanged(()=> SortedStaff);
                NewStaffCommand?.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedStaff))
            {
                RaisePropertyChanged(() => SortedLinens);
                NewLinenCommand?.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedLinen))
            {
                DeleteLinenCommand?.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedTag))
            {
                UseTagCommand?.RaiseCanExecuteChanged();
                FindTagCommand?.RaiseCanExecuteChanged();
            }
        }

        private async void Save()
        {
            var linens = Linens.Where(x => x.HasChanges());

            foreach (var linen in linens)
            {
                linen.AcceptChanges();

                await _laundryService.AddOrUpdateAsync(linen.OriginalObject);
            }

            _dialogService.ShowInfoDialog("All changes has been saved");
        }

        private bool SaveCommandCanExecute()
        {
            return Linens.Any(x=> x.HasChanges());
        }

        private async void DeleteLinen()
        {
            var masterLinen = MasterLinens.FirstOrDefault(x => x.Id == SelectedLinen.MasterLinenId);

            if (!_dialogService.ShowQuestionDialog(
                    $"Do you want to DELETE {masterLinen.Name} ?")) 
                return;

            await _laundryService.DeleteAsync(SelectedLinen.OriginalObject);

            Linens.Remove(SelectedLinen);
        }

        private void NewStaff()
        {
            var newStaff = new StaffEntityViewModel(){
                DepartmentId = SelectedDepartment.Id,
                PhoneNumber = "+971",
            };

            var staffWindow = _resolverService.Resolve<StaffChangeWindowModel>();
            staffWindow.SetSelectedStaff(newStaff);
            var showDialog = _dialogService.ShowDialog(staffWindow);
            if (!showDialog) return;

            Staffs.Add(staffWindow.SelectedStaff.OriginalObject);

            RaisePropertyChanged((() => SortedStaff));
        }

        private void NewLinen()
        {
            var newLinen = new LinenEntityViewModel()
            {
                ClientId = SelectedClient.Id,
                DepartmentId = SelectedDepartment.Id,
                StaffId = SelectedStaff?.Id,
                StatusId = 0,
            };

            Linens.Add(newLinen);

            RaisePropertyChanged(()=> SortedLinens);
        }

        private void SearchTag()
        {
            if(String.IsNullOrWhiteSpace(SearchingTag)) return;

            var linen = Linens.FirstOrDefault(x => x.Tag == SearchingTag);

            SelectedClient = Clients.FirstOrDefault(x => x.Id == linen?.ClientId);
            SelectedDepartment = SortedDepartments.FirstOrDefault(x => x.Id == linen?.DepartmentId);
            SelectedStaff = SortedStaff.FirstOrDefault(x => x.Id == linen?.StaffId);
        }

        private void FindTag()
        {
            SearchingTag = SelectedTag.Item2;
            SearchTag();
        }

        private void UseTag()
        {
            if(SelectedLinen ==null || SelectedTag == null) return;

            if (Linens.Any(x => x.Tag == SelectedTag.Item2))
            {
                var existLinen = Linens.FirstOrDefault(x => Equals(x.Tag, SelectedTag.Item2));
                var staff = Staffs.FirstOrDefault(x => x.Id == existLinen.StaffId);

                if (!_dialogService.ShowQuestionDialog(
                    $"Tag {SelectedTag.Item2} already using by {staff?.Name} in <{existLinen.Id}> linen \n Do you want to shift Tag?")
                ) return;

                existLinen.Tag = null;
                SelectedLinen.Tag = SelectedTag.Item2;
                return;
            }

            if (!String.IsNullOrWhiteSpace(SelectedLinen.Tag))
            {
                if (_dialogService.ShowQuestionDialog($"Selected linen has tag, \n Do you want to replace it?"))
                {
                    SelectedLinen.Tag = SelectedTag.Item2;
                    return;
                }
            }

        }
    }
}
