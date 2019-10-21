using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;
using PALMS.ViewModels.Common.Enumerations;

namespace PALMS.ViewModels.WindowViewModel
{
    public class LoginViewModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly IAuthService _authService;
        private string _login;
        private string _password;
        private string _errorText;
        private string _errorToolTip;
        private bool _isBusy;
        private ICollection<RoleEnum> _roles;

        public string Login
        {
            get => _login;
            set => Set(ref _login, value);
        }

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public string ErrorText
        {
            get => _errorText;
            set => Set(ref _errorText, value);
        }

        public string ErrorToolTip
        {
            get => _errorToolTip;
            set => Set(ref _errorToolTip, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        public ICollection<RoleEnum> Roles => _roles;

        public Action<bool> CloseAction { get; set; }

        public RelayCommand LoginCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            LoginCommand = new RelayCommand(LoginExecute, CanLoginExecute);
            CancelCommand = new RelayCommand(() => CloseAction?.Invoke(false));

            PropertyChanged += OnPropertyChanged;

            Login = "pos";
            Password = "Operator@123";

            Login = "admin";
            Password = "Admin";
        }

        private async void LoginExecute()
        {
            await LoginExecuteAsync();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Login) || e.PropertyName == nameof(Password) ||
                e.PropertyName == nameof(IsBusy))
            {
                Helper.RunInMainThread(() =>
                {
                    LoginCommand.RaiseCanExecuteChanged();
                });
            }
        }

        private async Task LoginExecuteAsync()
        {
            IsBusy = true;

            var response = await _authService.Login(new LoginRequest
            {
                UserName = Login,
                Password = Password
            })
            .ContinueWith(x =>
                {
                    IsBusy = false;
                    return x.Result;
                });

            ErrorText = response.ErrorText;
            ErrorToolTip = $"{response.StatusCode}: {response.Description}";
            _roles = response.AuthResponse?.Roles ?? new List<RoleEnum>();

            if (!response.IsUserExcist) return;

            CloseAction(true);
        }

        private bool CanLoginExecute()
        {
            return !string.IsNullOrEmpty(Login) &&
                !string.IsNullOrEmpty(Password) &&
                !IsBusy;
        }
    }
}
