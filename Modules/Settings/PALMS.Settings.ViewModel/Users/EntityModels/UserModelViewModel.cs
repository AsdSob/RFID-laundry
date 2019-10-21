using System;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.Audit;
using PALMS.ViewModels.Common;

namespace PALMS.Settings.ViewModel.Users.EntityModels
{
    public class UserModelViewModel : ViewModelBase, IDataErrorInfo
    {
        private int _id;
        private string _name;
        private int _userRoleId;
        private string _password;

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }
        public int UserRoleId
        {
            get => _userRoleId;
            set => Set(ref _userRoleId, value);
        }
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
        public User OriginalObject { get; set; }
        public string Error { get; }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public Func<UserModelViewModel, string, bool> NameUniqueValidationFunc { get; set; }
        public string this[string columnName] => Validate(columnName);

        private string Validate(string columnName)
        {
            string error;

            if (columnName == nameof(Name))
            {
                if (!Name.ValidateRequired(out error))
                {
                    return error;
                }

                if (NameUniqueValidationFunc != null && !NameUniqueValidationFunc(this, nameof(Name)))
                {
                    return "Name is already exist";
                }
            }
            else if (columnName == nameof(UserRoleId))
            {
                if (!UserRoleId.ValidateRequired(out error))

                {
                    return error;
                }
            }
            else if (columnName == nameof(Password))
            {
                if (!Password.ValidateRequired(out error))
                {
                    return error;
                }
            }

            return null;
        }

        public UserModelViewModel()
        {
            OriginalObject = new User();
        }

        public UserModelViewModel(User originalObject) : this()
        {
            Update(originalObject);
        }

        public void Reset()
        {
            Update(OriginalObject);
        }

        private void Update(User user)
        {
            OriginalObject = user;

            Id = OriginalObject.Id;
            Name = OriginalObject.Name;
            UserRoleId = OriginalObject.UserRoleId;
            Password = OriginalObject.Password;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Id = Id;
            OriginalObject.Name = Name;
            OriginalObject.Password = Password;
            OriginalObject.UserRoleId = UserRoleId;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    Id != OriginalObject.Id ||
                                    Name != OriginalObject.Name ||
                                    Password != OriginalObject.Password ||
                                    UserRoleId != OriginalObject.UserRoleId;
    }
}
