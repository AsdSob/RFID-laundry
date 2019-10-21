using PALMS.Reports.Epplus.TemplateModel;

namespace PALMS.Reports.Epplus.BuilderSettings
{
    public class RevenueExcelSettings
    {
        public Cell ReportName { get; set; } = new Cell(3, 1);
        public Cell PrintDate { get; set; } = new Cell(1, 2);

        public Cell Dates { get; set; } = new Cell(3, 4);

        public Cell VerticalGroup { get; set; } = new Cell(6, 2);
        public Cell VerticalItem { get; set; } = new Cell(7, 1);

        public Cell HorizontalTotal { get; set; } = new Cell(6, 4);
        public Cell HorizontalValue { get; set; } = new Cell(7, 4);

        public Cell TotalSales { get; set; } = new Cell(9, 4);
    }
}
