using System;
using System.Collections.ObjectModel;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Content.Administration
{
    public class AccountViewModel : ViewModelBase
    {
        private string _userName;
        private string _login;
        private string _password;
        private string _description;
        private string _email;
        private string _role;
        private string _repeatPassword;
        private int _id;

        public AccountEntity OriginalObject { get; private set; }

        public int Id
        {
            get => _id;
            set => Set(() => Id, ref _id, value);
        }
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

        public string RepeatPassword
        {
            get => _repeatPassword;
            set => Set(ref _repeatPassword, value);
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

        public string Role
        {
            get => _role;
            set => Set(ref _role, value);
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
            Role = OriginalObject.Roles;
            Id = OriginalObject.Id;
        }

        public void AcceptChanges()
        {
            OriginalObject.UserName = UserName;
            OriginalObject.Login = Login;
            OriginalObject.Email = Email;
            OriginalObject.Password = Password;
            OriginalObject.Roles = Role;
            OriginalObject.Id = Id;
        }

        public bool HasChanges(Func<string, string, bool> verifySecretPasswordFunc)
        {
            if (verifySecretPasswordFunc == null)
                throw new NullReferenceException(nameof(verifySecretPasswordFunc));

            return OriginalObject.IsNew ||
                   !Equals(OriginalObject.UserName, UserName) ||
                   !Equals(OriginalObject.Login, Login) ||
                   !Equals(OriginalObject.Email, Email) ||
                   !Equals(OriginalObject.Roles, Role) ||
                   !Equals(OriginalObject.Id, Id) ||
                   !verifySecretPasswordFunc(Password, OriginalObject.Password);
        }
    }
}