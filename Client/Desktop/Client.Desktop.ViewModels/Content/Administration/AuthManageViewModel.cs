using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Core.Abstract;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Content.Administration
{
    public class AuthManageViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IMainDispatcher _mainDispatcher;
        private readonly IAccountService _accountService;
        private readonly IDbContextFactory _dbContextFactory;
        private ObservableCollection<AccountViewModel> _accounts;
        private AccountViewModel _selectedAccount;

        public ObservableCollection<AccountViewModel> Accounts
        {
            get { return _accounts ??= new ObservableCollection<AccountViewModel>(); }
        }

        public AccountViewModel SelectedAccount
        {
            get => _selectedAccount;
            set => Set(ref _selectedAccount, value);
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand InitilizeCommand { get; }


        public AuthManageViewModel(IDialogService dialogService, IMainDispatcher mainDispatcher, IAccountService accountService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _mainDispatcher = mainDispatcher ?? throw new ArgumentNullException(nameof(mainDispatcher));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));

            SaveCommand = new RelayCommand(Save, SaveCommandCanExecute);
            DeleteCommand = new RelayCommand(Delete, DeleteCommandCanExecute);
            InitilizeCommand = new RelayCommand(Initialize);
        }

        private async void Initialize()
        {
            await Task.Factory.StartNew(() =>
            {
                foreach (var i in Enumerable.Range(1, 100))
                {
                    _mainDispatcher.RunInMainThread(() => Accounts.Add(new AccountViewModel
                    {
                        UserName = $"user {i}",
                        Login = $"user{i}"
                        , Description = $"{i} yeap yeap very busy"
                    }));

                    Thread.Sleep(100);
                }
            });

        }

        private async void Save()
        {
            if (!_dialogService.ShowQuestionDialog("Save changes?") == false)
                return;

            if (!Validate(out var error))
            {
                _dialogService.ShowWarnigDialog($"Validation error:{Environment.NewLine}{Environment.NewLine}" +
                                                $"{error}");

                return;
            }

            SelectedAccount.AcceptChanges();

            await _accountService.AddOrUpdateAsync(SelectedAccount.OriginalObject);

        }

        private bool SaveCommandCanExecute()
        {
            return true;
        }

        private async void Delete()
        {
            if (_dialogService.ShowQuestionDialog(
                    $"The account '{SelectedAccount.UserName} will be removed.{Environment.NewLine}{Environment.NewLine}" +
                    "Are you sure?") == false)
            {
                return;
            }

            var needSave = SelectedAccount.OriginalObject != null &&
                           SelectedAccount.OriginalObject.IsNew == false;

            Accounts.Remove(SelectedAccount);

            if (needSave)
                await _accountService.DeleteAsync(SelectedAccount.OriginalObject);
        }

        private bool DeleteCommandCanExecute()
        {
            return SelectedAccount != null;
        }
    }

    public class AccountViewModel : ViewModelBase
    {
        private string _userName;
        private string _login;
        private string _password;
        private string _description;
        private string _email;
        private ObservableCollection<string> _roles;

        public AccountEntity OriginalObject { get; private set; }

        public string UserName
        {
            get => _userName;
            set => Set(ref _userName, value);
        }

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

        public string Email
        {
            get => _email;
            set => Set(ref _email, value);
        }

        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        public ObservableCollection<string> Roles
        {
            get => _roles;
            set => Set(ref _roles, value);
        }

        public AccountViewModel()
        {
            OriginalObject = new AccountEntity();
        }

        public AccountViewModel(AccountEntity originalObject)
        {
            Update(originalObject);
        }

        private void Update(AccountEntity originalObject)
        {
            OriginalObject = originalObject;

            UserName = OriginalObject.UserName;
            Login = OriginalObject.Login;
            Email = OriginalObject.Email;
            Roles = OriginalObject.Roles;
        }

        public void AcceptChanges()
        {
            OriginalObject.UserName = UserName;
            OriginalObject.Login = Login;
            OriginalObject.Email = Email;
            OriginalObject.Password = Password;
            OriginalObject.Roles = string.Join(",", Roles ?? new ObservableCollection<string>());
        }
    }
}
