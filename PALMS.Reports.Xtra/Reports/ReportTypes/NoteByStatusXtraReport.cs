using PALMS.Reports.Common;

namespace PALMS.Reports.Xtra.Reports.ReportTypes
{
    public partial class NoteByStatusXtraReport : XtraReportBase
    {
        public NoteByStatusXtraReport()
        {
            InitializeComponent();
        }

        public override void Initialize(IReport report)
        {
            objectDataSource1.DataSource = report;
        }

    }
}
