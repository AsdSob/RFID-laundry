using System;
using System.Collections.Generic;
using PALMS.Reports.Common;

namespace PALMS.Reports.Model.ReportTypes
{
    public class LinenReportViewModel : IReport
    {
        public string ClientName { get; set; }
        public string DepartmentName { get; set; }
        public double CollectionWeight { get; set; }
        public double DeliveryWeight { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime DateStart { get; set; }

        public List<LinenReportItem> Items{ get; set; }

    }

    public class LinenReportItem 
    {
        public string LinenName { get; set; }
        public int LinenId { get; set; }
        public double CollectionQty { get; set; }
        public double DeliveryQty { get; set; }
        public double ClientReceivedQty { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }

        public double DifferenceCollectionDelivery { get; set; }
        public double DifferenceCollectionClientReceive { get; set; }

    }
}
