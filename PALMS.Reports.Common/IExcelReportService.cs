using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PALMS.Reports.Common
{
    public interface IExcelReportService
    {
        void SaveAsAsync(IReport report, string path);
    }
}
