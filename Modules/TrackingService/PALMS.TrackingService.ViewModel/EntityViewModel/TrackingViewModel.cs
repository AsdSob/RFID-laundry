using System;
using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace PALMS.TrackingService.ViewModel.EntityViewModel
{
    public class TrackingRowViewModel : ViewModelBase, IDataErrorInfo
    {
        public Data.Objects.Tracking.TrackingService OriginalObject { get; set; }
        private int _id;
        private int _clientId;
        private string _description;
        private int _trackingTypeId;
        private int _statusId;
        private DateTime _dateOpen;
        private DateTime _dateClose;


        public DateTime DateClose
        {
            get => _dateClose;
            set => Set(ref _dateClose, value);
        }
        public DateTime DateOpen
        {
            get => _dateOpen;
            set => Set(ref _dateOpen, value);
        }
        public int StatusId
        {
            get => _statusId;
            set => Set(ref _statusId, value);
        }
        public int TrackingTypeId
        {
            get => _trackingTypeId;
            set => Set(ref _trackingTypeId, value);
        }
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
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


        public string Error { get; set; }

        public string this[string columnName] => Validate(columnName);

        public TrackingRowViewModel()
        {
            OriginalObject = new Data.Objects.Tracking.TrackingService();
        }

        public TrackingRowViewModel(Data.Objects.Tracking.TrackingService entity) : this()
        {
            OriginalObject = entity ?? throw new ArgumentNullException(nameof(entity));

            Update(entity);
        }

        [Obsolete("Use IsChanged")]
        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    OriginalObject.ClientId != ClientId ||
                                    OriginalObject.Description != Description ||
                                    OriginalObject.DateOpen != DateOpen ||
                                    OriginalObject.DateClose != DateClose ||
                                    OriginalObject.StatusId != StatusId ||
                                    OriginalObject.TrackingTypeId != TrackingTypeId;

        public void Reset()
        {
            Update(OriginalObject);
        }

        private void Update(Data.Objects.Tracking.TrackingService tracking)
        {
            OriginalObject = tracking;

            Id = OriginalObject.Id;
            ClientId = OriginalObject.ClientId;
            DateClose = OriginalObject.DateClose;
            DateOpen = OriginalObject.DateOpen;
            StatusId = OriginalObject.StatusId;
            TrackingTypeId = OriginalObject.TrackingTypeId;
            Description = OriginalObject.Description;

        }

        public string Validate(string columnName)
        {
            string error;

            return null;
        }

        public void AcceptChanges()
        {
            OriginalObject.ClientId = ClientId;
            OriginalObject.Description = Description;
            OriginalObject.DateClose = DateClose;
            OriginalObject.DateOpen = DateOpen;
            OriginalObject.StatusId = StatusId;
            OriginalObject.TrackingTypeId = TrackingTypeId;


        }
    }
}
