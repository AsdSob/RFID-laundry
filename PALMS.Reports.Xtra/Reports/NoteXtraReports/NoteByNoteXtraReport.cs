using System;
using PALMS.Reports.Common;

namespace PALMS.Reports.Xtra.Reports.NoteXtraReports
{
    public partial class NoteByNoteXtraReport : XtraReportBase
    {
        public NoteByNoteXtraReport()
        {
            InitializeComponent();
        }

        public override void Initialize(IReport report)
        {
            objectDataSource1.DataSource = report;

            var subReport = xrSubreport1.ReportSource as NoteByNoteItemXtraReport;
            if (subReport == null)
                throw new ArgumentException("Invalid Report Type");

            subReport.ObjectDataSource.DataSource = report;
        }
    }
}
