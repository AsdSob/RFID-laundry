using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PALMS.Reports.Epplus.Model
{
    public class RevenueGroupData
    {
        public string Name { get; set; }
        public List<RevenueGroupItem> Items { get; set; }

        public RevenueGroupData()
        {
            Items = new List<RevenueGroupItem>();
        }
    }

    public class RevenueGroupItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double[] Amount { get; set; }
        public double[] SoiledKg { get; set; }
        public double[] CleanKg { get; set; }
        public double[] SalesKg { get; set; }

        public double[] CutOfDate { get; set; }
        public double[] CutOfDatePrevious { get; set; }
    }
    
}
