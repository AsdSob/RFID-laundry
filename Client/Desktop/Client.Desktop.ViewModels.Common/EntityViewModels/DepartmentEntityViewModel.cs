using System;
using System.ComponentModel;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Common.EntityViewModels
{
    public class DepartmentEntityViewModel :ViewModelBase, IDataErrorInfo
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
            PropertyChanged += OnPropertyChanged;
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


        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        public string Error { get; set; }
        public string this[string columnName] => Validate(columnName);

        private string Validate(string columnName)
        {
            //string error;

            //if (columnName == nameof(Name))
            //{
            //    if (!Name.ValidateRequired(out error) ||
            //        !Name.ValidateBySpaces(out error))
            //    {
            //        return error;
            //    }

            //}

            //if (columnName == nameof(DepartmentTypeId))
            //{
            //    if (!DepartmentTypeId.ValidateRequired(out error))
            //    {
            //        return error;
            //    }
            //}
            return null;
        }
    }
}
