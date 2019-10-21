using System;
using System.Collections.Generic;
using PALMS.Reports.Common;

namespace PALMS.Reports.Model.ReportTypes
{
    public class NoteByStatusReportViewModel : IReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string NoteStatuses { get; set; }

        public List<NoteByStatusReporItem> Items { get; set; }
    }

    public class NoteByStatusReporItem
    {
        public string NoteName { get; set; }
        public string Client{ get; set; }
        public string Department { get; set; }
        public string NoteStatus { get; set; }
        public DateTime CollectionDate { get; set; }
        public DateTime DeliveryDate { get; set; }
    }

}
