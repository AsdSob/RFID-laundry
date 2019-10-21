using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PALMS.Reports.Epplus.Model
{
    public class CoordinateGroupData
    {
        public string Name { get; set; }
        public List<CoordinateGroupItem> VerticalData { get; set; }

        public CoordinateGroupData()
        {
            VerticalData = new List<CoordinateGroupItem>();
        }
    }

    public class CoordinateGroupItem
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public double[] Values { get; set; }
    }
}
