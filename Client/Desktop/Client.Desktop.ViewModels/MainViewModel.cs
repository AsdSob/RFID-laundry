using System;
using System.ComponentModel;
using System.Windows.Input;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Content;
using Client.Desktop.ViewModels.Windows;

namespace Client.Desktop.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IResolver _resolver;
        private readonly IDialogService _dialogService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private object _content;
        private MenuViewModel _menuViewModel;
        private bool _menuIsVisible;

        public Action CloseAction { get; set; }

        public ICommand InitilizeCommand { get; }

        public object Content
        {
            get => _content;
            set { Set(() => Content, ref _content, value); }
        }

        public MenuViewModel MenuViewModel
        {
            get => _menuViewModel;
            set => Set(() => MenuViewModel, ref _menuViewModel, value);
        }

        public bool MenuIsVisible
        {
            get => _menuIsVisible;
            set => Set(() => MenuIsVisible, ref _menuIsVisible, value);
        }

        public RelayCommand LogoutCommand { get; }

        public MainViewModel(MenuViewModel menuViewModel,
            IResolver resolver,
            IDialogService dialogService,
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));

            MenuViewModel = menuViewModel ?? throw new ArgumentNullException(nameof(menuViewModel));

            MenuViewModel.PropertyChanged += MenuViewModelOnPropertyChanged;

            InitilizeCommand = new RelayCommand(Initialize);
            LogoutCommand = new RelayCommand(Logout);
        }

        private void Initialize()
        {
            if (_dialogService.ShowDialog(_resolver.Resolve<LoginWindowViewModel>()) == false)
            {
                return;
            }

            MenuIsVisible = true;
            SetMenuContent(MenuViewModel);
        }

        private async void Logout()
        {
            Content = null;
            MenuIsVisible = false;

            await _authorizationService.LogoutAsync();
            await _authenticationService.LogoutAsync();

            InitilizeCommand.Execute(null);
        }

        private void MenuViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var menuViewModel = sender as MenuViewModel;
            if (menuViewModel == null) return;

            if (e.PropertyName == nameof(MenuViewModel.SelectedItem))
            {
                SetMenuContent(menuViewModel);
            }
        }

        private void SetMenuContent(MenuViewModel menuViewModel)
        {
            if (menuViewModel.SelectedItem == typeof(ExitViewModel))
            {
                if (_dialogService.ShowQuestionDialog("Close the application?") == false) return;

                CloseAction?.Invoke();
                return;
            }

            var type = _resolver.Resolve(menuViewModel.SelectedItem);

            Content = type;
        }
    }
}
