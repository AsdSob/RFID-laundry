using System;
using PALMS.Reports.Common;

namespace PALMS.Reports.Model.LabelReports
{
    public class LabelNameOnlyReport: IReport
    {
        public string ClientName { get; set; }
        public DateTime CreateDate { get; set; }
        public string StaffId { get; set; }
    }
}
