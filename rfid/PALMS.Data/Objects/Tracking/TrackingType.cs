using System.Collections.Generic;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.Tracking
{
    public class TrackingType : NameEntity
    {
        public virtual ICollection<TrackingService> TrackingServices { get; set; }
    }
}
