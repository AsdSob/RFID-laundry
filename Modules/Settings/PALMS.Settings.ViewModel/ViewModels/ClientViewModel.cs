using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using PALMS.Settings.ViewModel.EntityViewModels;
using PALMS.ViewModels.Common.Services;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace PALMS.Settings.ViewModel.ViewModels
{
    public class ClientViewModel : ViewModelBase
    {
        private readonly IDispatcher _dispatcher;
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<ClientEntityViewModel> _clients;
        private ClientEntityViewModel _selectedClient;

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

        public RelayCommand SendToBelt2Command { get; }


        public async Task InitializeAsync()
        {

        }

        public ClientViewModel(IDispatcher dispatcher, IDataService dataService, IDialogService dialogService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SendToBelt2Command = new RelayCommand(ManualSendToBelt2);

            AddBeltItems();

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {

            }

        }


    }
}
