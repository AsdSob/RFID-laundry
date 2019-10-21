using System.Windows;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;
using PALMS.Reports.Common;

namespace PALMS.Reports.Xtra
{
    public class ReportsService : IReportService
    {
        public void ShowReportPreview(IReport report)
        {
            if (report == null) return;

            var xtraReport = ReportFactory.Create(report);

            var window = new DocumentPreviewWindow {WindowStartupLocation = WindowStartupLocation.CenterScreen};
            window.PreviewControl.DocumentSource = xtraReport;
            xtraReport.CreatePreviewDocument();
            window.ShowDialog();
            
        }

        public bool ShowReportPreviewBool(IReport report)
        {
            if (report == null) return false;

            var xtraReport = ReportFactory.Create(report);

            var window = new DocumentPreviewWindow { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            window.PreviewControl.DocumentSource = xtraReport;
            xtraReport.CreatePreviewDocument();

            return (bool) window.ShowDialog();
        }

        public void Print(IReport report)
        {
            if (report == null) return;

            var xtraReport = ReportFactory.Create(report);

            using (ReportPrintTool printTool = new ReportPrintTool(xtraReport))
            {
                printTool.Print();
            }
        }

        public void Print(IReport report, string printer)
        {
            if (report == null) return;

            var xtraReport = ReportFactory.Create(report);

            using (ReportPrintTool printTool = new ReportPrintTool(xtraReport))
            {
                printTool.Print(printer);
            }
        }

    }
}