using System;
using PALMS.Reports.Common;

namespace PALMS.Reports.Xtra.Reports.NoteXtraReports
{
    public partial class NoteXtraReport : XtraReportBase
    {
        public NoteXtraReport()
        {
            InitializeComponent();
        }

        public override void Initialize(IReport report)
        {
            objectDataSource1.DataSource = report;
        }
    }
}
