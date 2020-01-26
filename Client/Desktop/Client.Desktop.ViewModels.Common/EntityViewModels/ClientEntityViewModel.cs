using System;
using System.ComponentModel;
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



        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Name))
            {
                ShortName = Name;
            }

        }

        public string Error { get; set; }
        public string this[string columnName] => Validate(columnName);
        public Func<ClientEntityViewModel, string, bool> NameUniqueValidationFunc { get; set; }

        private string Validate(string columnName)
        {
            string error;

            if (columnName == nameof(Name))
            {
                if (!Name.ValidateRequired(out error) ||
                    !Name.ValidateBySpaces(out error))
                {
                    return Error = error;
                }

                if (NameUniqueValidationFunc != null && !NameUniqueValidationFunc(this, nameof(Name)))
                {
                    return Error = "Name is already exist";
                }
            }

            if (columnName == nameof(ShortName))
            {
                if (!ShortName.ValidateRequired(out error) ||
                    !ShortName.ValidateBySpaces(out error))
                {
                    return Error = error;
                }

                if (NameUniqueValidationFunc != null && !NameUniqueValidationFunc(this, nameof(ShortName)))
                {
                    return Error = "Name is already exist";
                }
            }
            Error = String.Empty;
            return null;
        }
    }
}
