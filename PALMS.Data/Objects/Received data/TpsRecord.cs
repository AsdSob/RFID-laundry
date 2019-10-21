using System;
using System.ComponentModel.DataAnnotations.Schema;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.EntityModel;
using PALMS.Data.Objects.LinenModel;

namespace PALMS.Data.Objects.Received_data
{
    public class TpsRecord : Entity
    {
        [ForeignKey("Client")]
        public int ClientId { get; set; }

        [ForeignKey("LinenList")]
        public int? LinenListId { get; set; }
        public int? Quantity { get; set; }
        public int? Package { get; set; }
        public string TicketType { get; set; }
        public DateTime PrintDate { get; set; }
        public string StaffId { get; set; }

        public virtual Client Client { get; set; }
        public virtual LinenList LinenList { get; set; }
    }
}
