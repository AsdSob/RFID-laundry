using System.Linq;
using System.Text;
using OfficeOpenXml;
using PALMS.Reports.Epplus.BuilderSettings;
using PALMS.Reports.Epplus.Model;
using PALMS.Reports.Epplus.TemplateModel;

namespace PALMS.Reports.Epplus.Builders
{
    public class RevenueReportBuilder
    {
        protected readonly ExcelWorksheet Sheet;
        protected readonly ExcelData Data;
        public int LastCol { get; set; }

        public Cell[] GroupCoordinates{ get; set; }

        protected virtual RevenueExcelSettings RevenueExcelSettings
        {
            get => new RevenueExcelSettings
            {
            };
        }

        public RevenueReportBuilder(ExcelWorksheet worksheet, ExcelData excelData)
        {
            Sheet = worksheet;
            Data = excelData;
            LastCol = Data.Days.Length * 4 + 3;
            GroupCoordinates = new Cell[Data.RevenueGroupDatas.Count];
        }

        public virtual void Build()
        {
            SetHeader();
            SetGroups();
        }

        public virtual RevenueReportBuilder SetHeader()
        {
            var settings = RevenueExcelSettings;
            var data = Data;
            var horCount = data.Days.Length;
            
            Sheet.Cells[settings.ReportName.Row, settings.ReportName.Col].Value = Data.Description;
            Sheet.Cells[settings.PrintDate.Row, settings.PrintDate.Col].Value = Data.PrintDate.ToString("dd-MMMM-yyyy");

            //copy date template
            var itemRange = GetRange(settings.Dates, new Cell(settings.HorizontalTotal.Row +3, settings.HorizontalTotal.Col + 3));

            for (int i = 1; i < horCount; i++)
            {
                var insertRange = GetRange(new Cell(3, 4 + i *4), new Cell(9, 7 + 4 * i));

                itemRange.Copy(insertRange);
            }

            for (int i = 0; i < horCount; i++)
            {
                Sheet.Cells[settings.Dates.Row, settings.Dates.Col + i *4].Value = data.Days[i].ToString("MMMM'/'yyyy");
            }
            return this;
        }

        public virtual RevenueReportBuilder SetGroups()
        {
            var settings = RevenueExcelSettings;
            var data = Data;
            var lastRow = settings.VerticalGroup.Row + 1;

            var itemRange = GetRange(new Cell(settings.VerticalGroup.Row, 1), new Cell(lastRow, LastCol));

            for (int i = 0; i < data.RevenueGroupDatas.Count; i++)
            {
                if (i == 0)
                {
                    GroupCoordinates[i] = new Cell(settings.VerticalGroup.Row, 1);
                    SetGroupValue(i);
                }
                else
                {
                    Sheet.InsertRow(lastRow , 3);

                    var insertRange = GetRange(new Cell(lastRow + 1, 1), new Cell(lastRow + 2 , LastCol));
                    itemRange.Copy(insertRange);

                    GroupCoordinates[i] = new Cell(lastRow + 1, 1);
                    SetGroupValue(i);
                }

                lastRow = +data.RevenueGroupDatas[i].Items.Count;
            }

            SetTotal();

            return this;
        }

        public virtual RevenueReportBuilder SetGroupValue(int i)
        {
            var data = Data;

            SetItem(i);

            Sheet.Cells[GroupCoordinates[i].Row, GroupCoordinates[i].Col +1].Value = data.RevenueGroupDatas[i].Name;
            SetGroupTotal(i);

            return this;
        }

        public virtual RevenueReportBuilder SetItem(int i)
        {
            var group = Data.RevenueGroupDatas[i];
            var groupCoor = GroupCoordinates[i];

            var itemRange = GetRange(new Cell(groupCoor.Row + 1, 1), new Cell(groupCoor.Row + 1, LastCol));
            Sheet.InsertRow(groupCoor.Row + 2, group.Items.Count - 1);

            for (int j = 0; j < group.Items.Count; j++)
            {
                var insertRange = GetRange(new Cell(groupCoor.Row + 1 + j, 1), new Cell(groupCoor.Row + 1 + j, LastCol));
                itemRange.Copy(insertRange);

                SetItemValue(groupCoor.Row + 1 + j, group.Items[j]);
            }

            return this;
        }

        public virtual RevenueReportBuilder SetItemValue (int row, RevenueGroupItem item)
        {
            Sheet.Cells[row, 1].Value = item.Id;
            Sheet.Cells[row, 2].Value = item.Name;

            for (int i = 0; i < Data.Days.Length; i++)
            {
                var j = i * 4;

                Sheet.Cells[row, 4 + j].Value = item.Amount[i];
                Sheet.Cells[row, 5 + j].Value = item.SoiledKg[i];
                Sheet.Cells[row, 6 + j].Value = item.CleanKg[i];
                Sheet.Cells[row, 7 + j].Value = item.SalesKg[i];
            }
            return this;
        }


        public virtual RevenueReportBuilder SetGroupTotal(int i)
        {
            var group = Data.RevenueGroupDatas[i];
            var groupCoor = GroupCoordinates[i];

            for (int j = 4; j <= LastCol; j++)
            {
                var builder = new StringBuilder();

                var firstAddress = Sheet.Cells[groupCoor.Row + 1, j].Address;
                var lastAddress = Sheet.Cells[groupCoor.Row + group.Items.Count, j].Address;

                builder.Append("=SUM(" + firstAddress + ":" + lastAddress + ")");

                Sheet.Cells[groupCoor.Row, j].Formula = builder.ToString();
            }

            return this;
        }

        public virtual RevenueReportBuilder SetTotal()
        {
            var totalRow = GroupCoordinates.Last().Row + Data.RevenueGroupDatas.Last().Items.Count + 2;

            for (int i = 4; i <= LastCol; i++)
            {
                var builder = new StringBuilder();
                builder.Append("=");

                foreach (var group in GroupCoordinates)
                {
                    builder.Append("+" + Sheet.Cells[group.Row, i].Address);
                }

                Sheet.Cells[totalRow, i].Formula = builder.ToString();
            }

            return this;
        }


        protected ExcelRange GetRange(Cell first, Cell last)
        {
            return Sheet.Cells[first.Row, first.Col,
                last.Row, last.Col];
        }
    }
}
