using System.Collections.Generic;
using PALMS.Reports.Model.NoteReports;

namespace PALMS.Reports.Model.Notes
{
    public class NoteReport : NotesReportBase
    {
        public List<NotesReportRowItem> ItemRows { get; set; }

        public NoteReport()
        {
            ItemRows = new List<NotesReportRowItem>();
        }

    }
}
