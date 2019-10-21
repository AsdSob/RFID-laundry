using System;
using System.Collections.Generic;
using PALMS.Reports.Common;

namespace PALMS.Reports.Model.NoteReports
{
    public class NotesReportNoteItem : IReport
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public double CollectionQuantity { get; set; }
        public double DeliveryQuantity { get; set; }
        public double? ClientReceivedQuantity { get; set; }
        public double CollectionNoteWeight { get; set; }
        public double DeliveryNoteWeight { get; set; }
        public string DeliveryType { get; set; }

        public double Price { get; set; }

        public DateTime CollectionDate { get; set; }
        public DateTime DelivereddDate { get; set; }

        public string Comment { get; set; }


        public List<NotesReportRowItem> Items { get; set; }

    }
}
