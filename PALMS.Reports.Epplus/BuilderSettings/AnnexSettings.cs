using PALMS.Reports.Epplus.TemplateModel;

namespace PALMS.Reports.Epplus.BuilderSettings
{
    public class AnnexSettings
    {
        public Cell Name { get; set; } = new Cell(1, 3);
        public Cell Month { get; set; } = new Cell(2, 3);
        public Cell Description { get; set; } = new Cell(3, 3);

        public Cell GroupFirst { get; set; }
        public Cell GroupLast { get; set; }

        public Cell DayFirst { get; set; }
        public int Shift { get; set; }
        public int Days { get; set; }
    }

}
