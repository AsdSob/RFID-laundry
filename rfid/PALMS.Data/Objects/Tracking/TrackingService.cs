using System;
using System.Collections.Generic;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.Tracking
{
    public class TrackingService : NameEntity
    {
        public int ClientId { get; set; }
        public int UserId { get; set; }
        public int TrackingTypeId { get; set; }
        public int StatusId { get; set; }
        public string Description { get; set; }
        public DateTime DateOpen { get; set; }
        public DateTime DateClose { get; set; }

        public virtual Client Client { get; set; }
        public virtual TrackingType TrackingType { get; set; }
        public virtual ICollection<TrackingServiceRow> TrackingServiceRows { get; set; }
    }
}
