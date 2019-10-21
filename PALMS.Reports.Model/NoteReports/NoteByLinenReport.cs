using System.Collections.Generic;
using PALMS.Reports.Model.NoteReports;

namespace PALMS.Reports.Model.Notes
{
    public class NoteByLinenReport : NotesReportBase
    {

        public List<NotesReportRowItem> ItemRows { get; set; }

        public NoteByLinenReport()
        {
            ItemRows = new List<NotesReportRowItem>();
        }

        public NoteByLinenReport(NotesReportBase notesReportBase)
        {
            ClientName = notesReportBase.ClientName;
            ClientAddress = notesReportBase.ClientAddress;
            ClientLogo = notesReportBase.ClientLogo;
            PrimeLogo = notesReportBase.PrimeLogo;
            DepartmentName = notesReportBase.DepartmentName;
            DeliveryType = notesReportBase.DeliveryType;
            NoteNumber = notesReportBase.NoteNumber;
            PrintDate = notesReportBase.PrintDate;
            DeliveryDate = notesReportBase.DeliveryDate;
            CollectionDate = notesReportBase.CollectionDate;
            CollectionWeight = notesReportBase.CollectionWeight;
            DeliveryKg = notesReportBase.DeliveryKg;
            Comment = notesReportBase.Comment;
            ShowItemPrice = notesReportBase.ShowItemPrice;
        }
    }
}