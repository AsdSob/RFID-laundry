using System.Text;
using OfficeOpenXml;
using PALMS.Reports.Epplus.BuilderSettings;
using PALMS.Reports.Epplus.Model;
using PALMS.Reports.Epplus.TemplateModel;

namespace PALMS.Reports.Epplus.Builders
{
    public class ReportBuilderAnnex1
    {
        protected readonly ExcelWorksheet Sheet;
        protected readonly ExcelData Data;
        protected AnnexTemp AnnexTemp;

        protected virtual AnnexSettings AnnexSettings
        {
            get => new AnnexSettings
            {
                Shift = 1,
                Days = Data.Days.Length,
                GroupFirst = new Cell(7, 1),
                GroupLast = new Cell(11, 1),

                DayFirst = new Cell(5,6),
            };
        }

        public ReportBuilderAnnex1(ExcelWorksheet worksheet, ExcelData excelData)
        {
            Sheet = worksheet;
            Data = excelData;
            AnnexTemp = new AnnexTemp();
        }

        public virtual void Build()
        {
            SetHeader()
                .SetDays()
                .AddGroups()
                .SetGroupTotal()
                .SetTotalsQty();
        }

        public ReportBuilderAnnex1 SetHeader()
        {
            var settings = AnnexSettings;

            Sheet.Cells[settings.Name.Row, settings.Name.Col].Value = Data.Name;
            Sheet.Cells[settings.Description.Row, settings.Description.Col].Value = Data.Description;
            Sheet.Cells[settings.Month.Row, settings.Month.Col].Value = Data.Month;

            return this;
        }

        protected ExcelRange GetRange(Cell first, Cell last)
        {
            return Sheet.Cells[first.Row, first.Col,
                last.Row, last.Col];
        }

        public virtual ReportBuilderAnnex1 SetDays()
        {
            InsertColumns();

            var settings = AnnexSettings;

            var dayRow = settings.DayFirst.Row;
            var dayRowCol = settings.DayFirst.Col;

            //Insert Value1 of Days
            var j = 0;
            for (var i = 0; i < settings.Days; i++)
            {
                var value = string.Format("{0:dd MMM }", Data.Days[i] );

                Sheet.Cells[dayRow, dayRowCol + j].Value = value;
                j += settings.Shift;
            }

            AnnexTemp.DayFirst = settings.DayFirst;
            AnnexTemp.DayLast = new Cell(settings.DayFirst.Row, settings.DayFirst.Col + (settings.Days-1) * settings.Shift);
            AnnexTemp.LastColumn = (AnnexTemp.DayLast.Col + 2) * settings.Shift;
            return this;
        }

        public void InsertColumns()
        {
            var settings = AnnexSettings;
            var first = settings.DayFirst;

            Sheet.InsertColumn(first.Col+settings.Shift, (settings.Days - 1) * settings.Shift);

            var itemRange = GetRange(first, new Cell(13, first.Col +settings.Shift -1) );

            //copy styles from template to new col
            var j = 0;
            for (int i = 0; i < settings.Days; i++)
            {
                var insertRange = GetRange(new Cell(first.Row, first.Col + j), new Cell(13, first.Col + j));
                j += settings.Shift;

                itemRange.Copy(insertRange);
            }
        }

        public virtual ReportBuilderAnnex1 AddGroups()
        {
            var setting = AnnexSettings;
            var lastCol = AnnexTemp.LastColumn;
            var lastRow = setting.GroupLast.Row;

            //Add first Group

            var itemRange = GetRange(new Cell(setting.GroupFirst.Row, 1), new Cell(setting.GroupLast.Row, lastCol));

            for (var i = 1; i < Data.Groups.Count; i++)
            {
                Sheet.InsertRow(lastRow + 1, 5);
                itemRange.Copy(GetRange(new Cell(lastRow +1, 1), new Cell(lastRow + 5, lastCol)));

                lastRow += 5;
            }

            //Insert Group Value1 + GroupItem Value1

            for (var i = Data.Groups.Count-1; i >= 0; i--)
            {
                var firstRow = lastRow - 4;

                InsertRows(firstRow + 2, Data.Groups[i]);

                lastRow += Data.Groups[i].Items.Count - 1;

                SetGroupInfo(firstRow, lastRow, Data.Groups[i]);
                SetColumnTotalFormula(new Cell(firstRow + 2, AnnexTemp.DayFirst.Col),
                    new Cell(lastRow - 2, AnnexTemp.DayLast.Col));
                SetGroupTotalFormula(new Cell(firstRow + 2, AnnexTemp.DayFirst.Col),
                    new Cell(lastRow - 2, AnnexTemp.DayLast.Col));


                lastRow = firstRow - 1;
            }
            return this;
        }

        public ReportBuilderAnnex1 SetGroupTotal()
        {
            var settings = AnnexSettings;
            var firstRow = settings.GroupFirst.Row;

            AnnexTemp.Groups = new Group[Data.Groups.Count];

            for (int i = 0; i < Data.Groups.Count; i++)
            {
                var lastRow = firstRow + 4 + Data.Groups[i].Items.Count -1;
                AnnexTemp.Groups[i] = new Group
                {
                    GroupFirst = new Cell(firstRow, 1),
                    GroupLast = new Cell(lastRow, 1)
                };

                firstRow = lastRow + 1;
            }
            return this;
        }

        public virtual ReportBuilderAnnex1 SetTotalsQty()
        {
            var settings = AnnexSettings;
            var dayFirst = AnnexTemp.DayFirst;
            var dayLast = AnnexTemp.DayLast;
            var totalKgRow = AnnexTemp.Groups[AnnexTemp.Groups.Length - 1].GroupLast.Row + 2;
            var totalPcRow = AnnexTemp.Groups[AnnexTemp.Groups.Length - 1].GroupLast.Row + 1;

            //Total Columns
            for (int i = dayFirst.Col; i <= dayLast.Col + 2*settings.Shift; i++)
            {
                var coordinatesKg = new StringBuilder();
                var coordinatesPcs = new StringBuilder();
                for (int j = 0; j < AnnexTemp.Groups.Length; j++)
                {
                    coordinatesKg.Append(Sheet.Cells[AnnexTemp.Groups[j].GroupLast.Row, i].Address + ",");
                    coordinatesPcs.Append(Sheet.Cells[AnnexTemp.Groups[j].GroupLast.Row - 1, i].Address + ",");
                }
                SetTotalSumRandom(new Cell(totalKgRow, i), coordinatesKg);
                SetTotalSumRandom(new Cell(totalPcRow, i), coordinatesPcs);
            }
            return this;
        }

        public virtual void InsertRows(int firstRow, AnnexGroupData annexGroupData)
        {
            var setting = AnnexSettings;
            var lastCol = AnnexTemp.DayLast.Col + ( 3 * setting.Shift) ;
            var data = annexGroupData;

            Sheet.InsertRow(firstRow + 1, data.Items.Count -1);

            var itemRange = GetRange(new Cell(firstRow, 1), new Cell(firstRow, lastCol));

            //copy styles from template to new row
            var j = 0;
            for (int i = 0; i < data.Items.Count; i++)
            {
                var insertRange = GetRange(new Cell(firstRow +i, 1), new Cell(firstRow +i, lastCol));

                //if(i != 0)
                itemRange.Copy(insertRange);

                SetRowValue(annexGroupData.Items[i], firstRow +i, ++j);
            }
        }

        public virtual void SetRowValue(AnnexGroupItem item, int itemRow, int id)
        {
            var setting = AnnexSettings;

            Sheet.Cells[itemRow, 1].Value = id; 
            Sheet.Cells[itemRow, 2].Value = item.Name; 
            Sheet.Cells[itemRow, 4].Value = item.PriceLaundry;

            for (int i = 0; i < setting.Days; i++)
            {
                Sheet.Cells[itemRow, setting.DayFirst.Col + i].Value = item.Value3[i];
            }

            SetTotalSum(
                new Cell(itemRow, AnnexTemp.DayLast.Col +1),
                new Cell(itemRow, setting.DayFirst.Col),
                new Cell(itemRow, AnnexTemp.DayLast.Col)
            );
        }

        public virtual void SetGroupInfo(int firstRow, int endRow, AnnexGroupData dataAnnexGroup)
        {
            var setting = AnnexSettings;

            Sheet.Cells[firstRow, 2].Value = dataAnnexGroup.Name;
            Sheet.Cells[endRow, 4].Value = dataAnnexGroup.PricePerKg;

            var j = 0;
            for (int i = 0; i < setting.Days; i++)
            {
                Sheet.Cells[firstRow, setting.DayFirst.Col + j].Value = dataAnnexGroup.Notes[i];
                Sheet.Cells[endRow, setting.DayFirst.Col + j].Value = dataAnnexGroup.WeightDelivery[i];
                j += setting.Shift;
            }
        }

        public virtual void SetGroupTotalFormula(Cell first, Cell last)
        {

            //Set Group Total Qty (Pcs)
            var cor = new Cell(last.Row + 1, last.Col + 1);
            var start = new Cell(first.Row, last.Col +1);
            SetTotalVertical(cor, start);

            //Set Group Total Amount (Pcs)
            cor = new Cell(last.Row + 1, last.Col + 2);
            start = new Cell(first.Row, last.Col + 2);
            SetTotalVertical(cor, start);

            //Set Group Total Qty (Kg)
            cor = new Cell(last.Row + 2, last.Col + 1);
            start = new Cell(last.Row + 2, first.Col);
            SetTotalHorizontal(cor, start);
        }

        public virtual void SetTotalVertical(Cell cor, Cell start)
        {
            var to = new Cell(cor.Row - 1, cor.Col);
            SetTotalSum(cor, start, to);
        }

        public virtual void SetTotalHorizontal(Cell cor, Cell start)
        {
            var to = new Cell(cor.Row, cor.Col - 1);
            SetTotalSum(cor, start, to);
        }

        public virtual void SetColumnTotalFormula(Cell first, Cell last)
        {
            var settings = AnnexSettings;

            for (int i = first.Col; i <= last.Col + settings.Shift -1; i++)
            {
                var cor = new Cell(last.Row +1, i);
                var from = new Cell(first.Row, i);
                var to = new Cell(last.Row, i);
                SetTotalSum(cor, from, to);
            }
        }

        public virtual void SetTotalSum(Cell cor, Cell from, Cell to)
        {
            var builder = new StringBuilder();

            builder.Append("=SUM(");

            builder.Append(Sheet.Cells[from.Row, from.Col].Address);
            builder.Append(":");
            builder.Append(Sheet.Cells[to.Row, to.Col].Address);
            builder.Append(")");

            Sheet.Cells[cor.Row, cor.Col].Formula = builder.ToString();
        }

        public virtual void SetTotalSumRandom(Cell cor, StringBuilder cordinate)
        {
            var builder = new StringBuilder();
            cordinate.Remove(cordinate.Length - 1, 1);

            builder.Append("=SUM(" + cordinate + ")");

            Sheet.Cells[cor.Row, cor.Col].Formula = builder.ToString();
        }

        public virtual void SetStepFormula(Cell cor, Cell from, Cell to, int steps)
        {
            var builder = new StringBuilder();
            var start = Sheet.Cells[from.Row, from.Col].Address;
            var end = Sheet.Cells[to.Row, to.Col].Address;

            builder.Append($"=SUMPRODUCT((MOD(COLUMN( {start}:{end} ) - COLUMN({start}), {steps}) = 0) * 1, {start}:{end})");

            Sheet.Cells[cor.Row, cor.Col].Formula = builder.ToString();
        }

        public virtual void SetTotalsFormula(int totalRow, int i, int rowNumb)
        {
            var builder = new StringBuilder();
            builder.Append("=SUM(");

            foreach (var group in AnnexTemp.Groups)
            {
                builder.Append(Sheet.Cells[group.GroupLast.Row - rowNumb, i].Address);
                builder.Append(",");
            }

            builder.Remove(builder.Length - 1, 1);
            builder.Append(")");

            Sheet.Cells[totalRow, i].Formula = builder.ToString();
        }

    }
}
