using System;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.ClientModel;

namespace PALMS.LinenList.ViewModel
{
    public class ClientViewModel : ViewModelBase
    {
        private Client _originalObject;
        private int _id;
        private bool _active;
        private string _name;
        private string _shortName;

        public Client OriginalObject
        {
            get => _originalObject;
            set => Set(ref _originalObject, value);
        }
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
        public bool Active
        {
            get => _active;
            set => Set(ref _active, value);
        }
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        public string ShortName
        {
            get => _shortName;
            set => Set(ref _shortName, value);
        }

        
        public ClientViewModel()
        {
        }

        public ClientViewModel(Client entity)
        {
            OriginalObject = entity ?? throw new ArgumentNullException(nameof(entity));

            Name = OriginalObject.Name;
            ShortName = OriginalObject.ShortName;
            Id = OriginalObject.Id;
            Active = OriginalObject.Active;
        }

        [Obsolete("Use IsChanged")]
        public bool HasChanges() => OriginalObject == null || OriginalObject.IsNew;

    }
}
