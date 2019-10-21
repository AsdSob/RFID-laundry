using PALMS.Reports.Common;

namespace PALMS.Reports.Xtra.Reports.ReportTypes
{
    public partial class LinenReportXtraReport : XtraReportBase
    {
        public LinenReportXtraReport()
        {
            InitializeComponent();
        }

        public override void Initialize(IReport report)
        {
            objectDataSource1.DataSource = report;
        }

    }
}
