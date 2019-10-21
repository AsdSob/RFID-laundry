using System;
using System.IO;
using PALMS.Reports.Common;
using PALMS.Reports.Epplus.Builders;
using PALMS.Reports.Epplus.Model;

namespace PALMS.Reports.Epplus.Services
{
    internal static class ReportFactory
    {
        internal static void CreateReport(string directory, ExcelData data, string resultPath)
        {
            switch (data.ReportType)
            {
                case ReportType.RevenueExcel:
                    using (var excelPackage = FileFactory.LoadPackage(GetPath(directory, "revenueTemplate.xlsx")))
                    {
                        var sheet = excelPackage.Workbook.Worksheets["Sheet1"];

                        new RevenueReportBuilder(sheet, data).Build();

                        excelPackage.SaveAs(new FileInfo(resultPath));
                    }
                    break;

                case ReportType.SimpleExcel:
                    using (var excelPackage = FileFactory.LoadPackage(GetPath(directory, "simpleExcel.xlsx")))
                    {
                        var sheet = excelPackage.Workbook.Worksheets["Sheet1"];

                        new SimpleReportBuilder(sheet, data).Build();

                        excelPackage.SaveAs(new FileInfo(resultPath));
                    }
                    break;

               case ReportType.AnnexTotal:
                    using (var excelPackage = FileFactory.LoadPackage(GetPath(directory, "annexTotal.xlsx")))
                    {
                        var sheet = excelPackage.Workbook.Worksheets["Sheet1"];

                        new ReportBuilderAnnex1(sheet, data).Build();

                        excelPackage.SaveAs(new FileInfo(resultPath));
                    }
                    break;

                case ReportType.AnnexCollection:
                    using (var excelPackage = FileFactory.LoadPackage(GetPath(directory, "annexCollection.xlsx")))
                    {
                        var sheet = excelPackage.Workbook.Worksheets["Sheet1"];

                        new ReportBuilderAnnex2(sheet, data).Build();

                        excelPackage.SaveAs(new FileInfo(resultPath));
                    }
                    break;

                case ReportType.AnnexService:
                    using (var excelPackage = FileFactory.LoadPackage(GetPath(directory, "annexService.xlsx")))
                    {
                        var sheet = excelPackage.Workbook.Worksheets["Sheet1"];

                        new ReportBuilderAnnex3(sheet, data).Build();

                        excelPackage.SaveAs(new FileInfo(resultPath));
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(data.ReportType), data.ReportType, null);
            }
        }

        private static string GetPath(string directory, string template)
        {
            var path = Path.Combine(directory, template);

            if (!File.Exists(path))
                throw new FileNotFoundException($"File template not found {path}");

            return path;
        }
    }
}