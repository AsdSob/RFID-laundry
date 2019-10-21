using PALMS.Data.Objects.EntityModel;
using PALMS.Data.Objects.LinenModel;
using PALMS.Data.Objects.Payment;

namespace PALMS.Data.Objects.ClientModel
{
    public class DepartmentContract : NameEntity
    {
        public int DepartmentId { get; set; }
        public int? FamilyLinenId { get; set; }
        public int? Quantity { get; set; }
        public double Percentage { get; set; }
        public int OrderNumber { get; set; }
        public int? InvoiceId { get; set; }

        public virtual Invoice Invoice { get; set; }
        public virtual Department Department { get; set; }
        public virtual FamilyLinen FamilyLinen { get; set; }
    }
}
