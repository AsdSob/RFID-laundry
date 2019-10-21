
namespace PALMS.Reports.Model.Invoice
{
    public class CoverPageItemReport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public double AmountWithoutVat { get; set; }
        public double VatAmount { get; set; }
    }
}
