using System;
using PALMS.Reports.Common;
using PALMS.Reports.Epplus.Model;

namespace PALMS.Reports.Epplus.Services
{
    public class EpplusReportService : IExcelReportService
    {
        private readonly string _templatesDirectory;

        public EpplusReportService(string templatesDirectory)
        {
            _templatesDirectory = templatesDirectory ?? throw new ArgumentNullException(nameof(templatesDirectory));
        }

        public void ShowReportPreview(IReport report)
        {
            throw new NotImplementedException();
        }

        public void Print(IReport report)
        {
            throw new NotImplementedException();
        }

        public void Print(IReport report, string printer)
        {
            throw new NotImplementedException();
        }

        public void SaveAsAsync(IReport report, string path)
        {
            if (!(report is ExcelData data)) return;

            ReportFactory.CreateReport(_templatesDirectory, data, path);
        }
    }
}
