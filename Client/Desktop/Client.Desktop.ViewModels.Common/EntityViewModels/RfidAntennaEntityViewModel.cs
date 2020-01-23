using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Common.EntityViewModels
{
    public class RfidAntennaEntityViewModel :ViewModelBase
    {
        private int _id;
        private string _name;
        private int _rfidReaderId;
        private double _rxSensitivity;
        private double _txPower;
        private RfidAntennaEntity _originalObject;
        private int _antennaNumb;

        public int AntennaNumb
        {
            get => _antennaNumb;
            set => Set(() => AntennaNumb, ref _antennaNumb, value);
        }
        public RfidAntennaEntity OriginalObject
        {
            get => _originalObject;
            set => Set(() => OriginalObject, ref _originalObject, value);
        }
        public double TxPower
        {
            get => _txPower;
            set => Set(() => TxPower, ref _txPower, value);
        }
        public double RxSensitivity
        {
            get => _rxSensitivity;
            set => Set(() => RxSensitivity, ref _rxSensitivity, value);
        }
        public int RfidReaderId
        {
            get => _rfidReaderId;
            set => Set(() => RfidReaderId, ref _rfidReaderId, value);
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
        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public RfidAntennaEntityViewModel()
        {
            OriginalObject = new RfidAntennaEntity();
        }

        public RfidAntennaEntityViewModel(RfidAntennaEntity originalObject) : this()
        {
            Update(originalObject);
        }


        private void Update(RfidAntennaEntity originalObject)
        {
            OriginalObject = originalObject;

            Id = OriginalObject.Id;
            Name = OriginalObject.Name;
            RfidReaderId = OriginalObject.RfidReaderId;
            RxSensitivity = OriginalObject.RxSensitivity;
            TxPower = OriginalObject.TxPower;
            AntennaNumb = OriginalObject.AntennaNumb;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = Name;
            OriginalObject.RfidReaderId = RfidReaderId;
            OriginalObject.RxSensitivity = RxSensitivity;
            OriginalObject.TxPower = TxPower;
            OriginalObject.AntennaNumb = AntennaNumb;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(Name, OriginalObject.Name) ||
                                    !Equals(AntennaNumb, OriginalObject.AntennaNumb) ||
                                    !Equals(RfidReaderId, OriginalObject.RfidReaderId) ||
                                    !Equals(RxSensitivity, OriginalObject.RxSensitivity) ||
                                    !Equals(TxPower, OriginalObject.TxPower);
    }
}
