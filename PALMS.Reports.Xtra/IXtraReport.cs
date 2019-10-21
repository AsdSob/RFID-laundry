using PALMS.Reports.Common;

namespace PALMS.Reports.Xtra
{
    public interface IXtraReport : DevExpress.XtraReports.IReport
    {
        void Initialize(IReport report);

        void ExportPdf(string path);

        void CreatePreviewDocument();
    }
}