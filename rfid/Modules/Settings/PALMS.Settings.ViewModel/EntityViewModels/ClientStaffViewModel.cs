using GalaSoft.MvvmLight;
using PALMS.Data.Objects;

namespace PALMS.Settings.ViewModel.EntityViewModels
{
    public class ClientStaffViewModel : ViewModelBase
    {
        private ClientStaff _originalObject;
        private int _id;
        private string _name;
        private string _staffId;
        private int _departmentId;

        public int DepartmentId
        {
            get => _departmentId;
            set => Set(ref _departmentId, value);
        }
        public string StaffId
        {
            get => _staffId;
            set => Set(ref _staffId, value);
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
        public ClientStaff OriginalObject
        {
            get => _originalObject;
            set => Set(ref _originalObject, value);
        }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public ClientStaffViewModel()
        {
            OriginalObject = new ClientStaff();
        }

        public ClientStaffViewModel(ClientStaff originalObject) : this()
        {
            Update(originalObject);
        }

        public void Reset()
        {
            Update(OriginalObject);
        }

        private void Update(ClientStaff originalObject)
        {
            OriginalObject = originalObject;

            Id = OriginalObject.Id;
            Name = OriginalObject.Name;
            StaffId = OriginalObject.StaffId;
            DepartmentId = OriginalObject.DepartmentId;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = Name;
            OriginalObject.StaffId = StaffId;
            OriginalObject.DepartmentId = DepartmentId;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(Name, OriginalObject.Name) ||
                                    !Equals(StaffId, OriginalObject.StaffId) ||
                                    !Equals(DepartmentId, OriginalObject.DepartmentId);
    }
}
