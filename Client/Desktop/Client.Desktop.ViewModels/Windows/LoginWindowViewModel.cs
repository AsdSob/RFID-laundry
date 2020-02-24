using System;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.Identity;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Common.Windows;

namespace Client.Desktop.ViewModels.Windows
{
    public class LoginWindowViewModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMainDispatcher _mainDispatcher;
        private readonly IAuthorizationService _authorizationService;
        private string _username;
        private string _status;

        public Action<bool> CloseAction { get; set; }


        public LoginWindowViewModel(IAuthenticationService authenticationService, IMainDispatcher mainDispatcher, IAuthorizationService authorizationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _mainDispatcher = mainDispatcher ?? throw new ArgumentNullException(nameof(mainDispatcher));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            
            LoginCommand = new RelayCommand<object>(Login, CanLogin);
            LogoutCommand = new RelayCommand(Logout, CanLogout);
        }

        #region Properties
        public string Username
        {
            get { return _username; }
            set { Set(() => Username, ref _username, value); }
        }

        public string AuthenticatedUser
        {
            get
            {
                if (IsAuthenticated)
                    return $"Signed in as {Thread.CurrentPrincipal.Identity.Name}";

                return "Not authenticated!";
            }
        }

        public string Status
        {
            get => _status;
            set => Set(() => Status, ref _status, value);
        }
        #endregion

        public RelayCommand<object> LoginCommand { get; }

        public RelayCommand LogoutCommand { get; }

        private void Login(dynamic parameter)
        {
            string clearTextPassword = parameter?.Password;
            try
            {
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(clearTextPassword))
                    throw new UnauthorizedAccessException();

                //Validate credentials through the authentication service
                User user = _authenticationService.AuthenticateUser(Username, clearTextPassword);

                //Get the current principal object
                CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
                if (customPrincipal == null)
                    throw new ArgumentException("The application's default thread principal must be set to a CustomPrincipal object on startup.");

                //Authenticate the user
                customPrincipal.Identity = new CustomIdentity(user.Username, user.Email, user.Roles);

                //Update UI
                RaisePropertyChanged(() => AuthenticatedUser);
                RaisePropertyChanged(() => IsAuthenticated);
                LoginCommand.RaiseCanExecuteChanged();
                LogoutCommand.RaiseCanExecuteChanged();
                //Username = string.Empty; //reset
                //passwordBox.Password = string.Empty; //reset
                Status = string.Empty;

                _authorizationService.CurrentPrincipal = customPrincipal;

                RaisePropertyChanged(() => AuthenticatedUser);

                Task.Delay(2000).ContinueWith(x =>
                {
                    _mainDispatcher.RunInMainThread(() => CloseAction?.Invoke(true));
                });
            }
            catch (UnauthorizedAccessException)
            {
                Status = "Login failed! Please provide some valid credentials.";
            }
            catch (Exception ex)
            {
                Status = $"ERROR: {ex.Message}";
            }
        }

        private bool CanLogin(object password)
        {
            return !IsAuthenticated;
        }

        private void Logout()
        {
            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
            if (customPrincipal != null)
            {
                customPrincipal.Identity = new AnonymousIdentity();
                RaisePropertyChanged(() => AuthenticatedUser);
                RaisePropertyChanged(() => IsAuthenticated);
                LoginCommand.RaiseCanExecuteChanged();
                LogoutCommand.RaiseCanExecuteChanged();
                Status = string.Empty;
            }
        }

        private bool CanLogout()
        {
            return IsAuthenticated;
        }

        public bool IsAuthenticated => Thread.CurrentPrincipal.Identity.IsAuthenticated;
    }
}
