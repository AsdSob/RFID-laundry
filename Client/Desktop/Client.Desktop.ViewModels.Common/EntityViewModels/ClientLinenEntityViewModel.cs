﻿using System;
using System.ComponentModel;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Common.EntityViewModels
{
    public class ClientLinenEntityViewModel : ViewModelBase, IDataErrorInfo
    {
        private ClientLinenEntity _originalObject; 
        private int _departmentId;
        private int _clientId;
        private int _masterLinenId;
        private string _tag;
        private int _statusId;
        private int _id;
        private int _packingValue;
        private bool _isValid;
        private string _error;
        private int? _staffId;

        public bool IsSelected { get; set; }

        public int? StaffId
        {
            get => _staffId;
            set => Set(ref _staffId, value);
        }
        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }
        public bool IsValid
        {
            get => _isValid;
            set => Set(ref _isValid, value);
        }
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

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public ClientLinenEntityViewModel()
        {
            OriginalObject = new ClientLinenEntity();

            PropertyChanged += OnPropertyChanged;
        }

        public ClientLinenEntityViewModel(ClientLinenEntity originalObject) : this()
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
            PackingValue = OriginalObject.PackingValue;
            StaffId = OriginalObject.StaffId;
        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.DepartmentId = DepartmentId;
            OriginalObject.ClientId = ClientId;
            OriginalObject.RfidTag = Tag;
            OriginalObject.StatusId = StatusId;
            OriginalObject.MasterLinenId = MasterLinenId;
            OriginalObject.PackingValue = PackingValue;
            OriginalObject.StaffId = StaffId;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(DepartmentId, OriginalObject.DepartmentId) ||
                                    !Equals(ClientId, OriginalObject.ClientId) ||
                                    !Equals(Tag, OriginalObject.RfidTag) ||
                                    !Equals(StatusId, OriginalObject.StatusId) ||
                                    !Equals(PackingValue, OriginalObject.PackingValue) ||
                                    !Equals(StaffId, OriginalObject.StaffId) ||
                                    !Equals(MasterLinenId, OriginalObject.MasterLinenId);

        public string this[string columnName] => Validate(columnName);
        public Func<ClientEntityViewModel, string, bool> NameUniqueValidationFunc { get; set; }
        
        private string Validate(string columnName)
        {
            var error = String.Empty;

            if (columnName == nameof(ClientId))
            {
                ClientId.ValidateRequired(ref error);
            }
            else
            if (columnName == nameof(DepartmentId))
            {
                DepartmentId.ValidateRequired(ref error);
            }
            else
            if (columnName == nameof(MasterLinenId))
            {
                MasterLinenId.ValidateRequired(ref error);
            }
            else
            if (columnName == nameof(PackingValue))
            {
                PackingValue.ValidateMinAmount(ref error);
            }

            //FullValidate(columnName);

            IsValid = String.IsNullOrWhiteSpace(error);
            return error;
        }

        private void FullValidate(string columnName)
        {
            var error = String.Empty;

            MasterLinenId.ValidateRequired(ref error);
            ClientId.ValidateRequired(ref error);
            DepartmentId.ValidateRequired(ref error);
            PackingValue.ValidateMinAmount(ref error);

            Error = error;
            IsValid = String.IsNullOrWhiteSpace(Error);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }
    }
}
