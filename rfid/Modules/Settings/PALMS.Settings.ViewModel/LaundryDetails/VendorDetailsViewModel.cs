using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using PALMS.Data.Objects.ClientModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Settings.ViewModel.LaundryDetails
{
    public class VendorDetailsViewModel : ViewModelBase, ISettingsContent, IInitializationAsync
    {
        public string Error { get; }
        public string Name => "PrimeInfo";
        private readonly IDispatcher _dispatcher;
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private BitmapImage _vendorLogo;

        public BitmapImage VendorLogo
        {
            get => _vendorLogo;
            set => Set(ref _vendorLogo, value);
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand PictureAddCommand { get; }

        public bool HasChanges()
        {
            return PrimeInfoChanges();
        }

        public async Task InitializeAsync()
        {
            if (0 == 0)
            {
               
                return;
            }
            
        }

        public void Cancel()
        {

        }

        public async void Save()
        {
            if(!PrimeInfoChanges()) return;

            if (!_dialogService.ShowQuestionDialog(" Do you want to Save all changes ?"))
                return;
        }

        public VendorDetailsViewModel(IDispatcher dispatcher, IDataService dataService, IDialogService dialogService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SaveCommand = new RelayCommand(Save); 
            CancelCommand = new RelayCommand(Cancel);
            PictureAddCommand = new RelayCommand(AddLogo);

            PropertyChanged += OnPropertyChanged;

        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        public bool PrimeInfoChanges()
        {
            return false;
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

            }
        }


    }
}
