namespace PALMS.Reports.Epplus.TemplateModel
{
    public class TemplateSettings
    {
        public Cell Name { get; set; } = new Cell(1, 3);
        public Cell Description { get; set; } = new Cell(3, 3);
        public Cell Month { get; set; } = new Cell(2, 3);
        public Cell NoteType { get; set; } = new Cell(4, 3);

        public Cell GroupFirst { get; set; }
        public Cell GroupLast { get; set; }
        public Cell Notes { get; set; }
        public Cell MonthDays { get; set; }
        public Cell Total { get; set; }
        public Cell TotalQuantity { get; set; }
        public Cell TotalAmount { get; set; }
        public int NotesShift { get; set; }
        public Cell Price { get; set; }
        public Cell GroupName { get; set; }
    }
}