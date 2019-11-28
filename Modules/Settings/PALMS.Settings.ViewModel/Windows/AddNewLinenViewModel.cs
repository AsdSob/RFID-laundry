using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Settings.ViewModel.Common;
using PALMS.Settings.ViewModel.EntityViewModels;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;

namespace PALMS.Settings.ViewModel.Windows
{
    public class AddNewLinenViewModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IDataService _dataService;
        private readonly IDispatcher _dispatcher;
        public Action<bool> CloseAction { get; set; }
        private bool IsSelected { get; set; }
        private ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>> _data =
            new ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>>();

        private List<string> _tags;
        private string _selectedTag;
        private RfidCommon _impinj;
        private Task _runningTask;
        private ObservableCollection<ClientEntityViewModel> _clients;
        private ObservableCollection<DepartmentEntityViewModel> _departments;
        private ObservableCollection<ClientStaffEntityViewModel> _staff;
        private ObservableCollection<MasterLinenEntityViewModel> _masterLinens;
        private ObservableCollection<ClientLinenEntityViewModel> _clientLinens;
        private ClientLinenEntityViewModel _selectedLinen;
        private DepartmentEntityViewModel _selectedDepartment;
        private ClientStaffEntityViewModel _selectedStaff;
        private ClientEntityViewModel _selectedClient;
        private List<ClientLinenEntityViewModel> _newLinens;

        public List<ClientLinenEntityViewModel> NewLinens
        {
            get => _newLinens;
            set => Set(() => NewLinens, ref _newLinens, value);
        }
        public ClientLinenEntityViewModel SelectedLinen
        {
            get => _selectedLinen;
            set => Set(() => SelectedLinen, ref _selectedLinen, value);
        }
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
        public ObservableCollection<ClientLinenEntityViewModel> ClientLinens
        {
            get => _clientLinens;
            set => Set(() => ClientLinens, ref _clientLinens, value);
        }
        public ObservableCollection<MasterLinenEntityViewModel> MasterLinens
        {
            get => _masterLinens;
            set => Set(() => MasterLinens, ref _masterLinens, value);
        }
        public ObservableCollection<ClientStaffEntityViewModel> Staff
        {
            get => _staff;
            set => Set(() => Staff, ref _staff, value);
        }
        public ObservableCollection<DepartmentEntityViewModel> Departments
        {
            get => _departments;
            set => Set(() => Departments, ref _departments, value);
        }
        public ObservableCollection<ClientEntityViewModel> Clients
        {
            get => _clients;
            set => Set(() => Clients, ref _clients, value);
        }
        public Task RunningTask
        {
            get => _runningTask;
            set => Set(() => RunningTask, ref _runningTask, value);
        }
        public RfidCommon Impinj
        {
            get => _impinj;
            set => Set(() => Impinj, ref _impinj, value);
        }
        public string SelectedTag
        {
            get => _selectedTag;
            set => Set(() => SelectedTag, ref _selectedTag, value);
        }
        public List<string> Tags
        {
            get => _tags;
            set => Set(() => Tags, ref _tags, value);
        }
        
        public ObservableCollection<DepartmentEntityViewModel> SortedDepartments => Departments?.Where(x=> x.ClientId == SelectedClient?.Id).ToObservableCollection();
        public ObservableCollection<ClientStaffEntityViewModel> SortedStaff => Staff?.Where(x=> x.DepartmentId == SelectedDepartment?.Id).ToObservableCollection();
        public ObservableCollection<ClientLinenEntityViewModel> SortedLinens => SortLinen();

        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand AddTagCommand { get; }
        public RelayCommand StartReadCommand { get; }
        public RelayCommand StopReadCommand { get; }
        public RelayCommand AddLinenCommand { get; }
        public RelayCommand RemoveCommand { get; }
        public RelayCommand<object> ShowAntennaTagCommand { get; }


        public async Task InitializeAsync()
        {
            IsSelected = false;

            Clients = new ObservableCollection<ClientEntityViewModel>();
            Departments = new ObservableCollection<DepartmentEntityViewModel>();
            Staff = new ObservableCollection<ClientStaffEntityViewModel>();

            var client = await _dataService.GetAsync<Client>();
            var clients = client.Select(x => new ClientEntityViewModel(x));
            _dispatcher.RunInMainThread(() => Clients = clients.ToObservableCollection());

            var department = await _dataService.GetAsync<Department>();
            var departments = department.Select(x => new DepartmentEntityViewModel(x));
            _dispatcher.RunInMainThread(() => Departments = departments.ToObservableCollection());

            var master = await _dataService.GetAsync<MasterLinen>();
            var masters = master.Select(x => new MasterLinenEntityViewModel(x));
            _dispatcher.RunInMainThread(() => MasterLinens = masters.ToObservableCollection());

            var staff = await _dataService.GetAsync<ClientStaff>();
            var staffs = staff.Select(x => new ClientStaffEntityViewModel(x));
            _dispatcher.RunInMainThread(() => Staff = staffs.ToObservableCollection());

            var linen = await _dataService.GetAsync<ClientLinen>();
            var linens = linen.Select(x => new ClientLinenEntityViewModel(x));
            _dispatcher.RunInMainThread(() => ClientLinens = linens.ToObservableCollection());
        }

        public AddNewLinenViewModel(IDialogService dialogService, IDataService dataService, IDispatcher dispatcher)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);
            RemoveCommand = new RelayCommand(Remove);
            AddLinenCommand = new RelayCommand(AddClientLinen, (() => SelectedDepartment != null));

            StartReadCommand = new RelayCommand(StartRead);
            StopReadCommand = new RelayCommand(StopRead);
            AddTagCommand = new RelayCommand(AddTag);
            ShowAntennaTagCommand = new RelayCommand<object>(SHowAntennaTags);

            NewLinens = new List<ClientLinenEntityViewModel>();
            IsSelected = false;
            Impinj = new RfidCommon();

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
                RaisePropertyChanged(() => SortedLinens);

                AddLinenCommand.RaiseCanExecuteChanged();

            }

            if (e.PropertyName == nameof(SelectedStaff))
            {
                RaisePropertyChanged(() => SortedLinens);
            }
        }

        private ObservableCollection<ClientLinenEntityViewModel> SortLinen()
        {
            var linen = new ObservableCollection<ClientLinenEntityViewModel>();

            if (SelectedStaff == null)
            {
                linen = ClientLinens?.Where(x => x.DepartmentId == SelectedDepartment?.Id).ToObservableCollection();

            }
            else
            {
                linen = ClientLinens?.Where(x => x.StaffId == SelectedStaff?.Id).ToObservableCollection();
            }

            return linen;
        }


        private void AddClientLinen()
        {
            var newLinen = new ClientLinenEntityViewModel()
            {
                DepartmentId = SelectedDepartment.Id,
                StatusId = 1,
            };

            if (SelectedStaff != null) newLinen.StaffId = SelectedStaff.Id;

            ClientLinens.Add(newLinen);

            RaisePropertyChanged(() => SortedLinens);
        }

        public void SHowAntennaTags(object antennaNumb)
        {
            var antenna = int.Parse(antennaNumb.ToString());

            Tags = _data[antenna].Keys.ToList();
        }

        private void StartRead()
        {
            Task.Factory.StartNew(Impinj.StartRead);
        }

        private void StopRead()
        {
            _data = Impinj.StopRead();
        }

        private void AddTag()
        {
            if(SelectedLinen == null)return;
            if(SelectedTag == null) return;

            SelectedLinen.Tag = SelectedTag;
        }


        public void Save()
        {
            if (SortedLinens.Any(x => x.HasChanges()))
            {
                if (_dialogService.ShowQuestionDialog("Do you want to save changes?"))
                {
                    var linens = SortedLinens.Where(x => x.HasChanges());

                    foreach (var linen in linens)
                    {
                        linen.PackingValue =
                            MasterLinens.FirstOrDefault(x => x.Id == linen.MasterLinenId).PackingValue;

                        linen.AcceptChanges();
                    }

                    _dataService.AddOrUpdateAsync(linens.Select(x => x.OriginalObject));

                    NewLinens = linens.ToList();

                    if (_dialogService.ShowQuestionDialog("All saves don \n Do you want to close window?"))
                    {
                        CloseAction?.Invoke(true);
                    }
                }

            }

        }

        private void Remove()
        {
            if(SelectedLinen == null) return;
            ClientLinens.Remove(SelectedLinen);

            RaisePropertyChanged(()=> SortedLinens);
        }

        public void Close()
        {
            if (_dialogService.ShowQuestionDialog("Do you want to Save changes and close window ? "))
            {
                CloseAction?.Invoke(IsSelected);
            }
        }

        public List<ClientLinenEntityViewModel> GetNewClient()
        {
            return NewLinens;
        }
    }
}
