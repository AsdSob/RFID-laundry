using PALMS.Reports.Common;

namespace PALMS.Reports.Xtra.Reports.NoteXtraReports
{
    public partial class NoteByTotalXtraReport : XtraReportBase
    {
        public NoteByTotalXtraReport()
        {
            InitializeComponent();
        }
        public override void Initialize(IReport report)
        {
            objectDataSource1.DataSource = report;

        }

    }
}
