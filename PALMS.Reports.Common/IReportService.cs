using System.Threading.Tasks;

namespace PALMS.Reports.Common
{
    public interface IReportService
    {
        void ShowReportPreview(IReport report);
        bool ShowReportPreviewBool(IReport report);

        void Print(IReport report);

        void Print(IReport report, string printer);

        
    }
}
