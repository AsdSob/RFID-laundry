using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Common.EntityViewModels
{
    public class ClientEntityViewModel :ViewModelBase, IDataErrorInfo
    {
        private ClientEntity _originalObject;
        private int _id;
        private string _name;
        private string _shortName;
        private int? _parentId;
        private bool _active;
        private string _address;
        private int _cityId;
        private string _error;
        private bool _isValid;

        public bool IsValid
        {
            get => _isValid;
            set => Set(ref _isValid, value);
        }
        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }
        public int CityId
        {
            get => _cityId;
            set => Set(() => CityId, ref _cityId, value);
        }
        public string Address
        {
            get => _address;
            set => Set(() => Address, ref _address, value);
        }
        public bool Active
        {
            get => _active;
            set => Set(() => Active, ref _active, value);
        }
        public int? ParentId
        {
            get => _parentId;
            set => Set(() => ParentId, ref _parentId, value);
        }
        public string ShortName
        {
            get => _shortName;
            set => Set(() => ShortName, ref _shortName, value);
        }
        public string Name
        {
            get => _name;
            set => Set(() => Name, ref _name, value);
        }
        public int Id
        {
            get => _id;
            set => Set(() => Id, ref _id, value);
        }
        public ClientEntity OriginalObject
        {
            get => _originalObject;
            set => Set(() => OriginalObject, ref _originalObject, value);
        }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public ClientEntityViewModel()
        {
            OriginalObject = new ClientEntity();

            PropertyChanged += OnPropertyChanged;
        }

        public ClientEntityViewModel(ClientEntity originalObject) : this()
        {
            Update(originalObject);
        }


        private void Update(ClientEntity originalObject)
        {
            OriginalObject = originalObject;

            Id = OriginalObject.Id;
            Name = OriginalObject.Name;
            ParentId = OriginalObject.ParentId;
            ShortName = OriginalObject.ShortName;
            Active = OriginalObject.Active;
            CityId = OriginalObject.CityId;
            Address = OriginalObject.Address;
        }

        public void Refresh()
        {
            Update(OriginalObject);
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = Name;
            OriginalObject.ParentId = ParentId;
            OriginalObject.ShortName = ShortName;
            OriginalObject.Active = Active;
            OriginalObject.CityId = CityId;
            OriginalObject.Address = Address;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(Name, OriginalObject.Name) ||
                                    !Equals(ShortName, OriginalObject.ShortName) ||
                                    !Equals(Active, OriginalObject.Active) ||
                                    !Equals(CityId, OriginalObject.CityId) ||
                                    !Equals(Address, OriginalObject.Address) ||
                                    !Equals(ParentId, OriginalObject.ParentId);



        public Func<ClientEntityViewModel, string, bool> NameUniqueValidationFunc { get; set; }

        public string this[string columnName] => Validate(columnName);


        private string Validate(string columnName)
        {
            var error = String.Empty;

            if (columnName == nameof(Name))
            {
                Name.ValidateRequired(ref error);
                Name.ValidateByNameMaxLength(ref error);
            }else 
            
            if (columnName == nameof(ShortName))
            {
                ShortName.ValidateRequired(ref error);
                ShortName.ValidateByNameMaxLength(ref error);
            }

            //FullValidate(columnName);

            return error;
        }

        private void FullValidate(string columnName)
        {
            var error = String.Empty;
            
            Name.ValidateRequired(ref error);
            Name.ValidateByNameMaxLength(ref error);

            ShortName.ValidateRequired(ref error);
            ShortName.ValidateByNameMaxLength(ref error);

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
            }else

            if (e.PropertyName == nameof(ShortName))
            {
                if (!String.IsNullOrEmpty(ShortName))
                {
                    var regex = new Regex(@"\s+");
                    ShortName = regex.Replace(ShortName, " ");
                }
            }

        }

    }
}
