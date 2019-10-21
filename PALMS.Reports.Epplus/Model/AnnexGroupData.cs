using System.Collections.Generic;

namespace PALMS.Reports.Epplus.Model
{
    public class AnnexGroupData
    {
        public string Name { get; set; }
        public IList<AnnexGroupItem> Items { get; set; }
        public string[] Notes { get; set; }
        public double[] WeightCollection { get; set; }
        public double[] WeightDelivery { get; set; }
        public double PricePerKg { get; set; }
    }

    public class AnnexGroupItem
    {
        public double PriceDryCleaning { get; set; }
        public double PriceLaundry { get; set; }
        public double PricePressing { get; set; }
        public double[] Value1 { get; set; }
        public double[] Value2 { get; set; }
        public double[] Value3 { get; set; }
        public string Name { get; set; }
    }
}