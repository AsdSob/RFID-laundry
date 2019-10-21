using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.Audit;
using PALMS.Settings.ViewModel.Users.EntityModels;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Settings.ViewModel.Users
{
    public class UserEditViewModel : ViewModelBase, ISettingsContent, IInitializationAsync
    {
        public string Name => "Users";

        private readonly IDispatcher _dispatcher;
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private readonly IResolver _resolverService;

        private List<UnitViewModel> _userRoles;
        private ObservableCollection<UserModelViewModel> _users;
        private UserModelViewModel _selectedUser;
        private bool _isValid;

        public bool IsValid
        {
            get => _isValid;
            set => Set(ref _isValid, value);
        }
        public UserModelViewModel SelectedUser
        {
            get => _selectedUser;
            set => Set(ref _selectedUser, value);
        }

        public ObservableCollection<UserModelViewModel> Users
        {
            get => _users;
            set => Set(ref _users, value);
        }

        public List<UnitViewModel> UserRoles
        {
            get => _userRoles;
            set => Set(ref _userRoles, value);
        }


        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand AddCommand { get; }

        public UserEditViewModel(IDispatcher dispatcher, IDataService dataService, IDialogService dialogService,
            IResolver resolver)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));

            DeleteCommand = new RelayCommand(DeleteUser, () => SelectedUser != null);
            SaveCommand = new RelayCommand(SaveUser, CanSave);
            AddCommand = new RelayCommand(AddNewUser);

            PropertyChanged += OnPropertyChanged;
        }

        public async Task InitializeAsync()
        {
            _dialogService.ShowBusy();

            try
            {
                var user = await _dataService.GetAsync<User>();
                var users = user.Select(x => new UserModelViewModel(x));
                _dispatcher.RunInMainThread(() => Users = users.ToObservableCollection());

                UserRoles = EnumExtentions.GetValues<RoleEnum>().ToList();
            }

            catch (Exception ex)
            {
                _dialogService.HideBusy();
                Helper.RunInMainThread(() => _dialogService.ShowErrorDialog($"Initialization error: {ex.Message}"));
            }

            finally
            {
                _dialogService.HideBusy();
            }

        }

        public bool HasChanges()
        {
            return false;
        }

        private bool CanSave()
        {
            return IsValid;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedUser))
            {
                DeleteCommand.RaiseCanExecuteChanged();
            }
            if (e.PropertyName == nameof(IsValid))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public async void SaveUser()
        {
            var users = Users.Where(x => x.HasChanges()).ToList();

            if (!_dialogService.ShowQuestionDialog($" Do you want to Save user ?"))
                return;

            users.ForEach(x=> x.AcceptChanges());
            await _dataService.AddOrUpdateAsync(users.Select(x=> x.OriginalObject));
        }

        public async void DeleteUser()
        {
            if (!_dialogService.ShowQuestionDialog($" Do you want to Delete user ?"))
                return;

            await _dataService.DeleteAsync(SelectedUser.OriginalObject);

            Users.Remove(SelectedUser);
        }

        public void AddNewUser()
        {
            Users.Add(new UserModelViewModel());

        }
    }
}
