using System;
using System.Threading;
using System.Windows.Input;
using Client.Desktop.ViewModels.Common.Identity;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Content;
using Client.Desktop.ViewModels.Content.Administration;
using Client.Desktop.ViewModels.Content.Master;

namespace Client.Desktop.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        private readonly IAuthorizationService _authorizationService;
        private Type _selectedItem;

        public Type SelectedItem
        {
            get => _selectedItem;
            set { Set(() => SelectedItem, ref _selectedItem, value); }
        }

        public ICommand NewCommand { get; }
        public ICommand ExitCommand { get; }

        public ICommand ClientCommand { get; }
        public ICommand StaffCommand { get; }
        public ICommand MasterLinenCommand { get; }
        public ICommand AuthManageCommand { get; }

        public MenuViewModel(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            
            NewCommand = new RelayCommand(() => Select(typeof(DataViewModel)));
            ExitCommand = new RelayCommand(() => Select(typeof(ExitViewModel)));
            ClientCommand = new RelayCommand(() => Select(typeof(MasterClientViewModel)));
            StaffCommand = new RelayCommand(() => Select(typeof(MasterStaffViewModel)), StaffCommandCanExecute);
            MasterLinenCommand = new RelayCommand(() => Select(typeof(MasterLinenViewModel)));
            AuthManageCommand = new RelayCommand(() => Select(typeof(AuthManageViewModel)), AuthManageCommandCanExecute);

            _selectedItem = typeof(MasterClientViewModel);
        }

        private bool AuthManageCommandCanExecute()
        {
            return _authorizationService.CurrentPrincipal?.IsInRole(Roles.Administrator) == true;
        }

        private bool StaffCommandCanExecute()
        {
            return _authorizationService.CurrentPrincipal?.IsInRole(Roles.Administrator) == true;
        }

        private void Select(Type type)
        {
            // TODO: use enum?
            SelectedItem = type;
        }
    }
}