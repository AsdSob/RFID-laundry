using System;
using System.Collections.Generic;
using PALMS.Reports.Common;

namespace PALMS.Reports.Model
{
    public class ClientLinensReport : IReport
    {
        public string CustomerNumber { get; set; }
        public string BillNumber { get; set; }
        public DateTime Created { get; set; }
        public List<TestReportItem> Items { get; set; }

        public ClientLinensReport()
        {
            Items = new List<TestReportItem>();
        }
    }

    public class TestReportItem
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Total { get; set; }
    }
}
