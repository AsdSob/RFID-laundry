using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Windows;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Content.Administration
{
    public class AuthManageViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IAccountService _accountService;
        private readonly ILaundryService _laundryService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IResolver _resolverService;
        private ObservableCollection<AccountViewModel> _accounts;
        private AccountViewModel _selectedAccount;
        private ObservableCollection<string> _roles;
        private bool _isAdmin;
        private List<RfidReaderEntity> _rfidReaders;
        private ObservableCollection<AccountDetailEntityViewModel> _accountDetails;
        private AccountDetailEntityViewModel _selectedAccountDetails;

        public AccountDetailEntityViewModel SelectedAccountDetails
        {
            get => _selectedAccountDetails;
            set => Set(() => SelectedAccountDetails, ref _selectedAccountDetails, value);
        }
        public ObservableCollection<AccountDetailEntityViewModel> AccountDetails
        {
            get => _accountDetails;
            set => Set(() => AccountDetails, ref _accountDetails, value);
        }
        public ObservableCollection<AccountViewModel> Accounts
        {
            get { return _accounts ??= new ObservableCollection<AccountViewModel>(); }
        }

        public List<RfidReaderEntity> RfidReaders
        {
            get => _rfidReaders;
            set => Set(ref _rfidReaders, value);
        }
        public AccountViewModel SelectedAccount
        {
            get => _selectedAccount;
            set => Set(ref _selectedAccount, value);
        }

        public ObservableCollection<string> Roles
        {
            get { return _roles ??= new ObservableCollection<string>(); }
        }

        public bool IsAdmin
        {
            get => _isAdmin;
            set => Set(ref _isAdmin, value);
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand InitilizeCommand { get; }
        public RelayCommand ReaderSettingCommand { get; }

        public AuthManageViewModel(IDialogService dialogService, IAccountService accountService, IAuthenticationService authenticationService, IResolver resolver, ILaundryService laundryService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

            SaveCommand = new RelayCommand(Save, SaveCommandCanExecute);
            AddCommand = new RelayCommand(Add, AddCommandCanExecute);
            DeleteCommand = new RelayCommand(Delete, DeleteCommandCanExecute);
            InitilizeCommand = new RelayCommand(Initialize);
            ReaderSettingCommand = new RelayCommand(ReaderSetting);

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedAccount))
            {
                SaveCommand?.RaiseCanExecuteChanged();
                DeleteCommand?.RaiseCanExecuteChanged();

                SelectedAccountDetails = SelectAccountDetails();

                if (SelectedAccount != null)
                    SelectedAccount.ValidateUnique = ValidateUnique;
            }

        }

        private AccountDetailEntityViewModel SelectAccountDetails()
        {
            if (SelectedAccount == null) return null;

            var accountDetail = AccountDetails?.FirstOrDefault(x =>
                x.OriginalObject.AccountEntity == SelectedAccount?.OriginalObject ||
                x.AccountId == SelectedAccount?.Id);

            if (accountDetail == null)
            {
                accountDetail = new AccountDetailEntityViewModel
                {
                    OriginalObject = {AccountEntity = SelectedAccount.OriginalObject}
                };

                AccountDetails?.Add(accountDetail);
            }

            return accountDetail;
        }

        private void Add()
        {
            var accountViewModel = new AccountViewModel();
            Accounts.Add(accountViewModel);
            SelectedAccount = accountViewModel;
        }

        private bool AddCommandCanExecute()
        {
            return true;
        }

        private async void Initialize()
        {
            var reader = await _laundryService.GetAllAsync<RfidReaderEntity>();
            RfidReaders = reader.ToList();

            var accountDetail = await _laundryService.GetAllAsync<AccountDetailsEntity>();
            var accountDetails = accountDetail.Select(x => new AccountDetailEntityViewModel(x));
            AccountDetails = accountDetails.ToObservableCollection();

            var accounts = await _accountService.GetAllAsync<AccountEntity>();

            foreach (var accountEntity in accounts.OrderBy(x => x.Id))
            {
                Accounts.Add(new AccountViewModel(accountEntity));
            }

            SelectedAccount = Accounts.FirstOrDefault();

            Roles.Add(Common.Identity.Roles.Administrator);
            Roles.Add(Common.Identity.Roles.Manager);
        }

        private async void Save()
        {
            var isAccount = SelectedAccount.HasChanges(_authenticationService.Verify) && SelectedAccount.IsValid;
            var isAccountDetails = SelectedAccountDetails.HasChanges();

            if (isAccount)
            {
                SelectedAccount.AcceptChanges();
                await _accountService.AddOrUpdateAsync(SelectedAccount.OriginalObject);
            }

            if (isAccountDetails)
            {
                SelectedAccountDetails.AcceptChanges();
                await _laundryService.AddOrUpdateAsync(SelectedAccountDetails.OriginalObject);
            }

            if (isAccount || isAccountDetails)
            {
                _dialogService.ShowInfoDialog("Saved!");
            }

        }

        private async void ReaderSetting()
        {
           var rfidReaderWindow = _resolverService.Resolve<RfidReaderWindowModel>();

           //var showDialog = _dialogService.ShowDialog(rfidReaderWindow);

           _dialogService.ShowDialog(rfidReaderWindow);

           RfidReaders.Clear();

           var reader = await _accountService.GetAllAsync<RfidReaderEntity>();
           RfidReaders = reader.ToList();
        }

        private string ValidateUnique(string columnName)
        {
            var error = string.Empty;

            var prop = SelectedAccount.GetType().GetProperty(columnName);
            if (prop == null) return null;

            foreach (var account in Accounts)
            {
                if (Equals(prop.GetValue(account), prop.GetValue(SelectedAccount)) && !Equals(account, SelectedAccount))
                {
                    error = "Email is not unique";
                    break;
                }
                if (Equals(account.Login, SelectedAccount.Login) && !Equals(account, SelectedAccount))
                {
                    error = "Login is not unique";
                    break;
                }
                if (Equals(account.UserName, SelectedAccount.UserName) && !Equals(account, SelectedAccount))
                {
                    error = "User name is not unique";
                    break;
                }
            }

            return error;
        }

        private bool SaveCommandCanExecute()
        {
            return SelectedAccount != null;
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

            if (needSave)
                await _accountService.DeleteAsync(SelectedAccount.OriginalObject);

            Accounts.Remove(SelectedAccount);
        }

        private bool DeleteCommandCanExecute()
        {
            return SelectedAccount != null;
        }
    }
}
