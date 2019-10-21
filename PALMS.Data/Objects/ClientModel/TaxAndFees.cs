using PALMS.Data.Objects.EntityModel;
using PALMS.Data.Objects.Payment;

namespace PALMS.Data.Objects.ClientModel
{
    public class TaxAndFees : NameEntity
    {
        public int ClientId { get; set; }
        public int UnitId { get; set; }
        public double Number { get; set; }
        public int Priority { get; set; }
        public int? InvoiceId { get; set; }

        public override bool IsNew => Id <= 0;

        public virtual Client Client { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}
