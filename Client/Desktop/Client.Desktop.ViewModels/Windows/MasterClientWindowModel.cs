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
        private ObservableCollection<ClientEntityViewModel> _clients;
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
        public ObservableCollection<ClientEntityViewModel> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }

        public List<ClientEntityViewModel> SortedParentClients => SortParentClients();

        private List<ClientEntityViewModel> SortParentClients()
        {
            return Clients?.Where(x => (x.ParentId == 0 || x.ParentId == null) && x.Id != SelectedClient?.Id).ToList();
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand NewCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand InitializeCommand { get; }

        public Action<bool> CloseAction { get; set; }


        public MasterClientWindowModel(ILaundryService laundryService, IDialogService dialogService, IMainDispatcher dispatcher)
        {
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(CloseWindow);
            DeleteCommand = new RelayCommand(Delete);
            NewCommand = new RelayCommand(NewItem);
            //InitializeCommand = new RelayCommand(Initialize);

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

            NewItem();
        }

        public void NewItem()
        {
            SelectedClient = new ClientEntityViewModel()
            {
                CityId = 1,
                Active = true
            };
        }

        private async void Initialize()
        {
            _dialogService.ShowBusy();

            try
            {
                var client = await _laundryService.GetAllAsync<ClientEntity>();
                var clients = client.Select(x => new ClientEntityViewModel(x));
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

        }

        private void Save()
        {
            if (!SelectedClient.IsValid || !SelectedClient.HasChanges())
            {
                return;
            }

            SelectedClient.AcceptChanges();

            _laundryService.AddOrUpdateAsync(SelectedClient.OriginalObject);

            if (_dialogService.ShowQuestionDialog("Saved! \n Do you want to close window ? "))
            {
                CloseWindow();
            }
        }

        private void Delete()
        {
            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {SelectedClient.Name} ?"))
                return;

            if (!SelectedClient.IsNew)
            {
                _laundryService.DeleteAsync(SelectedClient.OriginalObject);
            }

            CloseWindow();
        }

        private bool CanExecuteParentIdClearCommand()
        {
            return true;
        }

        private void CloseWindow()
        {
            if (SelectedClient.HasChanges())
            {
                if (_dialogService.ShowQuestionDialog($"Do you want to close window ? \n \"Changes is NOT saved\""))
                {
                    CloseAction?.Invoke(false);
                    return;
                }
            }

            CloseAction?.Invoke(true);
        }

    }

}
