
using Storage.Laundry.Models.Abstract;

namespace Storage.Laundry.Models
{
    public class ConveyorEntity : EntityBase
    {
        public int BeltNumber { get; set; }
        public int SlotNumber { get; set; }
        public int? ClientLinenId { get; set; }

        public virtual ClientLinenEntity ClientLinenEntity { get; set; }
    }
}
