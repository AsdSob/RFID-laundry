using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Common.EntityViewModels
{
    public class StaffEntityViewModel : ViewModelBase,IDataErrorInfo
    {
        private ClientStaffEntity _originalObject;
        private int _id;
        private string _staffId;
        private int _departmentId;
        private string _staffName;
        private string _phoneNumber;
        private string _email;
        private bool _isValid;
        private string _error;

        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }
        public bool IsValid
        {
            get => _isValid;
            set => Set(ref _isValid, value);
        }
        public string Email
        {
            get => _email;
            set => Set(() => Email, ref _email, value);
        }
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => Set(() => PhoneNumber, ref _phoneNumber, value);
        }
        public string StaffName
        {
            get => _staffName;
            set => Set(() => StaffName, ref _staffName, value);
        }
        public int DepartmentId
        {
            get => _departmentId;
            set => Set(() => DepartmentId, ref _departmentId, value);
        }
        public string StaffId
        {
            get => _staffId;
            set => Set(() => StaffId, ref _staffId, value);
        }
        public int Id
        {
            get => _id;
            set => Set(() => Id, ref _id, value);
        }
        public ClientStaffEntity OriginalObject
        {
            get => _originalObject;
            set => Set(() => OriginalObject, ref _originalObject, value);
        }

        public StaffEntityViewModel()
        {
            OriginalObject = new ClientStaffEntity();

            PropertyChanged += OnPropertyChanged;
        }

        public StaffEntityViewModel(ClientStaffEntity originalObject) :this()
        {
            Update(originalObject);
        }

        public void Update(ClientStaffEntity originalObject)
        {
            OriginalObject = originalObject;
            Id = OriginalObject.Id;
            StaffName = OriginalObject.Name;
            StaffId = OriginalObject.StaffId;
            DepartmentId = OriginalObject.DepartmentId;
            Email = OriginalObject.Email;
            PhoneNumber = OriginalObject.PhoneNumber;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = StaffName;
            OriginalObject.StaffId = StaffId;
            OriginalObject.PhoneNumber = PhoneNumber;
            OriginalObject.Email = Email;
            OriginalObject.DepartmentId = DepartmentId;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(StaffName, OriginalObject.Name) ||
                                    !Equals(StaffId, OriginalObject.StaffId) ||
                                    !Equals(PhoneNumber, OriginalObject.PhoneNumber) ||
                                    !Equals(Email, OriginalObject.Email) ||
                                    !Equals(DepartmentId, OriginalObject.DepartmentId);

        public string this[string columnName] => Validate(columnName);
        public Func<StaffEntityViewModel, string, bool> NameUniqueValidationFunc { get; set; }

        private string Validate(string columnName)
        {
            var error = String.Empty;

            if (columnName == nameof(StaffId))
            {
                StaffId.ValidateRequired(ref error);
                StaffId.ValidateByNameMaxLength(ref error);
            }
            else

            if (columnName == nameof(StaffName))
            {
                StaffName.ValidateRequired(ref error);
                StaffName.ValidateByNameMaxLength(ref error);
            }
            else

            if (columnName == nameof(DepartmentId))
            {
                DepartmentId.ValidateRequired(ref error);
            }
            else

            if (columnName == nameof(Email))
            {
                Email.ValidateEmail(ref error);
            }

            FullValidate(columnName);

            return error;
        }

        private void FullValidate(string columnName)
        {
            var error = String.Empty;

            StaffId.ValidateRequired(ref error);
            StaffId.ValidateByNameMaxLength(ref error);

            StaffName.ValidateRequired(ref error);
            StaffName.ValidateByNameMaxLength(ref error);

            DepartmentId.ValidateRequired(ref error);

            Email.ValidateEmail(ref error);

            Error = error;
            IsValid = String.IsNullOrWhiteSpace(Error);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StaffName))
            {
                if (!String.IsNullOrEmpty(StaffName))
                {
                    var regex = new Regex(@"\s+");
                    StaffName = regex.Replace(StaffName, " ");
                }
            }
            else

            if (e.PropertyName == nameof(StaffId))
            {
                if (!String.IsNullOrEmpty(StaffId))
                {
                    var regex = new Regex(@"\s+");
                    StaffId = regex.Replace(StaffId, " ");
                }
            }
            else

            if (e.PropertyName == nameof(Email))
            {
                if (!String.IsNullOrEmpty(Email))
                {
                    var regex = new Regex(@"\s");
                    Email = regex.Replace(Email, "");
                }
            }
        }
    }
}
