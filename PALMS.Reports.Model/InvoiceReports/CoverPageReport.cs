using System;
using System.Collections.Generic;
using System.Drawing;
using PALMS.Reports.Common;

namespace PALMS.Reports.Model.Invoice
{
    public class CoverPageReport : IReport
    {
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceStart { get; set; }
        public DateTime InvoiceEnd { get; set; }
        public DateTime InvoiceDate { get; set; }

        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
        public string ClientTRN { get; set; }

        public Bitmap Logo { get; set; }
        public string Address { get; set; }
        public string TRN { get; set; }

        public double Vat { get; set; }
        public string GrandTotalStr { get; set; }
        public double GrandTotal { get; set; }
        public double VatTotal { get; set; }
        public double AmountTotal { get; set; }

        public List<CoverPageItemReport> Items { get; set; }
    }
}
