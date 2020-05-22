using System;
using System.ComponentModel;
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


        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
 
        }

        public string Error { get; set; }
        public string this[string columnName] => Validate(columnName);
        public Func<StaffEntityViewModel, string, bool> NameUniqueValidationFunc { get; set; }

        private string Validate(string columnName)
        {
            //string error;

            //if (columnName == nameof(StaffName))
            //{
            //    if (!StaffName.ValidateRequired(out error) ||
            //        !StaffName.ValidateBySpaces(out error))
            //    {
            //        return error;
            //    }

            //}

            //if (columnName == nameof(StaffId))
            //{
            //    if (!StaffId.ValidateRequired(out error) ||
            //        !StaffId.ValidateBySpaces(out error) )
            //    {
            //        return error;
            //    }

            //    if (NameUniqueValidationFunc != null && !NameUniqueValidationFunc(this, nameof(StaffId)))
            //    {
            //        return "Staff Id is already exist";
            //    }
            //}
            return null;
        }
    }
}
