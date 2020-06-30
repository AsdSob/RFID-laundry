using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Common.EntityViewModels
{
    public class StaffDetailsEntityViewModel : ViewModelBase,IDataErrorInfo
    {
        private StaffDetailsEntity _originalObject;
        private int _id;
        private string _staffId;
        private int _departmentId;
        private string _phoneNumber;
        private string _email;
        private bool _isValid;
        private string _error;
        private string _name;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
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
        public StaffDetailsEntity OriginalObject
        {
            get => _originalObject;
            set => Set(() => OriginalObject, ref _originalObject, value);
        }

        public StaffDetailsEntityViewModel()
        {
            OriginalObject = new StaffDetailsEntity();

            PropertyChanged += OnPropertyChanged;
        }

        public StaffDetailsEntityViewModel(StaffDetailsEntity originalObject) :this()
        {
            Update(originalObject);
        }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public void Update(StaffDetailsEntity originalObject)
        {
            OriginalObject = originalObject;
            Id = OriginalObject.Id;
            StaffId = OriginalObject.StaffId;
            DepartmentId = OriginalObject.DepartmentId;
            Email = OriginalObject.Email;
            PhoneNumber = OriginalObject.PhoneNumber;
            Name = OriginalObject.Name;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.StaffId = StaffId;
            OriginalObject.PhoneNumber = PhoneNumber;
            OriginalObject.Email = Email;
            OriginalObject.DepartmentId = DepartmentId;
            OriginalObject.Name = Name;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(StaffId, OriginalObject.StaffId) ||
                                    !Equals(Name, OriginalObject.Name) ||
                                    !Equals(PhoneNumber, OriginalObject.PhoneNumber) ||
                                    !Equals(Email, OriginalObject.Email) ||
                                    !Equals(DepartmentId, OriginalObject.DepartmentId);

        public string this[string columnName] => Validate(columnName);
        public Func<StaffDetailsEntityViewModel, string, bool> NameUniqueValidationFunc { get; set; }

        private string Validate(string columnName)
        {
            var error = String.Empty;

            if (columnName == nameof(Name))
            {
                Name.ValidateRequired(ref error);
                Name.ValidateByNameMaxLength(ref error);
            }
            else
            if (columnName == nameof(StaffId))
            {
                StaffId.ValidateRequired(ref error);
                StaffId.ValidateByNameMaxLength(ref error);
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

            //FullValidate(columnName);
            IsValid = String.IsNullOrWhiteSpace(error);

            return error;
        }

        private void FullValidate(string columnName)
        {
            var error = String.Empty;

            StaffId.ValidateRequired(ref error);
            StaffId.ValidateByNameMaxLength(ref error);

            Name.ValidateRequired(ref error);
            Name.ValidateByNameMaxLength(ref error);

            DepartmentId.ValidateRequired(ref error);

            Email.ValidateEmail(ref error);

            Error = error;
            IsValid = String.IsNullOrWhiteSpace(Error);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Name))
            {
                if (!String.IsNullOrEmpty(Name))
                {
                    var regex = new Regex(@"\s+");
                    Name = regex.Replace(Name, " ");
                }
            }
            else if (e.PropertyName == nameof(StaffId))
            {
                if (!String.IsNullOrEmpty(StaffId))
                {
                    var regex = new Regex(@"\s+");
                    StaffId = regex.Replace(StaffId, " ");
                }
            }
            else if (e.PropertyName == nameof(Email))
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
