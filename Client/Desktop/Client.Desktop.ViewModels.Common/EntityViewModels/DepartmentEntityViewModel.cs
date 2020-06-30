using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
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

        public Func<ClientEntityViewModel, string, bool> NameUniqueValidationFunc { get; set; }

        public string this[string columnName] => Validate(columnName);

        private string Validate(string columnName)
        {
            var error = String.Empty;

            if (columnName == nameof(Name))
            {
                Name.ValidateRequired(ref error);
                Name.ValidateByNameMaxLength(ref error);
            }

            //FullValidate(columnName);
            IsValid = String.IsNullOrWhiteSpace(error);

            return error;
        }

        private void FullValidate(string columnName)
        {
            var error = String.Empty;

            Name.ValidateRequired(ref error);
            Name.ValidateByNameMaxLength(ref error);

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
        }
    }
}
