using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    public class BinClientViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly ILaundryService _laundryService;
        private readonly IResolver _resolverService;
        private readonly IMainDispatcher _dispatcher;

        private ObservableCollection<ClientEntityViewModel> _clients;
        private ObservableCollection<DepartmentEntityViewModel> _departments;
        private ObservableCollection<ClientStaffEntityViewModel> _staffs;
        private ObservableCollection<MasterLinenEntityViewModel> _masterLinens;
        private ObservableCollection<ClientLinenEntityViewModel> _linens;
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
        public ObservableCollection<ClientStaffEntityViewModel> Staffs
        {
            get => _staffs;
            set => Set(() => Staffs, ref _staffs, value);
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
        public BinRfidReaderViewModel RfidReader { get; set; }


        public ObservableCollection<ClientStaffEntityViewModel> StaffsLoaded => GetLoadedStaffs();
        public ObservableCollection<ClientLinenEntityViewModel> LinensLoaded => GetLoadedLinens();

        public DataViewModel DataViewModel { get; set; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand InitializeCommand { get; }


        public BinClientViewModel(ILaundryService dataService, IDialogService dialogService, IResolver resolver, IMainDispatcher dispatcher)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            SaveCommand = new RelayCommand(Save);
            InitializeCommand = new RelayCommand(Initialize);

            PropertyChanged += OnPropertyChanged;

            RfidReader = _resolverService.Resolve<BinRfidReaderViewModel>();

            RfidReader.Tags.CollectionChanged += TagsCollectionChanged;
        }


        #region Data
        private void Initialize()
        {
            GetClients();
            GetDepartments();
            GetClientStaffs();
            GetMasterLinens();
            GetLinens();
        }

        public async void GetClients()
        {
            var client = await _laundryService.GetAllAsync<ClientEntity>();
            var clients = client.Select(x => new ClientEntityViewModel(x));
            Clients = clients.ToObservableCollection();
        }
        public async void GetDepartments()
        {
            var department = await _laundryService.GetAllAsync<DepartmentEntity>();
            var departments = department.Select(x => new DepartmentEntityViewModel(x));
            Departments = departments.ToObservableCollection();
        }

        public async void GetMasterLinens()
        {
            var masterLinen = await _laundryService.GetAllAsync<MasterLinenEntity>();
            var masterLinens = masterLinen.Select(x => new MasterLinenEntityViewModel(x));
            MasterLinens = masterLinens.ToObservableCollection();
        }

        public async void GetClientStaffs()
        {
            var staff = await _laundryService.GetAllAsync<ClientStaffEntity>();
            var staffs = staff.Select(x => new ClientStaffEntityViewModel(x));
            Staffs = staffs.ToObservableCollection();
        }

        public async void GetLinens()
        {
            var linen = await _laundryService.GetAllAsync<ClientLinenEntity>();
            var linens = linen.Select(x => new ClientLinenEntityViewModel(x));
            Linens = linens.ToObservableCollection();
        }
        #endregion


        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(SelectedClient))
            //{
            //}
        }
        
        private void TagsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var tag in RfidReader.Tags)
                {

                }
            }
        }

        private ObservableCollection<ClientStaffEntityViewModel> GetLoadedStaffs()
        {
            var items = new ObservableCollection<ClientStaffEntityViewModel>();

            if (Staffs == null) return items;

            items.AddRange(Staffs);

            return items;
        }        
        
        private ObservableCollection<ClientLinenEntityViewModel> GetLoadedLinens()
        {
            var items = new ObservableCollection<ClientLinenEntityViewModel>();

            if (Staffs == null) return items;

            items.AddRange(Linens);

            return items;
        }

        private async void Save()
        {

        }

    }
}
