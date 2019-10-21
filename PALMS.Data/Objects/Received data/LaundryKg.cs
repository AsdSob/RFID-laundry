using System;
using System.ComponentModel.DataAnnotations.Schema;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.Received_data
{
    public class LaundryKg : Entity
    {
        [ForeignKey("Client")]
        public int ClientId { get; set; }

        public DateTime WashingDate { get; set; }
        public int ShiftId { get; set; }
        public int KgTypeId { get; set; }
        public int LinenTypeId { get; set; }

        public double Tunnel1 { get; set; }
        public double Tunnel2 { get; set; }
        public double ExtManager { get; set; }
        public double ExtUniform { get; set; }
        public double ExtGuest { get; set; }
        public double ExtFnB { get; set; }
        public double ExtLinen { get; set; }

        public virtual Client Client { get; set; }
    }
}
