using OfficeOpenXml;
using PALMS.Reports.Epplus.BuilderSettings;
using PALMS.Reports.Epplus.Model;
using PALMS.Reports.Epplus.TemplateModel;

namespace PALMS.Reports.Epplus.Builders
{
    public class ReportBuilderAnnex3 : ReportBuilderAnnex1
    {

        protected override AnnexSettings AnnexSettings => new AnnexSettings
        {
            Shift = 3,
            Days = Data.Days.Length,
            GroupFirst = new Cell(7, 1),
            GroupLast = new Cell(11, 1),

            DayFirst = new Cell(5, 8),
        };

        public ReportBuilderAnnex3(ExcelWorksheet worksheet, ExcelData excelData) : base(worksheet, excelData)
        {

        }

        public override void SetRowValue(AnnexGroupItem item, int itemRow, int id)
        {
            var setting = AnnexSettings;

            Sheet.Cells[itemRow, 1].Value = id;
            Sheet.Cells[itemRow, 2].Value = item.Name;
            Sheet.Cells[itemRow, 4].Value = item.PriceDryCleaning;
            Sheet.Cells[itemRow, 5].Value = item.PriceLaundry;
            Sheet.Cells[itemRow, 6].Value = item.PricePressing;

            var j = 0;
            for (int i = 0; i < setting.Days; i++)
            {
                Sheet.Cells[itemRow, setting.DayFirst.Col + j].Value = item.Value1[i];
                Sheet.Cells[itemRow, setting.DayFirst.Col + j + 1].Value = item.Value2[i];
                Sheet.Cells[itemRow, setting.DayFirst.Col + j + 2].Value = item.Value3[i];

                j += setting.Shift;
            }

            SetStepFormula(
                new Cell(itemRow, AnnexTemp.DayLast.Col + 3),
                new Cell(itemRow, setting.DayFirst.Col),
                new Cell(itemRow, AnnexTemp.DayLast.Col + 2),
                setting.Shift
            );
            SetStepFormula(
                new Cell(itemRow, AnnexTemp.DayLast.Col + 4),
                new Cell(itemRow, setting.DayFirst.Col + 1),
                new Cell(itemRow, AnnexTemp.DayLast.Col + 2),
                setting.Shift
            );
            SetStepFormula(
                new Cell(itemRow, AnnexTemp.DayLast.Col + 5),
                new Cell(itemRow, setting.DayFirst.Col + 2),
                new Cell(itemRow, AnnexTemp.DayLast.Col + 2),
                setting.Shift
            );

        }

        public override void SetGroupTotalFormula(Cell first, Cell last)
        {
            var settings = AnnexSettings;

            for (int i = 0; i < settings.Shift; i++)
            {
                //Set Group Total Qty (Pcs)
                var cor = new Cell(last.Row + 1, last.Col + 3 +i);
                var start = new Cell(first.Row, last.Col + 3 + i);
                SetTotalVertical(cor, start);

                //Set Group Total Amount (Pcs)
                cor = new Cell(last.Row + 1, last.Col + 6 + i);
                start = new Cell(first.Row, last.Col + 6 + i);
                SetTotalVertical(cor, start);

                //Set Group Total Qty (Kg)
                cor = new Cell(last.Row + 2, last.Col + 3 + i);
                start = new Cell(last.Row + 2, first.Col + i);
                SetStepFormula(cor, start, new Cell(cor.Row, cor.Col - 1 -i), settings.Shift);
            }
        }

        //public override void SetGroupInfo(int firstRow, int endRow, AnnexGroupData dataAnnexGroup)
        //{
        //    var setting = AnnexSettings;

        //    Sheet.Cells[firstRow, 2].Value = dataAnnexGroup.Name;
        //    Sheet.Cells[endRow, 4].Value = dataAnnexGroup.PricePerKg;


        //    var j = 0;
        //    for (int i = 0; i < setting.Days; i++)
        //    {
        //        Sheet.Cells[firstRow, setting.DayFirst.Col + j].Value = dataAnnexGroup.Notes[i];
        //        Sheet.Cells[endRow, setting.DayFirst.Col + j + 1].Value = dataAnnexGroup.WeightDelivery[i];
        //        j += setting.Shift;
        //    }
        //}
        public override ReportBuilderAnnex1 SetTotalsQty()
        {
            return base.SetTotalsQty();
        }
    }
}
