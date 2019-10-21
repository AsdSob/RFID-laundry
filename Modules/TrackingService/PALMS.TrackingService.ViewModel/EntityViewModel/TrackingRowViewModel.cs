using System;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.Tracking;

namespace PALMS.TrackingService.ViewModel.EntityViewModel
{
    public class TrackingViewModel : ViewModelBase, IDataErrorInfo
    {
        public TrackingServiceRow OriginalObject { get; set; }
        private int _id;
        private string _comment;
        private DateTime _dateOpen;
        private DateTime _dateClose;
        private int _orderNumber;
        private int _trackingServiceId;
        private int _userId;
        private string _staffName;


        public string StaffName
        {
            get => _staffName;
            set => Set(ref _staffName, value);
        }
        public int UserId
        {
            get => _userId;
            set => Set(ref _userId, value);
        }
        public int TrackingServiceId
        {
            get => _trackingServiceId;
            set => Set(ref _trackingServiceId, value);
        }
        public int OrderNumber
        {
            get => _orderNumber;
            set => Set(ref _orderNumber, value);
        }
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
        public string Comment
        {
            get => _comment;
            set => Set(ref _comment, value);
        }
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }


        public string Error { get; set; }

        public string this[string columnName] => Validate(columnName);

        public TrackingViewModel()
        {
            OriginalObject = new TrackingServiceRow();
        }

        public TrackingViewModel(TrackingServiceRow entity) : this()
        {
            OriginalObject = entity ?? throw new ArgumentNullException(nameof(entity));

            Update(entity);
        }

        [Obsolete("Use IsChanged")]
        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew || 
                                    OriginalObject.UserId != UserId ||
                                    OriginalObject.Comment != Comment ||
                                    OriginalObject.DateClose != DateClose ||
                                    OriginalObject.DateOpen != DateOpen ||
                                    OriginalObject.OrderNumber != OrderNumber ||
                                    OriginalObject.TrackingServiceId != TrackingServiceId ||
                                    OriginalObject.StaffName != StaffName;


        public void Reset()
        {
            Update(OriginalObject);
        }

        private void Update(TrackingServiceRow tracking)
        {
            OriginalObject = tracking;

            Id = OriginalObject.Id;
            DateClose = OriginalObject.DateClose;
            DateOpen = OriginalObject.DateOpen;
            UserId = OriginalObject.UserId;
            Comment = OriginalObject.Comment;
            OrderNumber = OriginalObject.OrderNumber;
            TrackingServiceId = OriginalObject.TrackingServiceId;
            StaffName = OriginalObject.StaffName;

        }

        public string Validate(string columnName)
        {
            string error;

            return null;
        }

        public void AcceptChanges()
        {
            OriginalObject.DateClose = DateClose;
            OriginalObject.DateOpen = DateOpen;
            OriginalObject.Comment = Comment;
            OriginalObject.UserId = UserId;
            OriginalObject.OrderNumber = OrderNumber;
            OriginalObject.StaffName = StaffName;
            OriginalObject.TrackingServiceId = TrackingServiceId;

        }
    }
}
