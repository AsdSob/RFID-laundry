using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Content.Administration
{
    public class AccountViewModel : ViewModelBase, IDataErrorInfo
    {
        private string _userName;
        private string _login;
        private string _password;
        private string _description;
        private string _email;
        private string _role;
        private string _repeatPassword;
        private int _id;
        private string _error;

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

        public Func<string, string> ValidateUnique { get; set; }

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

        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }

        public string this[string columnName] => Validate(columnName);


        private string Validate(string columnName)
        {
            var error = string.Empty;

            if (columnName == nameof(UserName))
            {
                if (string.IsNullOrWhiteSpace(UserName))
                    error = "User name is required";
                else
                    error = ValidateUnique?.Invoke(columnName);
            }
            else if (columnName == nameof(Login))
            {
                if (string.IsNullOrWhiteSpace(Login))
                    error = "Login is required";
                else
                    error = ValidateUnique?.Invoke(columnName);
            }
            else if (columnName == nameof(Password))
            {
                if (string.IsNullOrWhiteSpace(Password))
                {
                    error = "Password is required";
                }
                else if (!string.IsNullOrEmpty(RepeatPassword) && !Equals(Password, RepeatPassword))
                {
                    error = "Password not equal Repeat password";
                }
                else if (Password.Length < 5)
                {
                    error = "Password min length 5";
                }
            }
            else if (columnName == nameof(RepeatPassword))
            {
                if (string.IsNullOrWhiteSpace(RepeatPassword))
                    error = "Repeat password is required";
                else if (!Equals(Password, RepeatPassword))
                    error = "Password not equal Repeat password";
            }
            else if (columnName == nameof(Email))
            {
                if (string.IsNullOrWhiteSpace(Email))
                {
                    error = "Email is required";
                }
                else
                {
                    var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    var match = regex.Match(Email);
                    if (!match.Success)
                        error = "Email not valid";
                    else
                        error = ValidateUnique?.Invoke(columnName);
                }
            }


            Error = error;

            return Error;
        }
    }
}