using System.Collections.Generic;
using System.Collections.ObjectModel;
using PALMS.Reports.Model.Notes;

namespace PALMS.Reports.Model.NoteReports
{
    public class NoteByNoteReport : NotesReportBase
    {
        public ObservableCollection<NotesReportRowItem> ReportNoteRows { get; set; }

        public NoteByNoteReport()
        {
            ReportNoteRows = new ObservableCollection<NotesReportRowItem>();
        }

        public NoteByNoteReport(NotesReportBase notesReportBase)
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

            Items = new List<NotesReportNoteItem>();
            ReportNoteRows = new ObservableCollection<NotesReportRowItem>();
        }
    }
}
