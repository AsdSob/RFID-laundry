using PALMS.Reports.Common;

namespace PALMS.Reports.Xtra.Reports.LabelXtraReports
{
    public partial class LabelNameOnlyXtraReport : XtraReportBase
    {
        public LabelNameOnlyXtraReport()
        {
            InitializeComponent();
        }

        public override void Initialize(IReport report)
        {
            objectDataSource1.DataSource = report;
        }

    }
}
