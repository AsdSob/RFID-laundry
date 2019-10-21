using System;
using System.Collections.Generic;
using PALMS.Reports.Common;
using PALMS.Reports.Model;
using PALMS.Reports.Model.Invoice;
using PALMS.Reports.Model.InvoiceReports;
using PALMS.Reports.Model.LabelReports;
using PALMS.Reports.Model.NoteReports;
using PALMS.Reports.Model.Notes;
using PALMS.Reports.Model.ReportTypes;
using PALMS.Reports.Xtra.Reports;
using PALMS.Reports.Xtra.Reports.InvoiceXtraReports;
using PALMS.Reports.Xtra.Reports.Labels;
using PALMS.Reports.Xtra.Reports.LabelXtraReports;
using PALMS.Reports.Xtra.Reports.NoteXtraReports;
using PALMS.Reports.Xtra.Reports.ReportTypes;

namespace PALMS.Reports.Xtra
{
    public static class ReportFactory
    {
        private static readonly Dictionary<Type, Type> Reports = new Dictionary<Type, Type>();

        static ReportFactory()
        {
            Reports.Add(typeof(LabelFullReport), typeof(LabelLogoXtraReport));
            Reports.Add(typeof(LabelNameOnlyReport), typeof(LabelNameOnlyXtraReport));

            Reports.Add(typeof(NoteReport), typeof(NoteXtraReport));
            Reports.Add(typeof(NoteByTotalReport), typeof(NoteByTotalXtraReport));
            Reports.Add(typeof(NoteByLinenReport), typeof(NoteByLinenXtraReport));
            Reports.Add(typeof(NoteByNoteReport), typeof(NoteByNoteXtraReport));

            Reports.Add(typeof(NotesReportNoteItem), typeof(NoteByNoteItemXtraReport));

            Reports.Add(typeof(CoverPageReport), typeof(CoverPageXtraReport));
            Reports.Add(typeof(AnnexTotalReport), typeof(AnnexTotalXtraReport));

            Reports.Add(typeof(LinenReportViewModel), typeof(LinenReportXtraReport));
            Reports.Add(typeof(ClientLinenPriceViewModel), typeof(ClientLinenPriceXtraReport));
            Reports.Add(typeof(NoteByStatusReportViewModel), typeof(NoteByStatusXtraReport));
            Reports.Add(typeof(LaundryKgReport), typeof(LaundryKgXtraReport));
        }

        public static IXtraReport Create(IReport data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var reportType = data.GetType();

            if (!Reports.TryGetValue(reportType, out var xtraReport))
                throw new ArgumentException($"Not registered report {reportType}");

            var report = (IXtraReport) Activator.CreateInstance(xtraReport);
            report.Initialize(data);

            return report;
        }
    }
}