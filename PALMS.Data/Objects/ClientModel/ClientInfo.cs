using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.ClientModel
{
    public class ClientInfo : Entity, IAuditable
    {
        [Key]
        [ForeignKey("Client")]
        public new int Id { get; set; }
        public string Comment { get; set; }

        public int TicketId { get; set; }
        public int NoteId { get; set; }
        public int InvoiceId { get; set; }
        public bool? ByCollectionDate { get; set; }

        public string Address { get; set; }
        public double Express { get; set; }
        public double WeighPrice { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public string TRNNumber { get; set; }
        public byte[] Logo { get; set; }
        public override bool IsNew => Id <= 0;

        public virtual Client Client { get; set; }
    }
}
