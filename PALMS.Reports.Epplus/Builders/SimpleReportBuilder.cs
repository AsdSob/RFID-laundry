using System.Collections.Generic;
using OfficeOpenXml;
using PALMS.Reports.Epplus.BuilderSettings;
using PALMS.Reports.Epplus.Model;
using PALMS.Reports.Epplus.TemplateModel;

namespace PALMS.Reports.Epplus.Builders
{
    public class SimpleReportBuilder
    {
        protected readonly ExcelWorksheet Sheet;
        protected readonly ExcelData Data;
        public int HorCount { get; set; }

        protected virtual SimpleExcelSettings SimpleSettings
        {
            get => new SimpleExcelSettings
            {
            };
        }

        public SimpleReportBuilder(ExcelWorksheet worksheet, ExcelData excelData)
        {
            Sheet = worksheet;
            Data = excelData;
            HorCount = excelData.HorizontalData.Length;
        }

        public virtual void Build()
        {
            SetHeader()
                .AddHor()
                .AddVer();
        }

        public virtual SimpleReportBuilder SetHeader()
        {
            var settings = SimpleSettings;
            var data = Data;

            Sheet.Cells[settings.ReportName.Row, settings.ReportName.Row].Value = data.Name;
            Sheet.Cells[settings.Date.Row, settings.Date.Row].Value = data.PrintDate.ToString();

            return this;
        }
        public virtual SimpleReportBuilder AddHor()
        {
            var settings = SimpleSettings;
            var first = settings.HorizontalFirst;

            var data = Data;

            var itemRange = GetRange(first, new Cell(first.Row + 2, first.Col));

            //copy styles from template to new col
            for (int i = 0; i < HorCount; i++)
            {
                var insertRange = GetRange(new Cell(first.Row, first.Col +i), new Cell(first.Row +2, first.Col +i));
                itemRange.Copy(insertRange);

                Sheet.Cells[first.Row, first.Col + i].Value = data.HorizontalData[i];
            }
            return this;
        }

        public virtual SimpleReportBuilder AddVer()
        {
            var settings = SimpleSettings;
            var first = settings.VerticalFirstGroup;
            var lastGroupRow = 6;

            var itemRange = GetRange(first, new Cell(first.Row, first.Col + HorCount));

            for (int i = 0; i < Data.VerticalData.Count; i++)
            {
                var insertRange = GetRange(new Cell(lastGroupRow, first.Col), new Cell(lastGroupRow, first.Col + HorCount));
                itemRange.Copy(insertRange);

                Sheet.Cells[lastGroupRow, first.Col].Value = Data.VerticalData[i].Name;

                AddVerItems(Data.VerticalData[i].VerticalData, lastGroupRow +1);

                lastGroupRow += Data.VerticalData[i].VerticalData.Count+1;
            }

            return this;
        }

        public void AddVerItems(List<CoordinateGroupItem> items, int lastGroupRow)
        {
            var settings = SimpleSettings;
            var first = settings.VerticalFirstItem;
            var itemRange = GetRange(first, new Cell(first.Row, first.Col + HorCount));
            var id = 1;

            foreach (var item in items)
            {
                var insertRange = GetRange(new Cell(lastGroupRow, first.Col), new Cell(lastGroupRow, first.Col + HorCount));
                itemRange.Copy(insertRange);

                Sheet.Cells[lastGroupRow, first.Col-1].Value = id++;
                Sheet.Cells[lastGroupRow, first.Col].Value = item.Name;

                for (int j = 0; j < HorCount; j++)
                {
                    Sheet.Cells[lastGroupRow, first.Col + j + 1].Value =item.Values[j];
                }

                lastGroupRow++;
            }
        }

        protected ExcelRange GetRange(Cell first, Cell last)
        {
            return Sheet.Cells[first.Row, first.Col,
                last.Row, last.Col];
        }
    }
}
