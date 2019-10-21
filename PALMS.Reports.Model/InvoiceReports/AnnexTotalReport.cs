using System;
using System.Collections.Generic;
using PALMS.Reports.Common;

namespace PALMS.Reports.Model.InvoiceReports
{
    public class AnnexTotalReport : IReport
    {
        public string ClientName { get; set; }
        public string DepartmentName { get; set; }

        public List<AnnexTotalItemReport> Items { get; set; }
    }

    public class AnnexTotalItemReport
    {
        public DateTime Day { get; set; }
        public double TotalQty { get; set; }
        public double TotalAmount { get; set; }
    }
}
