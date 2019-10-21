using PALMS.Reports.Common;

namespace PALMS.Reports.Xtra.Reports.ReportTypes
{
    public partial class LaundryKgXtraReport : XtraReportBase
    {
        public LaundryKgXtraReport()
        {
            InitializeComponent();
        }

        public override void Initialize(IReport report)
        {
            objectDataSource1.DataSource = report;
        }

    }
}
