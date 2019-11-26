using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.ClientModel;

namespace PALMS.Settings.ViewModel.EntityViewModels
{
    public class ClientLinenEntityViewModel : ViewModelBase
    {
        private ClientLinen _originalObject;
        private int _id;
        private int _departmentId;
        private int _masterLinenId;
        private int? _staffId;
        private string _tag;
        private int _statusId;
        private int _packingValue;

        public int PackingValue
        {
            get => _packingValue;
            set => Set(() => PackingValue, ref _packingValue, value);
        }
        public int StatusId
        {
            get => _statusId;
            set => Set(ref _statusId, value);
        }
        public string Tag
        {
            get => _tag;
            set => Set(ref _tag, value);
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

        public ClientLinenEntityViewModel()
        {
            OriginalObject = new ClientLinen();
        }

        public ClientLinenEntityViewModel(ClientLinen originalObject) : this()
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
            MasterLinenId = OriginalObject.MasterLinenId;
            DepartmentId = OriginalObject.DepartmentId;
            StaffId = OriginalObject.StaffId;
            StatusId = OriginalObject.StatusId;
            PackingValue = OriginalObject.PackingValue;
            Tag = OriginalObject.Tag;

        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.DepartmentId = DepartmentId;
            OriginalObject.MasterLinenId = MasterLinenId;
            OriginalObject.StaffId = StaffId;
            OriginalObject.StatusId = StatusId;
            OriginalObject.Tag = Tag;
            OriginalObject.PackingValue = PackingValue;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(DepartmentId, OriginalObject.DepartmentId) ||
                                    !Equals(MasterLinenId, OriginalObject.MasterLinenId) ||
                                    !Equals(StaffId, OriginalObject.StaffId) ||
                                    !Equals(StatusId, OriginalObject.StatusId) ||
                                    !Equals(PackingValue, OriginalObject.PackingValue) ||
                                    !Equals(Tag, OriginalObject.Tag);

    }
}
