using GalaSoft.MvvmLight;
using PALMS.Data.Objects.Conveyor;

namespace PALMS.TPS.ViewModel.EntityViewModel
{
    public class CClientLinenViewModel : ViewModelBase
    {
        public int? _clientId;
        public int? _staffId;
        public string _rFID;
        private int? _statusId;
        private int? _linenId;
        private int? _beltNum;
        private int? _slotNum;
        private CClientLinen _originalObject;
        private int? _id;

        public int? Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
        public CClientLinen OriginalObject
        {
            get => _originalObject;
            set => Set(ref _originalObject, value);
        }
        public int? ClientId
        {
            get => _clientId;
            set => Set(ref _clientId, value);
        }
        public int? StaffId
        {
            get => _staffId;
            set => Set(ref _staffId, value);
        }
        public string RFID
        {
            get => _rFID;
            set => Set(ref _rFID, value);
        }
        public int? SlotNum
        {
            get => _slotNum;
            set => Set(ref _slotNum, value);
        }
        public int? BeltNum
        {
            get => _beltNum;
            set => Set(ref _beltNum, value);
        }
        public int? LinenId
        {
            get => _linenId;
            set => Set(ref _linenId, value);
        }
        public int? StatusId
        {
            get => _statusId;
            set => Set(ref _statusId, value);
        }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public CClientLinenViewModel()
        {
            OriginalObject = new CClientLinen();
        }

        public CClientLinenViewModel(CClientLinen linen) : this()
        {
            Update(linen);

        }

        public void Reset()
        {
            Update(OriginalObject);
        }

        private void Update(CClientLinen linen)
        {
            OriginalObject = linen;

            Id = OriginalObject.Id;
            ClientId = OriginalObject.ClientId;
            BeltNum = OriginalObject.BeltNum;
            SlotNum = OriginalObject.SlotNum;
            LinenId = OriginalObject.LinenId;
            StaffId = OriginalObject.StaffId;
            StatusId = OriginalObject.StatusId;
            RFID = OriginalObject.RFID;

        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.ClientId = ClientId;
            OriginalObject.BeltNum = BeltNum;
            OriginalObject.SlotNum = SlotNum;
            OriginalObject.LinenId = LinenId;
            OriginalObject.StaffId = StaffId;
            OriginalObject.StatusId = StatusId;
            OriginalObject.RFID = RFID;

        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(OriginalObject.ClientId, ClientId) ||
                                    !Equals(OriginalObject.BeltNum, BeltNum) ||
                                    !Equals(OriginalObject.SlotNum, SlotNum) ||
                                    !Equals(OriginalObject.LinenId, LinenId) ||
                                    !Equals(OriginalObject.StaffId, StaffId) ||
                                    !Equals(OriginalObject.StatusId, StatusId) ||
                                    !Equals(OriginalObject.RFID , RFID);
    }
}
