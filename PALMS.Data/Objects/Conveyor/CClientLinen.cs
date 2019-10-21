using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.Conveyor
{
    public class CClientLinen: Entity
    {
        public int? ClientId { get; set; }
        public int? StaffId { get; set; }
        public string RFID { get; set; }
        public int? StatusId { get; set; }
        public int? LinenId { get; set; }
        public int? BeltNum { get; set; }
        public int? SlotNum { get; set; }
    }
}
