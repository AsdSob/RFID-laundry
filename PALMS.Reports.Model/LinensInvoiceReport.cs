using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using PALMS.Reports.Common;

namespace PALMS.Reports.Model
{
    public class LinensInvoiceReport : IReport
    {
        public DateTime CreateDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public double Total { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string VendorPhone { get; set; }
        public string VendorEmail { get; set; }
        public string VendorWebSite { get; set; }
        public List<LinensInvoiceItem> Items { get; set; }
        public string DeliveryType { get; set; }
        public int TotalQty { get; set; }

        public LinensInvoiceReport()
        {
            Items = new List<LinensInvoiceItem>();
        }
    }

    public class LinensInvoiceItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Total { get; set; }
    }
}
