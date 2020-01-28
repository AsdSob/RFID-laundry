using System;
using System.ComponentModel;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Content;

namespace Client.Desktop.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IResolver _resolver;
        private object _content;
        private MenuViewModel _menuViewModel;

        public Action CloseAction { get; set; }

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

        public MainViewModel(MenuViewModel menuViewModel, IResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            MenuViewModel = menuViewModel ?? throw new ArgumentNullException(nameof(menuViewModel));

            MenuViewModel.PropertyChanged += MenuViewModelOnPropertyChanged;

            SetMenuContent(MenuViewModel);
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
                // TODO: show question
                CloseAction?.Invoke();
                return;
            }

            var type = _resolver.Resolve(menuViewModel.SelectedItem);

            Content = type;
        }
    }
}
