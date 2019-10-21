using System;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using PALMS.ViewModels.Common;
using LaundryKgModel = PALMS.Data.Objects.Received_data.LaundryKg;

namespace PALMS.LaundryKg.ViewModel.EntityViewModel
{
    public class LaundryKgEntityModel : ViewModelBase, IDataErrorInfo
    {
        private LaundryKgModel _originalObject;
        private int _id;
        private int _clientId;
        private DateTime _washingDate;
        private int _shiftId;
        private int _kgTypeId;
        private double _tunnel1;
        private double _tunnel2;
        private double _extManager;
        private double _extUniform;
        private double _extGuest;
        private double _extFnB;
        private double _extLinen;
        private int _linenTypeId;
        private string _clientOrderNumber;

        public string ClientOrderNumber
        {
            get => _clientOrderNumber;
            set => Set(ref _clientOrderNumber, value);
        }
        public int LinenTypeId
        {
            get => _linenTypeId;
            set => Set(ref _linenTypeId, value);
        }
        public double ExtLinen
        {
            get => _extLinen;
            set => Set(ref _extLinen, value);
        }
        public double ExtFnB
        {
            get => _extFnB;
            set => Set(ref _extFnB, value);
        }
        public double ExtGuest
        {
            get => _extGuest;
            set => Set(ref _extGuest, value);
        }
        public double ExtUniform
        {
            get => _extUniform;
            set => Set(ref _extUniform, value);
        }
        public double ExtManager
        {
            get => _extManager;
            set => Set(ref _extManager, value);
        }
        public double Tunnel2
        {
            get => _tunnel2;
            set => Set(ref _tunnel2, value);
        }
        public double Tunnel1
        {
            get => _tunnel1;
            set => Set(ref _tunnel1, value);
        }
        public int KgTypeId
        {
            get => _kgTypeId;
            set => Set(ref _kgTypeId, value);
        }
        public int ShiftId
        {
            get => _shiftId;
            set => Set(ref _shiftId, value);
        }
        public DateTime WashingDate
        {
            get => _washingDate;
            set => Set(ref _washingDate, value);
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
        public LaundryKgModel OriginalObject
        {
            get => _originalObject;
            set => Set(ref _originalObject, value);
        }

        public LaundryKgEntityModel()
        {
            OriginalObject = new LaundryKgModel();
        }

        public LaundryKgEntityModel(LaundryKgModel entity)
        {
            OriginalObject = entity ?? throw new ArgumentNullException(nameof(entity));

            Id = OriginalObject.Id;
            ClientId = OriginalObject.ClientId;
            WashingDate = OriginalObject.WashingDate;
            ShiftId = OriginalObject.ShiftId;
            KgTypeId = OriginalObject.KgTypeId;
            LinenTypeId = OriginalObject.LinenTypeId;

            Tunnel1 = OriginalObject.Tunnel1;
            Tunnel2 = OriginalObject.Tunnel2;
            ExtFnB = OriginalObject.ExtFnB;
            ExtGuest = OriginalObject.ExtGuest;
            ExtLinen = OriginalObject.ExtLinen;
            ExtManager = OriginalObject.ExtManager;
            ExtUniform = OriginalObject.ExtUniform;

        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(OriginalObject.ClientId, ClientId)  ||
                                    !Equals(OriginalObject.WashingDate, WashingDate) ||
                                    !Equals(OriginalObject.ShiftId, ShiftId) ||
                                    !Equals(OriginalObject.KgTypeId, KgTypeId) ||
                                    !Equals(OriginalObject.LinenTypeId, LinenTypeId) ||

                                    !Equals(OriginalObject.Tunnel1, Tunnel1 )||
                                    !Equals(OriginalObject.Tunnel2, Tunnel2 )||
                                    !Equals(OriginalObject.ExtGuest, ExtGuest )||
                                    !Equals(OriginalObject.ExtManager, ExtManager )||
                                    !Equals(OriginalObject.ExtLinen, ExtLinen )||
                                    !Equals(OriginalObject.ExtUniform, ExtUniform )||
                                    !Equals(OriginalObject.ExtFnB, ExtFnB);


        public void Reset()
        {
            Update(OriginalObject);
        }

        private void Update(LaundryKgModel originalObject)
        {
            OriginalObject = originalObject;

            if (OriginalObject == null) return;

            ClientId = OriginalObject.ClientId;
            WashingDate = OriginalObject.WashingDate;
            ShiftId = OriginalObject.ShiftId;
            KgTypeId = OriginalObject.KgTypeId;
            LinenTypeId = OriginalObject.LinenTypeId;

            Tunnel1 = OriginalObject.Tunnel1;
            Tunnel2 = OriginalObject.Tunnel2;
            ExtUniform = OriginalObject.ExtUniform;
            ExtGuest = OriginalObject.ExtGuest;
            ExtLinen = OriginalObject.ExtLinen;
            ExtManager = OriginalObject.ExtManager;
            ExtFnB = OriginalObject.ExtFnB;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.ClientId = ClientId;
            OriginalObject.KgTypeId = KgTypeId;
            OriginalObject.ShiftId = ShiftId;
            OriginalObject.WashingDate = WashingDate;
            OriginalObject.LinenTypeId = LinenTypeId;

            OriginalObject.Tunnel1 = Tunnel1;
            OriginalObject.Tunnel2 = Tunnel2;
            OriginalObject.ExtFnB = ExtFnB;
            OriginalObject.ExtGuest = ExtGuest;
            OriginalObject.ExtLinen = ExtLinen;
            OriginalObject.ExtManager = ExtManager;
            OriginalObject.ExtUniform = ExtUniform;
        }

        public string this[string columnName] => Validate(columnName);
        public string Error { get; }

        private string Validate(string columnName)
        {
            string error;

            if (columnName == nameof(KgTypeId))
            {
                if (!KgTypeId.ValidateRequired(out error))
                    return error;
            }
            else if (columnName == nameof(LinenTypeId))
            {
                if (!LinenTypeId.ValidateRequired(out error))
                    return error;
            }
            else if (columnName == nameof(ShiftId))
            {
                if (!ShiftId.ValidateRequired(out error))
                    return error;
            }
            else if (columnName == nameof(ClientId))
            {
                if (!ClientId.ValidateRequired(out error))
                    return error;
            }
            else if (columnName == nameof(WashingDate))
            {
                if (!WashingDate.ValidateRequired(out error))
                    return error;
            }

            return null;
        }
    }
}
