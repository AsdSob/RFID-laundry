using GalaSoft.MvvmLight;
using TestConveyor.Svoyo.Data;

namespace TestConveyor.Svoyo.EntityModels
{
    public class ClientLinenViewModel : ViewModelBase
    {
        private ClientLinen _originalObject;
        private int _id;
        private int _departmentId;
        private int _clientId;
        private int _masterLinenId;
        private int? _staffId;
        private string _rfidTag;
        private int _statusId;

        public int StatusId
        {
            get => _statusId;
            set => Set(ref _statusId, value);
        }
        public string RfidTag
        {
            get => _rfidTag;
            set => Set(ref _rfidTag, value);
        }
        public int? StaffId
        {
            get => _staffId;
            set => Set(ref _staffId, value);
        }
        public int MasterLinenId
        {
            get => _masterLinenId;
            set => Set(ref _masterLinenId, value);
        }
        public int ClientId
        {
            get => _clientId;
            set => Set(ref _clientId, value);
        }
        public int DepartmentId
        {
            get => _departmentId;
            set => Set(ref _departmentId, value);
        }
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
        public ClientLinen OriginalObject
        {
            get => _originalObject;
            set => Set(ref _originalObject, value);
        }
        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public ClientLinenViewModel()
        {
            OriginalObject = new ClientLinen();
        }

        public ClientLinenViewModel(ClientLinen originalObject) : this()
        {
            Update(originalObject);

        }

        public void Reset()
        {
            Update(OriginalObject);
        }

        private void Update(ClientLinen originalObject)
        {
            OriginalObject = originalObject;
            Id = OriginalObject.Id;
            ClientId = OriginalObject.ClientId;
            MasterLinenId = OriginalObject.MasterLinenId;
            DepartmentId = OriginalObject.DepartmentId;
            StaffId = OriginalObject.StaffId;
            StatusId = OriginalObject.StatusId;
            RfidTag = OriginalObject.RfidTag;

        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.ClientId = ClientId;
            OriginalObject.DepartmentId = DepartmentId;
            OriginalObject.MasterLinenId = MasterLinenId;
            OriginalObject.StaffId = StaffId;
            OriginalObject.StatusId = StatusId;
            OriginalObject.RfidTag = RfidTag;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(ClientId, OriginalObject.ClientId) ||
                                    !Equals(DepartmentId, OriginalObject.DepartmentId) ||
                                    !Equals(MasterLinenId, OriginalObject.MasterLinenId) ||
                                    !Equals(StaffId, OriginalObject.StaffId) ||
                                    !Equals(StatusId, OriginalObject.StatusId) ||
                                    !Equals(RfidTag, OriginalObject.RfidTag);

    }
}
