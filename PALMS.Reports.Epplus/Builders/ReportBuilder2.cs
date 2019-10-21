using System;
using System.Text;
using OfficeOpenXml;
using PALMS.Reports.Epplus.Model;
using PALMS.Reports.Epplus.TemplateModel;

namespace PALMS.Reports.Epplus.Builders
{
    public class ReportBuilder2 : ReportBuilder
    {
        protected override TemplateSettings TemplateSettings => new TemplateSettings
        {
            Notes = new Cell(8, 5),
            NotesShift = 2,
            MonthDays = new Cell(6, 5),
            GroupName = new Cell(8, 2),
            Price = new Cell(9, 4),
            GroupFirst = new Cell(8, 1),
            GroupLast = new Cell(10, 70),
            Total = new Cell(11, 5),
            TotalQuantity = new Cell(11, 68),
            TotalAmount = new Cell(11, 70)
        };

        public ReportBuilder2(ExcelWorksheet worksheet, ExcelData excelData)
            : base(worksheet, excelData)
        {

        }

        public override ReportBuilder SetDays()
        {
            var settings = TemplateSettings;

            var monthDayRow = settings.MonthDays.Row;
            var monthDayRowCol = settings.MonthDays.Col;

            var j = 1;
            for (var i = 0; i < Data.Days.Length; i++)
            {
                var value = String.Format("{0:dd MMM }", Data.Days[i]);

                Sheet.Cells[monthDayRow, j + monthDayRowCol].Value = value;
                j += settings.NotesShift;
            }

            return this;
        }

        public override ReportBuilder SetNotes()
        {
            var settings = TemplateSettings;

            var noteRow = settings.Notes.Row;
            var firstNoteCol = settings.Notes.Col;
            var groupRowsCount = settings.GroupLast.Row - settings.GroupFirst.Row + 1;

            foreach (var groupData in Data.Groups)
            {
                var j = 1;
                for (var i = 0; i < groupData.Notes.Length; i++)
                {
                    Sheet.Cells[noteRow, j + firstNoteCol].Value = groupData.Notes[i];
                    j += settings.NotesShift;
                }
                noteRow += groupRowsCount;
            }
            return this;
        }

        protected override void InsertRows()
        {
            var settings = TemplateSettings;
            var totalRow = settings.Total.Row;

            var itemRange = GetRange(new Cell(settings.GroupFirst.Row + 1, settings.GroupFirst.Col),
                new Cell(settings.GroupFirst.Row + 1, settings.GroupLast.Col +2));

            // add for first (default) group
            InsertRows(ref totalRow, Data.Groups[0].Items, itemRange);
            SetSubTotalFormula(totalRow -1);

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
            var col = TemplateSettings.Price.Col;

            // set number
            Sheet.Cells[itemRow, itemNameCol - 1].Value = number;

            // set name
            Sheet.Cells[itemRow, itemNameCol].Value = annexGroupItem.Name;

            // set price
            Sheet.Cells[itemRow, col].Value = annexGroupItem.PriceLaundry;

            col++;
            for (var i = 1; i <= annexGroupItem.Value1.Length; i++)
            {
                Sheet.Cells[itemRow, ++col].Value = annexGroupItem.Value1[i - 1];
                Sheet.Cells[itemRow, ++col].Value = annexGroupItem.Value3[i - 1];
            }
        }

        protected override void SetColTotal(int totalRow, int totalCol, int[] groupSubTotals)
        {
            var col = totalCol +1;

            for (var i = 0; i < Data.Groups[0].Notes.Length; i++)
            {
                Sheet.Cells[totalRow, col].Formula = GetTotalFormula(groupSubTotals, col);
                col++;
                Sheet.Cells[totalRow, col].Formula = GetTotalFormula(groupSubTotals, col);
                col++;
            }
        }

        protected override void SetQuantityTotal(int totalRow, int totalCol)
        {

        }

        protected override void SetAmountTotal(int totalRow, int[] groupSubTotals)
        {
            var col = TemplateSettings.TotalAmount.Col - 5;
            var rows = groupSubTotals;

            // Set total Quantity

            col = TemplateSettings.TotalAmount.Col - 4;
            rows = groupSubTotals;

            for (int j = 0; j < 4; j++)
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
            }

            return this;
        }
    }
}
