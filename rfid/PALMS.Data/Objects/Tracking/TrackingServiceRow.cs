using System;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.Tracking
{
    public class TrackingServiceRow : Entity
    {
        public int TrackingServiceId { get; set; }
        public string Comment { get; set; }
        public int OrderNumber { get; set; }
        public int UserId { get; set; }
        public string StaffName { get; set; }
        public DateTime DateOpen { get; set; }
        public DateTime DateClose { get; set; }

        public virtual TrackingService TrackingService { get; set; }

    }
}
