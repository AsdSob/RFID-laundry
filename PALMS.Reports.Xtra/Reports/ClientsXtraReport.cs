using PALMS.Reports.Common;

namespace PALMS.Reports.Xtra.Reports
{
    public partial class ClientsXtraReport : XtraReportBase
    {
        public ClientsXtraReport()
        {
            InitializeComponent();
        }

        public override void Initialize(IReport report)
        {
            mainObjectDataSource.DataSource = report;
        }
    }
}
