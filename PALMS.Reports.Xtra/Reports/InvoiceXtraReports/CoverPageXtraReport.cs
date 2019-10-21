using PALMS.Reports.Common;

namespace PALMS.Reports.Xtra.Reports.InvoiceXtraReports
{
    public partial class CoverPageXtraReport : XtraReportBase
    {
        public CoverPageXtraReport()
        {
            InitializeComponent();
        }

        public override void Initialize(IReport report)
        {
            objectDataSource1.DataSource = report;
        }
    }
}
