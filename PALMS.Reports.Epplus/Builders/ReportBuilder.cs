using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using PALMS.Reports.Epplus.Model;
using PALMS.Reports.Epplus.TemplateModel;

namespace PALMS.Reports.Epplus.Builders
{
    public class ReportBuilder
    {
        protected readonly ExcelWorksheet Sheet;
        protected readonly ExcelData Data;


        protected virtual TemplateSettings TemplateSettings
        {
            get => new TemplateSettings
            {
                NotesShift = 1,
                MonthDays = new Cell(5, 8),
                

                GroupName = new Cell(7, 2),
                Price = new Cell(8, 4),
                GroupFirst = new Cell(8, 1),
                GroupLast = new Cell(10, 37),

                Total = new Cell(11, 5),

                TotalQuantity = new Cell(11, 5),
                TotalAmount = new Cell(11, 6),
            };
        }

        public ReportBuilder(ExcelWorksheet worksheet, ExcelData excelData)
        {
            Sheet = worksheet;
            Data = excelData;
        }

        public virtual void Build()
        {
            SetHeader()
                .SetDays()
                .AddGroups()
                .SetNotes()
                .AddRows()
                .SetTotals()
                .RemoveColumns()
                .SetTotalsFormulas();
        }

        public virtual ReportBuilder SetDays()
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

        protected virtual ReportBuilder SetTotalsFormulas()
        {
            return this;
        }

        public ReportBuilder SetHeader()
        {
            var settings = TemplateSettings;

            Sheet.Cells[settings.Name.Row, settings.Name.Col].Value = Data.Name;
            Sheet.Cells[settings.Description.Row, settings.Description.Col].Value = Data.Description;
            Sheet.Cells[settings.Month.Row, settings.Month.Col].Value = Data.Month;
            Sheet.Cells[settings.NoteType.Row, settings.NoteType.Col].Value = Data.NoteType;
            Sheet.Cells[settings.MonthDays.Row, settings.MonthDays.Col].Value = Data.Days;

            return this;
        }

        public virtual ReportBuilder SetNotes()
        {
            var settings = TemplateSettings;

            var noteRow = settings.Notes.Row;
            var firstNoteCol = settings.Notes.Col;
            var groupRowsCount = settings.GroupLast.Row - settings.GroupFirst.Row + 1;

            foreach (var groupData in Data.Groups)
            {
                var j = 0;
                for (var i = 0; i < groupData.Notes.Length; i++)
                {
                    j += settings.NotesShift;

                    Sheet.Cells[noteRow, j + firstNoteCol].Value = groupData.Notes[i];
                }

                noteRow += groupRowsCount;
            }
            return this;
        }

        public virtual ReportBuilder SetTotals()
        {
            var groupSubTotals = new int[Data.Groups.Count];

            var totalRow = TemplateSettings.Total.Row;
            var totalCol = TemplateSettings.Total.Col;

            var subTotalRow = TemplateSettings.GroupLast.Row;

            for (int i = 0; i < Data.Groups.Count; i++)
            {
                var groupRowSize = TemplateSettings.GroupLast.Row - TemplateSettings.GroupFirst.Row + 1;

                if (i > 0)
                {
                    // default group size
                    totalRow += groupRowSize;
                    subTotalRow += groupRowSize;
                }

                // items from group - default template row
                var itemsCount = Data.Groups[i].Items.Count;
                var addedItemsCount = itemsCount - 1;
                totalRow += addedItemsCount;

                subTotalRow += addedItemsCount;

                groupSubTotals[i] = subTotalRow;
            }

            // set totals by column
            SetColTotal(totalRow, totalCol, groupSubTotals);

            // set quantity total
            SetQuantityTotal(totalRow, totalCol);

            // set amount total
            SetAmountTotal(totalRow, groupSubTotals);

            return this;
        }

        protected virtual void SetColTotal(int totalRow, int totalCol, int[] groupSubTotals)
        {
            var columnsCount = Data.Groups[0].Notes.Length;
            for (var i = 0; i < columnsCount; i++)
            {
                var totalColNumber = i + totalCol;
                Sheet.Cells[totalRow, totalColNumber].Formula = GetTotalFormula(groupSubTotals, totalColNumber);
            }
        }

        protected virtual void SetQuantityTotal(int totalRow, int totalCol)
        {
            Sheet.Cells[totalRow, TemplateSettings.TotalQuantity.Col].Formula =
                $"=SUM({Sheet.Cells[totalRow, totalCol]}:{Sheet.Cells[totalRow, totalCol + Data.Groups[0].Notes.Length - 1]})";
        }

        protected virtual void SetAmountTotal(int totalRow, int[] groupSubTotals)
        {
            Sheet.Cells[totalRow, TemplateSettings.TotalAmount.Col].Formula =
                GetTotalFormula(groupSubTotals, TemplateSettings.TotalAmount.Col);
        }

        public ReportBuilder AddGroups()
        {
            InitGroups();

            return this;
        }

        public ReportBuilder AddRows()
        {
            InsertRows();

            return this;
        }

        protected virtual ReportBuilder RemoveColumns()
        {
            var lastDay = Data.Groups[0].Notes.Length;
            if (lastDay >= 31) return this;

            var count = Math.Max(lastDay, 1);

            var lastCol = TemplateSettings.TotalQuantity.Col;

            while (count++ < 31)
            {
                Sheet.DeleteColumn(--lastCol);
            }

            return this;
        }

        private void InitGroups()
        {
            var settings = TemplateSettings;
            var groupNameCol = TemplateSettings.GroupName.Col;
            var totalRow = settings.Total.Row;

            SetGroupInfo(new Cell(settings.GroupFirst.Row, groupNameCol), Data.Groups[0], 1);

            if (Data.Groups.Count > 1)
            {
                var groupRange = GetRange(settings.GroupFirst, settings.GroupLast);

                // add 2+ groups
                for (int i = 1; i < Data.Groups.Count; i++)
                {
                    var rangeRowCount = settings.GroupLast.Row - settings.GroupFirst.Row + 1;

                    Sheet.InsertRow(totalRow, rangeRowCount);

                    totalRow += rangeRowCount;

                    var range = GetRange(new Cell(totalRow - rangeRowCount, settings.GroupFirst.Col),
                        new Cell(totalRow - 2, settings.GroupLast.Col));

                    groupRange.Copy(range);

                    SetGroupInfo(new Cell(totalRow - rangeRowCount, groupNameCol), Data.Groups[i], i + 1);
                }
            }
        }

        private void SetGroupInfo(Cell groupNameGroup, AnnexGroupData dataAnnexGroup, int number)
        {
            Sheet.Cells[groupNameGroup.Row, groupNameGroup.Col - 1].Value = number;
            Sheet.Cells[groupNameGroup.Row, groupNameGroup.Col].Value = dataAnnexGroup.Name;
        }

        protected virtual void InsertRows()
        {
            var settings = TemplateSettings;
            var totalRow = settings.Total.Row;

            var itemRange = GetRange(new Cell(settings.GroupFirst.Row + 1, settings.GroupFirst.Col),
                new Cell(settings.GroupFirst.Row + 1, settings.GroupLast.Col));

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

        protected ExcelRange GetRange(Cell first, Cell last)
        {
            return Sheet.Cells[first.Row, first.Col,
                last.Row, last.Col];
        }

        protected virtual void InsertRows(ref int totalRow, IList<AnnexGroupItem> items, ExcelRange itemRange)
        {
            if (items.Count == 0) return;

            SetValues(items[0], totalRow - 2, 1);

            if (items.Count == 1)
            {
                // 1 row added in template with styles
                return;
            }

            var insertCount = items.Count - 1;

            for (int i = 1; i <= insertCount; i++)
            { 
                // inser new row
                Sheet.InsertRow(totalRow - 1, 1);

                // total row moved down
                totalRow++;


                // copy styles from template to new row
                itemRange.Copy(GetRange(new Cell(totalRow - 2, TemplateSettings.GroupFirst.Col),
                    new Cell(totalRow - 2, TemplateSettings.GroupLast.Col)));

                // set values
                SetValues(items[i], totalRow - 2, i + 1);
            }
        }

        protected virtual void SetValues(AnnexGroupItem annexGroupItem, int itemRow, int number)
        {
            var itemNameCol = TemplateSettings.GroupName.Col;
            var priceCol = TemplateSettings.Price.Col;
            var col = priceCol;

            // set number
            Sheet.Cells[itemRow, itemNameCol - 1].Value = number;

            // set name
            Sheet.Cells[itemRow, itemNameCol].Value = annexGroupItem.Name;

            // set price2
            Sheet.Cells[itemRow, col].Value = annexGroupItem.PriceLaundry;

            for (int i = 1; i <= annexGroupItem.Value3.Length; i++)
            {
                Sheet.Cells[itemRow, ++col].Value = annexGroupItem.Value3[i - 1];
            }
        }

        protected void SetSubTotalFormula(int subTotalRow)
        {
            // default template formula: =SUM(E9:E9) or =SUM(E9)
            // example result for 3 items: =SUM(E9:E11)

            var firstFormulaCol = TemplateSettings.Notes.Col;
            var lastFormulaCol = TemplateSettings.GroupLast.Col;

            for (int i = firstFormulaCol; i <= lastFormulaCol; i++)
            {
                var cell = Sheet.Cells[subTotalRow, i];
                var formula = cell.Formula;

                if (formula.Contains(":"))
                {
                    var cellValue = formula.Split(':')[1]; // with last bracket
                    var rowNumber = Regex.Match(cellValue, @"\d+").Value;
                    var fixRow = cellValue.Replace(rowNumber, (subTotalRow - 1).ToString());
                    formula = formula.Replace(cellValue, fixRow);

                    cell.Formula = formula;
                }
                else if (formula.Contains("("))
                {
                    var cellValue = formula.Split('(')[1].Split(')')[0];
                    var rowNumber = Regex.Match(cellValue, @"\d+").Value;
                    var col = cellValue.Substring(0, cellValue.Length - rowNumber.Length);
                    formula = formula.Replace(cellValue, $"{cellValue}:{col}{subTotalRow - 1}");

                    cell.Formula = formula;
                }
            }
        }

        protected string GetTotalFormula(int[] rows, int col)
        {
            var builder = new StringBuilder();

            builder.Append("=SUM(");

            var lastIndex = rows.Length - 1;

            for (int i = 0; i < rows.Length; i++)
            {
                var address = Sheet.Cells[rows[i], col].Address;
                builder.Append(address);

                if (i < lastIndex)
                    builder.Append(",");
            }

            builder.Append(")");

            return builder.ToString();
        }
    }
}