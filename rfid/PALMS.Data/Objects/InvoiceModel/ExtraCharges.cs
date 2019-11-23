using PALMS.Data.Objects.EntityModel;
using PALMS.Data.Objects.Payment;

namespace PALMS.Data.Objects.InvoiceModel
{
    public class ExtraCharge: NameEntity
    {
        public int? InvoiceId { get; set; }
        public double Amount { get; set; }

        public override bool IsNew => Id <= 0;

        public virtual Invoice Invoice { get; set; }
    }
}
