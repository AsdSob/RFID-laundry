using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Common.Windows;

namespace Client.Desktop.ViewModels.Windows
{
    public class MasterClientWindowModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private ObservableCollection<ClientEntityViewModel> _clients;
        private ClientEntityViewModel _selectedClient;
        private List<UnitViewModel> _cities;
        private ObservableCollection<ClientEntityViewModel> _parentClients;

        public ObservableCollection<ClientEntityViewModel> ParentClients
        {
            get => _parentClients;
            set => Set(ref _parentClients, value);
        }
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

        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand NewCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand InitializeCommand { get; }

        public Action<bool> CloseAction { get; set; }


        public MasterClientWindowModel(ILaundryService laundryService, IDialogService dialogService)
        {
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(CloseWindow);
            DeleteCommand = new RelayCommand(Delete);
            NewCommand = new RelayCommand(New);
            //InitializeCommand = new RelayCommand(Initialize);

            Cities = EnumExtensions.GetValues<CitiesEnum>();
        }

        public async void SetItem(ClientEntityViewModel item)
        {
            await Initialize();

            ParentClients = Clients
                ?.Where(x => (x.ParentId == 0 || x.ParentId == null) && x.Id != item?.Id)
                .ToObservableCollection();

            if (item != null)
            {
                SelectedClient = item;
                return;
            }
            New();
        }

        public void New()
        {
            SelectedClient = new ClientEntityViewModel()
            {
                CityId = 1,
                Active = true,
            };
        }

        public async Task Initialize()
        {
            _dialogService.ShowBusy();

            try
            {
                Clients = await _laundryService.Clients();

            }
            catch (Exception e)
            {
                _dialogService.HideBusy();
            }
            finally
            {
                _dialogService.HideBusy();
            }
        }

        private void Save()
        {
            if (!SelectedClient.IsValid || !SelectedClient.HasChanges())
            {
                return;
            }

            SelectedClient.AcceptChanges();

            _laundryService.AddOrUpdateAsync(SelectedClient.OriginalObject);

            CloseWindow();
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
