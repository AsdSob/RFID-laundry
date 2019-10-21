using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PALMS.Reports.Common;

namespace PALMS.Reports.Model.ReportTypes
{
    public class LaundryKgReport : IReport
    {
        public DateTime WashingDate { get; set; }
        public string Shift { get; set; }
        public string LaundryKgType { get; set; }
        public string WashingType { get; set; }
        public List<LaundryKgItems> Items { get; set; }
    }

    public class LaundryKgItems
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public double Tunnel1 { get; set; }
        public double Tunnel2 { get; set; }
        public double ExtFnB { get; set; }
        public double ExtManagement { get; set; }
        public double ExtUniform { get; set; }
        public double ExtGuest { get; set; }
        public double ExtLinen { get; set; }
    }
}
