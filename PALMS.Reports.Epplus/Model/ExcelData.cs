using System;
using System.Collections.Generic;
using PALMS.Reports.Common;

namespace PALMS.Reports.Epplus.Model
{
    public class ExcelData : IReport
    {
        public ReportType ReportType { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime PrintDate { get; set; }
        public DateTime[] Days { get; set; }


        // Annex Group
        public IList<AnnexGroupData> Groups { get; set; }
        public string Month { get; set; }
        public string NoteType { get; set; }


        //Coordinate Group
        public string[] HorizontalData { get; set; }
        public List<CoordinateGroupData> VerticalData { get; set; }


        //Revenue Group
        public List<RevenueGroupData> RevenueGroupDatas { get; set; }
    }
    
}