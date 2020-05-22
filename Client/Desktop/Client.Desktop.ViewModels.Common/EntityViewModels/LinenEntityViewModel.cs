using System;
using System.ComponentModel;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Common.EntityViewModels
{
    public class LinenEntityViewModel : ViewModelBase, IDataErrorInfo
    {
        private ClientLinenEntity _originalObject; 
        private int _departmentId;
        private int _clientId;
        private int? _staffId;
        private int _masterLinenId;
        private string _tag;
        private int _statusId;
        private int _id;
        private int _packingValue;

        public int PackingValue
        {
            get => _packingValue;
            set => Set(() => PackingValue, ref _packingValue, value);
        }
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
            PackingValue = OriginalObject.PackingValue;
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
            OriginalObject.PackingValue = PackingValue;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(DepartmentId, OriginalObject.DepartmentId) ||
                                    !Equals(ClientId, OriginalObject.ClientId) ||
                                    !Equals(Tag, OriginalObject.RfidTag) ||
                                    !Equals(StatusId, OriginalObject.StatusId) ||
                                    !Equals(StaffId, OriginalObject.StaffId) ||
                                    !Equals(PackingValue, OriginalObject.PackingValue) ||
                                    !Equals(MasterLinenId, OriginalObject.MasterLinenId);

        public string Error { get; set; }
        public string this[string columnName] => Validate(columnName);
        public Func<ClientEntityViewModel, string, bool> NameUniqueValidationFunc { get; set; }

        private string Validate(string columnName)
        {
            //string error;

            //if (columnName == nameof(PackingValue))
            //{
            //    if (!PackingValue.ValidateRequired(out error) ||
            //        (!PackingValue.ValidateMinAmount(out error)))
            //    {
            //        return Error = error;
            //    }
            //}

            //if (columnName == nameof(MasterLinenId))
            //{
            //    if (!MasterLinenId.ValidateRequired(out error))
            //    {
            //        return Error = error;
            //    }

            //}
            //Error = String.Empty;
            return null;
        }
    }
}
