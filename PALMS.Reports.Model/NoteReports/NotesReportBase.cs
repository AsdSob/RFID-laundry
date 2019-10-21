using System;
using System.Collections.Generic;
using System.Drawing;
using PALMS.Reports.Common;
using PALMS.Reports.Model.NoteReports;

namespace PALMS.Reports.Model.Notes
{
    public class NotesReportBase : IReport
    {
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
        public Bitmap ClientLogo { get; set; }
        public Bitmap PrimeLogo { get; set; }
        public string DepartmentName { get; set; }
        public string DeliveryType { get; set; }
        public string NoteNumber { get; set; }
        public string WaterMark { get; set; }

        public DateTime PrintDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime CollectionDate { get; set; }

        public double CollectionWeight { get; set; }
        public double DeliveryKg { get; set; }

        public string Comment { get; set; }

        public List<NotesReportNoteItem> Items { get; set; }

        public bool ShowItemPrice { get; set; }
        public bool ShowClientReceivedQty { get; set; }

        public NotesReportBase()
        {
            Items = new List<NotesReportNoteItem>();
        }
    }
}
