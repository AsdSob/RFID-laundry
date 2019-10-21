using DevExpress.DataAccess.ObjectBinding;
using PALMS.Reports.Common;

namespace PALMS.Reports.Xtra.Reports.NoteXtraReports
{
    public partial class NoteByNoteItemXtraReport : XtraReportBase
    {
        public ObjectDataSource ObjectDataSource => objectDataSource1;

        public NoteByNoteItemXtraReport()
        {
            InitializeComponent();
        }

        public override void Initialize(IReport report)
        {
            objectDataSource1.DataSource = report;
        }

    }
}
