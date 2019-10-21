using PALMS.Reports.Common;

namespace PALMS.Reports.Xtra.Reports.ReportTypes
{
    public partial class ClientLinenPriceXtraReport : XtraReportBase
    {
        public ClientLinenPriceXtraReport()
        {
            InitializeComponent();
        }

        public override void Initialize(IReport report)
        {
            objectDataSource2.DataSource = report;
        }

    }
}
