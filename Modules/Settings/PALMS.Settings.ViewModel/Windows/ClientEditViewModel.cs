using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Settings.ViewModel.EntityViewModels;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;

namespace PALMS.Settings.ViewModel.Windows
{
    public class ClientEditViewModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly IDispatcher _dispatcher;
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private ObservableCollection<ClientEntityViewModel> _clients;
        private ClientEntityViewModel _selectedClient;
        private List<UnitViewModel> _cities;

        public List<UnitViewModel> Cities
        {
            get => _cities;
            set => Set(() => Cities, ref _cities, value);
        }
        public Action<bool> CloseAction { get; set; }

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

        public RelayCommand CloseCommand { get; }
        public RelayCommand AddClientCommand { get; }
        public RelayCommand SaveCommand { get; }

        public async Task InitializeAsync()
        {
            var client = await _dataService.GetAsync<Client>();
            var clients = client.Select(x => new ClientEntityViewModel(x));
            _dispatcher.RunInMainThread(() => Clients = clients.ToObservableCollection());

            Cities = EnumExtentions.GetValues<Cities>();

        }

        public ClientEditViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            CloseCommand = new RelayCommand(Close);
            AddClientCommand = new RelayCommand(AddClient);
            SaveCommand = new RelayCommand(Save);

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(()=> SortedLocations);
            }
        }

        private ObservableCollection<ClientEntityViewModel> SortClients(int? clientId)
        {
            var clients = new ObservableCollection<ClientEntityViewModel>();

            if (clientId != null)
            {
                clients = Clients.Where(x => x.ParentId == clientId).ToObservableCollection();
            }
            else
            {
                clients = Clients.Where(x => x.ParentId == null).ToObservableCollection();
            }

            return clients;
        }


        private void AddClient()
        {

        }

        private void Save()
        {

        }

        public void Close()
        {
            if (!_dialogService.ShowQuestionDialog("Do you want to close window ?"))
                return;

            CloseAction?.Invoke(false);
        }
    }
}
