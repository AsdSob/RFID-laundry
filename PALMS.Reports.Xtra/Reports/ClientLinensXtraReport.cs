using PALMS.Reports.Common;

namespace PALMS.Reports.Xtra.Reports
{
    public partial class ClientLinensXtraReport : XtraReportBase
    {
        public ClientLinensXtraReport()
        {
            InitializeComponent();
        }

        public override void Initialize(IReport report)
        {
            if (report == null) return;

            //objectDataSource1.DataSource = report;
        }
    }
}
