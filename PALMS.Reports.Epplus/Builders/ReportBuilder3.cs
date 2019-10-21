using System;
using System.Text;
using OfficeOpenXml;
using PALMS.Reports.Epplus.Model;
using PALMS.Reports.Epplus.TemplateModel;

namespace PALMS.Reports.Epplus.Builders
{
    public class ReportBuilder3 : ReportBuilder
    {
        protected override TemplateSettings TemplateSettings => new TemplateSettings
        {
            Notes = new Cell(8, 4),
            NotesShift = 3,
            MonthDays = new Cell(6, 7),
            GroupName = new Cell(8, 2),
            Price = new Cell(10, 4),
            GroupFirst = new Cell(8, 1),
            GroupLast = new Cell(11, 105),
            Total = new Cell(12, 7),
            TotalQuantity = new Cell(12, 100),
            TotalAmount = new Cell(12, 103)
        };

        public ReportBuilder3(ExcelWorksheet worksheet, ExcelData excelData)
            : base(worksheet, excelData)
        {

        }

        public override ReportBuilder SetDays()
        {
            var settings = TemplateSettings;

            var monthDayRow = settings.MonthDays.Row;
            var monthDayRowCol = settings.MonthDays.Col;

            var j = 0;
            for (var i = 0; i < Data.Days.Length; i++)
            {
                var value = String.Format("{0:dd MMM }", Data.Days[i]);

                Sheet.Cells[monthDayRow, j + monthDayRowCol].Value = value;
                j += settings.NotesShift;
            }

            return this;
        }

        protected override void InsertRows()
        {
            var settings = TemplateSettings;
            var totalRow = settings.Total.Row;

            var itemRange = GetRange(new Cell(settings.GroupFirst.Row + 2, settings.GroupFirst.Col),
                new Cell(settings.GroupFirst.Row + 2, settings.GroupLast.Col));

            // add for first (default) group
            InsertRows(ref totalRow, Data.Groups[0].Items, itemRange);
            SetSubTotalFormula(totalRow - 1);

            if (Data.Groups.Count > 1)
            {
                var rangeRowCount = settings.GroupLast.Row - settings.GroupFirst.Row + 1;

                // add 2+ groups
                for (int i = 1; i < Data.Groups.Count; i++)
                {
                    totalRow += rangeRowCount;

                    InsertRows(ref totalRow, Data.Groups[i].Items, itemRange);
                    SetSubTotalFormula(totalRow - 1);
                }
            }
        }

        protected override void SetValues(AnnexGroupItem annexGroupItem, int itemRow, int number)
        {
            var itemNameCol = TemplateSettings.GroupName.Col;
            var priceCol = TemplateSettings.Price.Col;
            var col = priceCol;

            // set number
            Sheet.Cells[itemRow, itemNameCol - 1].Value = number;

            // set name
            Sheet.Cells[itemRow, itemNameCol].Value = annexGroupItem.Name;

            // set price
            Sheet.Cells[itemRow, col].Value = annexGroupItem.PriceDryCleaning;
            Sheet.Cells[itemRow, ++col].Value = annexGroupItem.PriceLaundry;
            Sheet.Cells[itemRow, ++col].Value = annexGroupItem.PricePressing;

            for (var i = 1; i <= annexGroupItem.Value1.Length; i++)
            {
                Sheet.Cells[itemRow, ++col].Value = annexGroupItem.Value1[i - 1];
                Sheet.Cells[itemRow, ++col].Value = annexGroupItem.Value2[i - 1];
                Sheet.Cells[itemRow, ++col].Value = annexGroupItem.Value3[i - 1];
            }
        }

        protected override void SetColTotal(int totalRow, int totalCol, int[] groupSubTotals)
        {
            var col = totalCol;

            for (var i = 0; i < Data.Groups[0].Notes.Length; i++)
            {
                Sheet.Cells[totalRow, col].Formula = GetTotalFormula(groupSubTotals, col);
                col++;
                Sheet.Cells[totalRow, col].Formula = GetTotalFormula(groupSubTotals, col);
                col++;
                Sheet.Cells[totalRow, col].Formula = GetTotalFormula(groupSubTotals, col);
                col++;
            }
        }

        protected override ReportBuilder SetTotalsFormulas()
        {
            // = SUMPRODUCT((MOD(COLUMN(A5: L5) - COLUMN(A5), 3) = 0) * 1, A5: L5)

            //var row = TemplateSettings.GroupFirst.Row + 2;
            //var col = TemplateSettings.TotalQuantity.Col;

            //for (int j = 0; j < 3; j++)
            //{
            //    var firstCell = new StringBuilder();
            //    var lastCell = new StringBuilder();

            //    firstCell.Append($"{Sheet.Cells[row, TemplateSettings.Notes.Col + TemplateSettings.NotesShift + j].Address}");
            //    lastCell.Append($"{Sheet.Cells[row, col - 1].Address}");

            //    Sheet.Cells[row, col + j].Formula = GetFormula(firstCell.ToString() , lastCell.ToString());
            //}

            return this;

            //string GetFormula(string first, string last)
            //{
            //    return $"= SUMPRODUCT((MOD(COLUMN({first}:{last}) - COLUMN({first}), 3) = 0) * 1, {first}:{last})";
            //}
        }

        protected override void SetQuantityTotal(int totalRow, int totalCol)
        {

        }

        // Grand Total
        protected override void SetAmountTotal(int totalRow, int[] groupSubTotals)
        {
            var col = TemplateSettings.TotalAmount.Col -1;
            var rows = groupSubTotals;

            // Set total Amount
            for (int j = 0; j < 3; j++)
            {
                col ++;

                var builder = new StringBuilder();
                builder.Append("=SUM(");

                for (int i = 0; i < rows.Length; i++)
                {
                    var address = Sheet.Cells[rows[i], col].Address;

                    builder.Append($"{address},");
                }

                builder.Remove(builder.Length - 1, 1); // remove last ','
                builder.Append(")");
                Sheet.Cells[totalRow, col].Formula = builder.ToString();
            }

            // Set total Quantity

            col = TemplateSettings.TotalAmount.Col - 4;
            rows = groupSubTotals;

            for (int j = 0; j < 3; j++)
            {
                col++;

                var builder = new StringBuilder();
                builder.Append("=SUM(");

                for (int i = 0; i < rows.Length; i++)
                {
                    var address = Sheet.Cells[rows[i], col].Address;

                    builder.Append($"{address},");
                }

                builder.Remove(builder.Length - 1, 1); // remove last ','
                builder.Append(")");
                Sheet.Cells[totalRow, col].Formula = builder.ToString();
            }
        }

        protected override ReportBuilder RemoveColumns()
        {
            var lastDay = Data.Groups[0].Notes.Length;
            if (lastDay >= 31) return this;

            var count = Math.Max(lastDay, 1);

            var lastCol = TemplateSettings.TotalQuantity.Col;

            while (count++ < 31)
            {
                Sheet.DeleteColumn(--lastCol);
                Sheet.DeleteColumn(--lastCol);
                Sheet.DeleteColumn(--lastCol);
            }

            return this;
        }
    }
}