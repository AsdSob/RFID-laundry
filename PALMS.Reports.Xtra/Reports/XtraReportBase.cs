using DevExpress.XtraReports.UI;
using PALMS.Reports.Common;

namespace PALMS.Reports.Xtra.Reports
{
    public class XtraReportBase : XtraReport, IXtraReport
    {
        public virtual void Initialize(IReport report)
        {
        }

        public virtual void ExportPdf(string path)
        {
            ExportToPdf(path);
        }

        public virtual void CreatePreviewDocument()
        {
            CreateDocument(true);
        }
    }
}