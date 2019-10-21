namespace PALMS.Reports.Model.NoteReports
{
    public class NotesReportRowItem
    {
        public int Id { get; set; }
        public int LinenListId { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public double CollectionQuantity { get; set; }
        public double DeliveryQuantity { get; set; }
        public double? ClientReceivedQuantity { get; set; }
        public string ServiceType { get; set; }
        public int ServiceTypeId { get; set; }

        public int NoteId { get; set; }
        public string DepartmentName { get; set; }

        public double Price { get; set; }
    }
}
