using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Common.Windows;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Windows
{
    public class MasterClientWindowModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private readonly IMainDispatcher _dispatcher;
        private ObservableCollection<ClientEntity> _clients;
        private ClientEntityViewModel _selectedClient;
        private List<UnitViewModel> _cities;

        public List<UnitViewModel> Cities
        {
            get => _cities;
            set => Set(ref _cities, value);
        }
        public ClientEntityViewModel SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public ObservableCollection<ClientEntity> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }

        public ObservableCollection<ClientEntity> SortedParentClients =>
            Clients?.Where(x => (x.ParentId == 0 || x.ParentId == null) && x.Id != SelectedClient?.Id).ToObservableCollection();


        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand ClearParentIdCommand { get; }
        public RelayCommand InitializeCommand { get; }

        public Action<bool> CloseAction { get; set; }

        public MasterClientWindowModel(ILaundryService laundryService, IDialogService dialogService, IMainDispatcher dispatcher)
        {
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);
            ClearParentIdCommand = new RelayCommand(ClearParentId);
            InitializeCommand = new RelayCommand(Initialize);

            Cities = EnumExtensions.GetValues<CitiesEnum>();
        }

        public void SetSelectedClient(ClientEntityViewModel client)
        {
            SelectedClient = null;

            if (client != null)
            {
                SelectedClient = client;
                return;
            }

            SelectedClient = new ClientEntityViewModel(new ClientEntity()
            {
                Active = true,
                CityId = (int)CitiesEnum.AbuDhabi,
            });
        }

        private async void Initialize()
        {
            _dialogService.ShowBusy();

            try
            {
                var clients = await _laundryService.GetAllAsync<ClientEntity>();
                Clients = clients.ToObservableCollection();

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
            RaisePropertyChanged((() => SortedParentClients));
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private void Save()
        {
            if (!SelectedClient.HasChanges() || !SelectedClient.IsValid)
            {
                return;
            }

            SelectedClient.AcceptChanges();

            _laundryService.AddOrUpdateAsync(SelectedClient.OriginalObject);

            if (_dialogService.ShowQuestionDialog("Saved! \n Do you want to close window ? "))
            {
                CloseAction?.Invoke(true);
            }
        }

        private void ClearParentId()
        {
            SelectedClient.ParentId = null;
        }

        private bool CanExecuteParentIdClearCommand()
        {
            return SelectedClient.ParentId.HasValue;
        }

        private void Close()
        {
            if (_dialogService.ShowQuestionDialog($"Do you want to close window ? \n \"All changes will be canceled\""))
            {
                CloseAction?.Invoke(false);
            }
        }


    }
}
