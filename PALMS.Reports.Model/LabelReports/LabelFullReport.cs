using System;
using System.Drawing;
using PALMS.Reports.Common;

namespace PALMS.Reports.Model
{
    public class LabelFullReport : IReport
    {
        public string VendorName { get; set; }
        public DateTime CreateDate { get; set; }
        public string CustomerName { get; set; }
        public string DepartmentName { get; set; }
        public string LinenName { get; set; }
        public int Quantity { get; set; }
        public Bitmap PrimeLogo { get; set; }
        public Bitmap ClientLogo { get; set; }
        public string TicketLinenCondition { get; set; }
        public string StaffId { get; set; }
    }
}

