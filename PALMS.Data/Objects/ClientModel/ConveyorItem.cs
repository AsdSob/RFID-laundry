using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.ClientModel
{
    public class ConveyorItem : Entity
    {
        public int BeltNumber { get; set; }
        public int SlotNumber { get; set; }
        public int? ClientLinenId { get; set; }

        public virtual ClientLinen ClientLinen { get; set; }

    }
}
