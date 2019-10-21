using System;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.ClientModel;
using PALMS.ViewModels.Common;

namespace PALMS.ViewModels.EntityViewModel
{
    public class DepartmentViewModel : ViewModelBase, IDataErrorInfo
    {
        private int _id;
        private string _name;
        private int? _keyParentId;
        private int _clientId;
        private bool _allFree;
        private int _keyId;
        private int? _parentId;
        private bool _labeling;
        private int? _departmentType;
        private double _express;

        public double Express
        {
            get => _express;
            set => Set(ref _express, value);
        }
        public int? DepartmentType
        {
            get => _departmentType;
            set => Set(ref _departmentType, value);
        }
        public bool Labeling
        {
            get => _labeling;
            set => Set(ref _labeling, value);
        }
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

        public int? ParentId
        {
            get => _parentId;
            set => Set(ref _parentId, value);
        }

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        /// <summary>
        /// Parent ID for tree visualization.
        /// We can use original ParentId for set but for saving use ONLY original parent department object.
        /// Dont't save in DB!
        /// </summary>
        public int? KeyParentId
        {
            get => _keyParentId;
            set => Set(ref _keyParentId, value);
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
            KeyParentId = OriginalObject.ParentId;
            Labeling = OriginalObject.Labeling;
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
            Labeling = OriginalObject.Labeling;
            DepartmentType = OriginalObject.DepartmentType;
            Express = OriginalObject.Express;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = Name;
            OriginalObject.ClientId = ClientId;
            OriginalObject.ParentId = ParentId;
            OriginalObject.AllFree = AllFree;
            OriginalObject.Labeling = Labeling;
            OriginalObject.DepartmentType = DepartmentType;
            OriginalObject.Express = Express;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    Name != OriginalObject.Name ||
                                    KeyParentId != OriginalObject.ParentId ||
                                    ClientId != OriginalObject.ClientId ||
                                    AllFree != OriginalObject.AllFree ||
                                    Labeling != OriginalObject.Labeling ||
                                    !Equals(Express, OriginalObject.Express) ||
                                    DepartmentType != OriginalObject.DepartmentType;

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

            if (columnName == nameof(DepartmentType))
            {
                if (!Name.ValidateRequired(out error))
                {
                    return error;
                }
            }
            return null;
        }
    }
}
