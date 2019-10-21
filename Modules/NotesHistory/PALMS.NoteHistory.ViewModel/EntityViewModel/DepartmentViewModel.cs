using System.ComponentModel;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.ClientModel;
using PALMS.ViewModels.Common;

namespace PALMS.NoteHistory.ViewModel.EntityViewModel
{
    public class DepartmentViewModel : ViewModelBase, IDataErrorInfo
    {
        private int _id;
        private string _name;
        private int? _parentId;
        private int _clientId;
        private bool _allFree;
        private int _keyId;

        public int ClientId
        {
            get => _clientId;
            set => Set(ref _clientId, value);
        }
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        public int? ParentId
        {
            get => _parentId;
            set => Set(ref _parentId, value);
        }
        public int KeyId
        {
            get => _keyId;
            set => Set(ref _keyId, value);
        }
        public bool AllFree
        {
            get => _allFree;
            set => Set(ref _allFree, value);
        }

        public Department OriginalObject { get; set; }
        public string Error { get; }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public DepartmentViewModel()
        {
            OriginalObject = new Department();

        }
        public DepartmentViewModel(Department department) : this()
        {
            Update(department);
            KeyId = OriginalObject.Id;

        }

        public void Reset()
        {
            Update(OriginalObject);
        }

        private void Update(Department department)
        {
            OriginalObject = department;

            Id = OriginalObject.Id;
            Name = OriginalObject.Name;
            ClientId = OriginalObject.ClientId;
            ParentId = OriginalObject.ParentId;
            AllFree = OriginalObject.AllFree;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = Name;
            OriginalObject.ClientId = ClientId;
            OriginalObject.ParentId = ParentId;
            OriginalObject.AllFree = AllFree;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    Name != OriginalObject.Name ||
                                    ParentId != OriginalObject.ParentId ||
                                    ClientId != OriginalObject.ClientId ||
                                    AllFree != OriginalObject.AllFree;

        public string this[string columnName] => Validate(columnName);

        private string Validate(string columnName)
        {
            string error;
            if (columnName == nameof(Name))
            {
                if (!Name.ValidateRequired(out error) ||
                    !Name.ValidateBySpaces(out error) ||
                    !Name.ValidateByMaxLength(out error))
                {
                    return error;
                }
            }
            return null;
        }
    }
}
