using GalaSoft.MvvmLight;
using TestConveyor.Svoyo.Data;

namespace TestConveyor.Svoyo.EntityModels
{
    public class DepartmentViewModel : ViewModelBase
    {
        private Department _originalObject;
        private int _id;
        private string _name;
        private int _parentId;
        private int _departmentTypeId;
        private int _clientId;

        public int ClientId
        {
            get => _clientId;
            set => Set(ref _clientId, value);
        }
        public int DepartmentTypeId
        {
            get => _departmentTypeId;
            set => Set(ref _departmentTypeId, value);
        }
        public int ParentId
        {
            get => _parentId;
            set => Set(ref _parentId, value);
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
        public Department OriginalObject
        {
            get => _originalObject;
            set => Set(ref _originalObject, value);
        }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public DepartmentViewModel()
        {
            OriginalObject = new Department();
        }

        public DepartmentViewModel(Department originalObject) : this()
        {
            Update(originalObject);
        }

        public void Reset()
        {
            Update(OriginalObject);
        }

        private void Update(Department originalObject)
        {
            OriginalObject = originalObject;

            Id = OriginalObject.Id;
            Name = OriginalObject.Name;
            ParentId = OriginalObject.ParentId;
            DepartmentTypeId = OriginalObject.DepartmentTypeId;
            ClientId = OriginalObject.ClientId;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = Name;
            OriginalObject.ParentId = ParentId;
            OriginalObject.DepartmentTypeId = DepartmentTypeId;
            OriginalObject.ClientId = ClientId;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(Name, OriginalObject.Name) ||
                                    !Equals(ClientId, OriginalObject.ClientId) ||
                                    !Equals(DepartmentTypeId, OriginalObject.DepartmentTypeId) ||
                                    !Equals(ParentId, OriginalObject.ParentId);
    }
}
