using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
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
        private PrimeInfoViewModel _primeInfo;
        private BitmapImage _vendorLogo;

        public BitmapImage VendorLogo
        {
            get => _vendorLogo;
            set => Set(ref _vendorLogo, value);
        }
        public PrimeInfoViewModel PrimeInfo
        {
            get => _primeInfo;
            set => Set(ref _primeInfo, value);
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
            var primeInfo =  await _dataService.GetAsync<PrimeInfo>();
            if (primeInfo.Count == 0)
            {
                _dispatcher.RunInMainThread(() => PrimeInfo = new PrimeInfoViewModel());
                return;
            }
            _dispatcher.RunInMainThread(() => PrimeInfo = new PrimeInfoViewModel(primeInfo?.FirstOrDefault()));
        }

        public void Cancel()
        {
            PrimeInfo.Reset();
        }

        public async void Save()
        {
            if(!PrimeInfoChanges()) return;

            if (!_dialogService.ShowQuestionDialog(" Do you want to Save all changes ?"))
                return;
            
            PrimeInfo.AcceptChanges();
            await _dataService.AddOrUpdateAsync(PrimeInfo.OriginalObject);
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
            if (e.PropertyName == nameof(PrimeInfo))
            {
                SaveCommand.RaiseCanExecuteChanged();
                CancelCommand.RaiseCanExecuteChanged();
            }
        }

        public bool PrimeInfoChanges()
        {
            return PrimeInfo.HasChanges();
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
                PrimeInfo.Logo = Extension.GetBitmapImage(File.ReadAllBytes(op.FileName));
                PrimeInfo.SetLogoBytes(File.ReadAllBytes(op.FileName));
            }
        }


    }
}
