using PALMS.Reports.Epplus.TemplateModel;

namespace PALMS.Reports.Epplus.BuilderSettings
{
    public class SimpleExcelSettings
    {
        public Cell ReportName { get; set; } = new Cell(1, 2);
        public Cell Date { get; set; } = new Cell(2, 2);

        public Cell VerticalFirstItem { get; set; } = new Cell(7, 3);
        public Cell VerticalFirstGroup { get; set; } = new Cell(6, 3);
        public Cell HorizontalFirst { get; set; } = new Cell(5, 4);
    }
}
