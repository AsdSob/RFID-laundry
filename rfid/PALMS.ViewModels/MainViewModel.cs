using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly ICanExecuteMediator _canExecuteMediator;
        private readonly IResolver _resolver;
        private object _content;
        private MenuViewModel _menuViewModel;
        private readonly IAuthService _authService;
        private bool _isBusy;
        private Type _previousMenuSelectedItem;
        private ICollection<RoleEnum> _roles;

        public object Content
        {
            get { return _content; }
            set { Set(() => Content, ref _content, value); }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set { Set(() => IsBusy, ref _isBusy, value); }
        }

        public MenuViewModel MenuViewModel
        {
            get { return _menuViewModel; }
            set { Set(() => MenuViewModel, ref _menuViewModel, value); }
        }

        public MainViewModel(IResolver resolverService,
            DataViewModel content,
            MenuViewModel menu,
            ICanExecuteMediator canExecuteMediator,
            IDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _canExecuteMediator = canExecuteMediator ?? throw new ArgumentNullException(nameof(canExecuteMediator));
            _resolver = resolverService ?? throw new ArgumentNullException(nameof(resolverService));

            Content = content;

            _menuViewModel = menu;
        }

        public void Initialize()
        {
            CheckDb();

            InitializeMenu();
        }

        private void InitializeMenu()
        {
            MenuViewModel.Set(_roles);
            RaisePropertyChanged(() => MenuViewModel);

            MenuViewModel.PropertyChanged += MenuPropertyChanged;
        }

        private void MenuPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is MenuViewModel menuViewModel))
                return;

            if (e.PropertyName == nameof(MenuViewModel.SelectedItem))
            {
                if (menuViewModel.SelectedItem == null || Equals(menuViewModel.SelectedItem, _previousMenuSelectedItem))
                    return;


                if (!CanChangeModule())
                {
                    _dialogService.ShowWarnigDialog("You have unsaved data!");

                    var selectPreviousAction = new Func<Task>(() => Task.Factory.StartNew(() =>
                    {
                        Helper.RunInMainThread(() => menuViewModel.SelectedItem = _previousMenuSelectedItem);
                    }));

                    selectPreviousAction();

                    return;
                }

                var contentType = menuViewModel.SelectedItem;

                // TODO: use IResolver
                Content = _resolver.Resolve(contentType);

                _previousMenuSelectedItem = menuViewModel.SelectedItem;
            }
        }

        private bool CanChangeModule()
        {
            if (_canExecuteMediator.CanExecute == null) return true;

            return _canExecuteMediator.CanExecute();
        }

        private static void CheckDb()
        {
            ServiceLocator.Current.GetInstance<IDataService>().LoadAsync();
        }
    }
}
