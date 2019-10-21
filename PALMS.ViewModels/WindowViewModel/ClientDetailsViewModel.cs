using System;
using System.IO;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;
using PALMS.ViewModels.EntityViewModel;


namespace PALMS.ViewModels
{
    public class ClientDetailsViewModel : ViewModelBase, IWindowDialogViewModel, IInitializationAsync
    {
        private readonly IDialogService _dialogService;
        private ClientEditViewModel _editClient;
        private string _iconPath;
        private bool _isBusy;
        private string _saveText;

        public string SaveText
        {
            get => _saveText;
            set => Set(ref _saveText, value);
        }
        public string IconPath
        {
            get => _iconPath;
            set => Set(ref _iconPath, value);
        }
        public ClientEditViewModel EditClient
        {
            get => _editClient;
            set => Set(ref _editClient, value);
        }
        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        public Action<bool> CloseAction { get; set; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand ExitCommand { get; }
        public RelayCommand PictureAddCommand { get; }

        public ClientDetailsViewModel(IDialogService dialogService, IDataService dataService)
        {
            _dialogService = dialogService;

            SaveText = "Create new client";
            IconPath = "/PALMS.WPFClient;component/Icons/add_32.png";

            SaveCommand = new RelayCommand(Save);
            ExitCommand = new RelayCommand(() => Close());
            EditClient = new ClientEditViewModel(dataService);
            PictureAddCommand=new RelayCommand(AddLogo);
        }
        public void SetClient(ClientViewModel client)
        {
            EditClient.SetClient(client);

            SaveText = "Edit client";
            IconPath = "/PALMS.View.Resources;component/Icons/edit_32.png"; // TODO: use service for rosolving images pathes
        }

        public ClientViewModel GetClient()
        {
            return EditClient.Client;
        }

        public async Task InitializeAsync()
        {
            IsBusy = true;

            await EditClient.InitializeAsync();

            IsBusy = false;
        }

        protected virtual void Close(bool dialogResult = false)
        {
            CloseAction?.Invoke(dialogResult);
        }

        private async void Save()
        {
            IsBusy = true;

            try
            {
                await EditClient.SaveAsync();

                IsBusy = false;

                CloseAction?.Invoke(true);
            }
            catch (Exception e)
            {
                IsBusy = false;

                _dialogService.ShowErrorDialog(e.Message);
            }
        }

        private void AddLogo()
        {
            OpenFileDialog op = new OpenFileDialog();

            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                        "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                        "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                GetClient().ClientInfo.Logo = Extension.GetBitmapImage(File.ReadAllBytes(op.FileName));
                GetClient().ClientInfo.SetLogoBytes(File.ReadAllBytes(op.FileName));
            }
        }

    }
}