using System.ComponentModel.DataAnnotations.Schema;
using PALMS.Data.Objects.EntityModel;
using PALMS.Data.Objects.LinenModel;

namespace PALMS.Data.Objects.NoteModel
{
    public class NoteRow: Entity
    {
        public int NoteHeaderId { get; set; }

        [ForeignKey("LinenList")]
        public int LinenListId { get; set; }

        public double PrimeCollectionQty { get; set; }
        public double PrimeDeliveryQty { get; set; }
        public double ClientReceivedQty { get; set; }
        public string Comment { get; set; }

        public int ServiceTypeId { get; set; }
        public double Price { get; set; }
        public int PriceUnit { get; set; }
        public int Weight { get; set; }

        public virtual NoteHeader NoteHeader { get; set; }
        public virtual LinenList LinenList { get; set; }
    }
}
