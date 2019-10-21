using PALMS.Reports.Common;

namespace PALMS.Reports.Xtra.Reports.Labels
{
    public partial class LabelLogoXtraReport : XtraReportBase
    {
        public LabelLogoXtraReport()
        {
            InitializeComponent();
        }

        public override void Initialize(IReport report)
        {
            objectDataSource1.DataSource = report;
        }

    }
}
