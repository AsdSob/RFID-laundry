using GalaSoft.MvvmLight;
using PALMS.Data.Objects.ClientModel;

namespace PALMS.Settings.ViewModel.EntityViewModels
{
    public class ClientEntityViewModel : ViewModelBase
    {
        private int _id;
        private string _name;
        private Client _originalObject;
        private int? _parentId;
        private string _shortName;
        private bool _active;
        private int? _cityId;

        public int? CityId
        {
            get => _cityId;
            set => Set(() => CityId, ref _cityId, value);
        }
        public bool Active
        {
            get => _active;
            set => Set(ref _active, value);
        }
        public string ShortName
        {
            get => _shortName;
            set => Set(ref _shortName, value);
        }
        public int? ParentId
        {
            get => _parentId;
            set => Set(ref _parentId, value);
        }
        public Client OriginalObject
        {
            get => _originalObject;
            set => Set(ref _originalObject, value);
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
        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public ClientEntityViewModel()
        {
            OriginalObject = new Client();
        }

        public ClientEntityViewModel(Client originalObject) : this()
        {
            Update(originalObject);
        }

        public void Reset()
        {
            Update(OriginalObject);
        }

        private void Update(Client originalObject)
        {
            OriginalObject = originalObject;

            Id = OriginalObject.Id;
            Name = OriginalObject.Name;
            ParentId = OriginalObject.ParentId;
            ShortName = OriginalObject.ShortName;
            Active = OriginalObject.Active;
            CityId = OriginalObject.CityId;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = Name;
            OriginalObject.ParentId = ParentId;
            OriginalObject.ShortName = ShortName;
            OriginalObject.Active = Active;
            OriginalObject.CityId = CityId;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(Name, OriginalObject.Name) ||
                                    !Equals(ShortName, OriginalObject.ShortName) ||
                                    !Equals(Active, OriginalObject.Active) ||
                                    !Equals(CityId, OriginalObject.CityId) ||
                                    !Equals(ParentId, OriginalObject.ParentId);
    }
}
