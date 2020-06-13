using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Content
{
    public class BinClientViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly ILaundryService _laundryService;
        private readonly IResolver _resolverService;
        private readonly IMainDispatcher _dispatcher;

        private List<ClientEntity> _clients;
        private List<DepartmentEntity> _departments;
        private List<ClientStaffEntity> _staffs;
        private List<MasterLinenEntity> _masterLinens;
        private List<ClientLinenEntity> _linens;

        public List<ClientLinenEntity> Linens
        {
            get => _linens;
            set => Set(() => Linens, ref _linens, value);
        }
        public List<MasterLinenEntity> MasterLinens
        {
            get => _masterLinens;
            set => Set(() => MasterLinens, ref _masterLinens, value);
        }
        public List<ClientStaffEntity> Staffs
        {
            get => _staffs;
            set => Set(() => Staffs, ref _staffs, value);
        }
        public List<DepartmentEntity> Departments
        {
            get => _departments;
            set => Set(() => Departments, ref _departments, value);
        }
        public List<ClientEntity> Clients
        {
            get => _clients;
            set => Set(() => Clients, ref _clients, value);
        }


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
        }

        private async void Initialize()
        {
            var client = await _laundryService.GetAllAsync<ClientEntity>();
            Clients = client.ToList();

            var departments = await _laundryService.GetAllAsync<DepartmentEntity>();
            Departments = departments.ToList();

            var staff = await _laundryService.GetAllAsync<ClientStaffEntity>();
            Staffs = staff.ToList();

            var masterLinen = await _laundryService.GetAllAsync<MasterLinenEntity>();
            MasterLinens = masterLinen.ToList();

            var linen = await _laundryService.GetAllAsync<ClientLinenEntity>();
            Linens = linen.ToList();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(SelectedClient))
            //{
            //}
        }

        private async void Save()
        {

        }

    }
}
