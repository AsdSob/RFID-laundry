using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Common.EntityViewModels
{
    public class LinenEntityViewModel : ViewModelBase
    {
        private ClientLinenEntity _originalObject;
        private int _departmentId;
        private int _clientId;
        private int? _staffId;
        private int _masterLinenId;
        private string _tag;
        private int _statusId;
        private int _id;

        public int Id
        {
            get => _id;
            set => Set(() => Id, ref _id, value);
        }
        public int StatusId
        {
            get => _statusId;
            set => Set(() => StatusId, ref _statusId, value);
        }
        public string Tag
        {
            get => _tag;
            set => Set(() => Tag, ref _tag, value);
        }
        public int MasterLinenId
        {
            get => _masterLinenId;
            set => Set(() => MasterLinenId, ref _masterLinenId, value);
        }
        public int? StaffId
        {
            get => _staffId;
            set => Set(() => StaffId, ref _staffId, value);
        }
        public int ClientId
        {
            get => _clientId;
            set => Set(() => ClientId, ref _clientId, value);
        }
        public int DepartmentId
        {
            get => _departmentId;
            set => Set(() => DepartmentId, ref _departmentId, value);
        }
        public ClientLinenEntity OriginalObject
        {
            get => _originalObject;
            set => Set(() => OriginalObject, ref _originalObject, value);
        }

        public LinenEntityViewModel()
        {
            OriginalObject = new ClientLinenEntity();
        }

        public LinenEntityViewModel(ClientLinenEntity originalObject) : this()
        {
            Update(originalObject);
        }

        public void Update(ClientLinenEntity originalObject)
        {
            OriginalObject = originalObject;

            Id = OriginalObject.Id;
            DepartmentId = OriginalObject.DepartmentId;
            ClientId = OriginalObject.ClientId;
            Tag = OriginalObject.RfidTag;
            StatusId = OriginalObject.StatusId;
            MasterLinenId = OriginalObject.MasterLinenId;
            StaffId = OriginalObject.StaffId;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.DepartmentId = DepartmentId;
            OriginalObject.ClientId = ClientId;
            OriginalObject.RfidTag = Tag;
            OriginalObject.StatusId = StatusId;
            OriginalObject.StaffId = StaffId;
            OriginalObject.MasterLinenId = MasterLinenId;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(DepartmentId, OriginalObject.DepartmentId) ||
                                    !Equals(ClientId, OriginalObject.ClientId) ||
                                    !Equals(Tag, OriginalObject.RfidTag) ||
                                    !Equals(StatusId, OriginalObject.StatusId) ||
                                    !Equals(StaffId, OriginalObject.StaffId) ||
                                    !Equals(MasterLinenId, OriginalObject.MasterLinenId);
    }
}
