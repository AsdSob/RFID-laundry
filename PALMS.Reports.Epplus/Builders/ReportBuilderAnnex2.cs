using OfficeOpenXml;
using PALMS.Reports.Epplus.BuilderSettings;
using PALMS.Reports.Epplus.Model;
using PALMS.Reports.Epplus.TemplateModel;

namespace PALMS.Reports.Epplus.Builders
{
    public class ReportBuilderAnnex2 : ReportBuilderAnnex1
    {
        protected override AnnexSettings AnnexSettings => new AnnexSettings
        {
            Shift = 2,
            Days = Data.Days.Length,
            GroupFirst = new Cell(7, 1),
            GroupLast = new Cell(11, 1),

            DayFirst = new Cell(5, 7),
        };

        public ReportBuilderAnnex2(ExcelWorksheet worksheet, ExcelData excelData) : base(worksheet, excelData)
        {
        }

        public override void SetRowValue(AnnexGroupItem item, int itemRow, int id)
        {
            var setting = AnnexSettings;

            Sheet.Cells[itemRow, 1].Value = id;
            Sheet.Cells[itemRow, 2].Value = item.Name;
            Sheet.Cells[itemRow, 4].Value = item.PriceLaundry;

            var j = 0;
            for (int i = 0; i < setting.Days; i++)
            {
                Sheet.Cells[itemRow, setting.DayFirst.Col + j].Value = item.Value1[i];
                Sheet.Cells[itemRow, setting.DayFirst.Col + j + 1].Value = item.Value3[i];

                j += setting.Shift;
            }

            SetStepFormula(
                new Cell(itemRow, AnnexTemp.DayLast.Col + 2),
                new Cell(itemRow, setting.DayFirst.Col),
                new Cell(itemRow, AnnexTemp.DayLast.Col + 1),
                setting.Shift
            );

            SetStepFormula(
                new Cell(itemRow, AnnexTemp.DayLast.Col + 3),
                new Cell(itemRow, setting.DayFirst.Col +1),
                new Cell(itemRow, AnnexTemp.DayLast.Col +1),
                setting.Shift
            );
        }

        public override void SetGroupTotalFormula(Cell first, Cell last)
        {
            var settings = AnnexSettings;

            //Set Group Total Qty (Pcs)
            var cor = new Cell(last.Row + 1, last.Col + 2);
            var start = new Cell(first.Row, last.Col + 2);
            SetTotalVertical(cor, start);

            //Set Group Total Qty (Pcs)
            cor = new Cell(last.Row + 1, last.Col + 3);
            start = new Cell(first.Row, last.Col + 3);
            SetTotalVertical(cor, start);

            //Set Group Total Amount (Pcs)
            cor = new Cell(last.Row + 1, last.Col + 4);
            start = new Cell(first.Row, last.Col + 4);
            SetTotalVertical(cor, start);

            //Set Group Total Qty (Kg)
            cor = new Cell(last.Row + 2, last.Col +2);
            start = new Cell(last.Row + 2, first.Col);
            SetStepFormula(cor, start, new Cell(cor.Row, cor.Col - 1), settings.Shift);

            //Set Group Total Qty (Kg)
            cor = new Cell(last.Row + 2, last.Col + 3);
            start = new Cell(last.Row + 2, first.Col +1);
            SetStepFormula(cor, start, new Cell(cor.Row, cor.Col - 2), settings.Shift);
        }

        public override void SetGroupInfo(int firstRow, int endRow, AnnexGroupData dataAnnexGroup)
        {
            var setting = AnnexSettings;

            Sheet.Cells[firstRow, 2].Value = dataAnnexGroup.Name;
            Sheet.Cells[endRow, 4].Value = dataAnnexGroup.PricePerKg;

            var j = 0;
            for (int i = 0; i < setting.Days; i++)
            {
                Sheet.Cells[firstRow, setting.DayFirst.Col + j].Value = dataAnnexGroup.Notes[i];
                Sheet.Cells[endRow, setting.DayFirst.Col + j].Value = dataAnnexGroup.WeightCollection[i];
                Sheet.Cells[endRow, setting.DayFirst.Col + j +1].Value = dataAnnexGroup.WeightDelivery[i];
                j += setting.Shift;
            }
        }

    }
}
