using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.LinenModel
{
    public class LeasingLinen : NameEntity
    {
        public int LinenListId { get; set; }
        public double OriginalPrice { get; set; }

        public virtual LinenList LinenList { get; set; }
    }
}
