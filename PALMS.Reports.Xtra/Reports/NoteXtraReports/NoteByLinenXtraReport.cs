using PALMS.Reports.Common;

namespace PALMS.Reports.Xtra.Reports.NoteXtraReports
{
    public partial class NoteByLinenXtraReport : XtraReportBase
    {
        public NoteByLinenXtraReport()
        {
            InitializeComponent();
        }

        public override void Initialize(IReport report)
        {
            objectDataSource1.DataSource = report;
        }
    }
}
