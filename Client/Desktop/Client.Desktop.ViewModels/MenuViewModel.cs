using System;
using System.Windows.Input;
using Client.Desktop.ViewModels.Common.Identity;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Content;
using Client.Desktop.ViewModels.Content.Administration;

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
        public ICommand TagRegistrationCommand { get; }
        public ICommand UniversalNoteCommand { get; }

        public ICommand BinSoilCollectionCommand { get; }

        public MenuViewModel(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            
            NewCommand = new RelayCommand(() => Select(typeof(DataViewModel)));
            ExitCommand = new RelayCommand(() => Select(typeof(ExitViewModel)));
            ClientCommand = new RelayCommand(() => Select(typeof(MasterClientViewModel)));
            //StaffCommand = new RelayCommand(() => Select(typeof(MasterStaffViewModel)), StaffCommandCanExecute);
            MasterLinenCommand = new RelayCommand(() => Select(typeof(MasterLinenViewModel)), MasterLinenCommandCanExecute);
            AuthManageCommand = new RelayCommand(() => Select(typeof(AuthManageViewModel)), AuthManageCommandCanExecute);

            TagRegistrationCommand = new RelayCommand(() => Select(typeof(TagRegistrationViewModel)), TagRegistrationCommandCanExecute);

            BinSoilCollectionCommand = new RelayCommand(() => Select(typeof(BinClientViewModel)), BinSoilCollectionCommandCanExecute);
            UniversalNoteCommand = new RelayCommand(() => Select(typeof(UniversalNoteViewModel)), UniversalNoteCommandCanExecute);

            _selectedItem = typeof(UniversalNoteViewModel);
        }

        private bool AuthManageCommandCanExecute()
        {
            if (_authorizationService.CurrentPrincipal?.IsInRole(Roles.Administrator) is true)
                return true;

            if (_authorizationService.CurrentPrincipal?.IsInRole(Roles.Manager) is true)
                return false;

            return false;
        }

        private bool BinSoilCollectionCommandCanExecute()
        {
            return _authorizationService.CurrentPrincipal?.IsInRole(Roles.Administrator) == true;
        }        
        
        private bool UniversalNoteCommandCanExecute()
        {
            return _authorizationService.CurrentPrincipal?.IsInRole(Roles.Administrator) == true;
        }

        private bool TagRegistrationCommandCanExecute()
        {
            return _authorizationService.CurrentPrincipal?.IsInRole(Roles.Administrator) == true;
        }

        private bool StaffCommandCanExecute()
        {
            return _authorizationService.CurrentPrincipal?.IsInRole(Roles.Administrator) == true;
        }
        
        private bool MasterLinenCommandCanExecute()
        {
            //if (_authorizationService.CurrentPrincipal?.IsInRole(Roles.Administrator) is true)
            //    return true;

            //if (_authorizationService.CurrentPrincipal?.IsInRole(Roles.Manager) is true)
            //    return false;

            return true;
        }

        private void Select(Type type)
        {
            SelectedItem = type;
        }
    }
}