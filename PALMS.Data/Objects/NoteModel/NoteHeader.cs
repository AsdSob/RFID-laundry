using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.EntityModel;
using PALMS.Data.Objects.Payment;

namespace PALMS.Data.Objects.NoteModel
{
    public class NoteHeader : NameEntity  
    {
        [ForeignKey("Client")]
        public int ClientId { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }

        public int DeliveryTypeId { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CollectionDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DeliveredDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime ClientReceivedDate { get; set; }
        public double ExpressCharge { get; set; }
        public double WeightPrice { get; set; }
        public double CollectionWeight { get; set; }
        public double DeliveryWeight { get; set; }
        public string Comment { get; set; }
        public int? InvoiceId { get; set; }
        public int? NoteStatus { get; set; }


        public virtual Invoice Invoice { get; set; }
        public virtual Client Client { get; set; }
        public virtual Department Department { get; set; }
        public virtual ICollection<NoteRow> NoteRows { get; set; }
    }
}
