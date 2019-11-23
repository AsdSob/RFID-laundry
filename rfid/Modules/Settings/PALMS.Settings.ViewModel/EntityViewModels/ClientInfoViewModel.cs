using GalaSoft.MvvmLight;
using PALMS.Data.Objects;

namespace PALMS.Settings.ViewModel.EntityViewModels
{
    public class ClientInfoViewModel : ViewModelBase
    {
        private ClientInfo _originalObject;
        private int _id;
        private int _clientId;
        private int _cityId;
        private string _address;

        public string Address
        {
            get => _address;
            set => Set(ref _address, value);
        }
        public int CityId
        {
            get => _cityId;
            set => Set(ref _cityId, value);
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
        public ClientInfo OriginalObject
        {
            get => _originalObject;
            set => Set(ref _originalObject, value);
        }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public ClientInfoViewModel()
        {
            OriginalObject = new ClientInfo();
        }

        public ClientInfoViewModel(ClientInfo originalObject) : this()
        {
            Update(originalObject);
        }

        public void Reset()
        {
            Update(OriginalObject);
        }

        private void Update(ClientInfo originalObject)
        {
            OriginalObject = originalObject;

            Id = OriginalObject.Id;
            ClientId = OriginalObject.ClientId;
            Address = OriginalObject.Address;
            CityId = OriginalObject.CityId;

        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.ClientId = ClientId;
            OriginalObject.Address = Address;
            OriginalObject.CityId = CityId;

        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(ClientId, OriginalObject.ClientId) ||
                                    !Equals(Address, OriginalObject.Address) ||
                                    !Equals(CityId, OriginalObject.CityId);
    }
}
