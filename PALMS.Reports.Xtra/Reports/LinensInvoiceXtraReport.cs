using DevExpress.XtraReports.UI;
using PALMS.Reports.Common;

namespace PALMS.Reports.Xtra.Reports
{
    public partial class LinensInvoiceXtraReport : XtraReportBase
    {
        public LinensInvoiceXtraReport()
        {
            InitializeComponent();
        }

        public override void Initialize(IReport report)
        {
            objectDataSource1.DataSource = report;
        }
    }
}
