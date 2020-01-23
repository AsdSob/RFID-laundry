using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Common.EntityViewModels
{
    public class DepartmentEntityViewModel :ViewModelBase
    {
        private DepartmentEntity _originalObject;
        private string _name;
        private int _id;
        private int _clientId;
        private int _departmentTypeId;

        public int DepartmentTypeId
        {
            get => _departmentTypeId;
            set => Set(() => DepartmentTypeId, ref _departmentTypeId, value);
        }
        public int ClientId
        {
            get => _clientId;
            set => Set(() => ClientId, ref _clientId, value);
        }
        public int Id
        {
            get => _id;
            set => Set(() => Id, ref _id, value);
        }
        public string Name
        {
            get => _name;
            set => Set(() => Name, ref _name, value);
        }
        public DepartmentEntity OriginalObject
        {
            get => _originalObject;
            set => Set(() => OriginalObject, ref _originalObject, value);
        }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public DepartmentEntityViewModel()
        {
            OriginalObject = new DepartmentEntity();
        }

        public DepartmentEntityViewModel(DepartmentEntity originalObject) : this()
        {
            Update(originalObject);
        }


        public void Update(DepartmentEntity originalObject)
        {
            OriginalObject = originalObject;

            Id = OriginalObject.Id;
            Name = OriginalObject.Name;
            ClientId = OriginalObject.ClientId;
            DepartmentTypeId = OriginalObject.DepartmentTypeId;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = Name;
            OriginalObject.ClientId = ClientId;
            OriginalObject.DepartmentTypeId = DepartmentTypeId;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(Name, OriginalObject.Name) ||
                                    !Equals(DepartmentTypeId, OriginalObject.DepartmentTypeId) ||
                                    !Equals(ClientId, OriginalObject.ClientId);
    }
}
