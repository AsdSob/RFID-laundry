using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Settings.ViewModel.AppSettings
{
    public class AppSettingsViewModel : ViewModelBase, ISettingsContent, IInitializationAsync
    {
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private readonly ILogger _logger;
        private readonly IAppSettingsProvider _appSettingsProvider;
        private bool _isBusy;

        public string Name => "Settings";

        public AppSettingsDataViewModel Settings { get; }

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        public RelayCommand SaveCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public RelayCommand CheckCommand { get; set; }

        public AppSettingsViewModel(IAppSettingsProvider appSettingsProvider, ILogger logger, IDialogService dialogService, IDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettingsProvider = appSettingsProvider ?? throw new ArgumentNullException(nameof(appSettingsProvider));

            Settings = new AppSettingsDataViewModel();

            Settings.PropertyChanged += SettingsOnPropertyChanged;
            PropertyChanged += OnPropertyChanged;

            SaveCommand = new RelayCommand(Save, () => !IsBusy && HasChanges());
            CancelCommand = new RelayCommand(Cancel, () => !IsBusy && HasChanges());
            CheckCommand = new RelayCommand(Check, () => !IsBusy && HasChanges());
        }

        private async void Check()
        {
            IsBusy = true;

            var validationError = await ValidateConnectionString();

            IsBusy = false;

            if (string.IsNullOrEmpty(validationError))
            {
                _dialogService.ShowInfoDialog("It's OK!");
            }
            else
            {
                _dialogService.ShowWarnigDialog(validationError);
            }
        }

        public bool HasChanges()
        {
            return !Equals(Settings.ConnectionString?.Trim(), _appSettingsProvider.ConnectionString);
        }

        public async Task InitializeAsync()
        {
            await _appSettingsProvider.ReadAsync();

            SetSettings();
        }

        private void SetSettings()
        {
            Settings.ConnectionString = _appSettingsProvider.ConnectionString;
        }

        private async void Save()
        {
            IsBusy = true;

            try
            {
                var isValid = await ValidateAsync();
                if (!isValid)
                {
                    IsBusy = false;
                    _dialogService.ShowWarnigDialog("Settings has erros");
                    return;
                }

                _appSettingsProvider.ConnectionString = Settings.ConnectionString.Trim();

                _appSettingsProvider.Save();

                _dataService.SetState(DatabaseState.Available);

                RaiseCommands();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void Cancel()
        {
            SetSettings();
        }

        private async Task<bool> ValidateAsync()
        {
            if (!Settings.Validate())
                return false;

            var validationError = await ValidateConnectionString();
            return string.IsNullOrEmpty(validationError);
        }

        private async Task<string> ValidateConnectionString()
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    using (var connection = new SqlConnection(Settings.ConnectionString))
                        connection.Open();
                }).ContinueWith(x =>
                {
                    if (x.IsFaulted)
                    {
                        var exception = x.Exception?.Flatten();
                        var message = string.Join("; ", exception?.InnerExceptions.Select(ex => ex.Message) ?? new List<string>().AsReadOnly());
                        throw new Exception($"Not valid connection string: {message}", exception);
                    }
                });

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return ex.Message;
            }

            return null;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsBusy))
                RaiseCommands();
        }

        private void SettingsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseCommands();
        }

        private void RaiseCommands()
        {
            SaveCommand?.RaiseCanExecuteChanged();
            CancelCommand?.RaiseCanExecuteChanged();
            CheckCommand?.RaiseCanExecuteChanged();
        }
    }
}