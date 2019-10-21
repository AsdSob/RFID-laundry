using System;
using System.Collections.Generic;
using PALMS.Reports.Common;

namespace PALMS.Reports.Model.ReportTypes
{
    public class ClientLinenPriceViewModel : IReport
    {
        public string ReportType { get; set; }
        public DateTime PrintDate { get; set; }

        public List<ReportClient> Clients { get; set; }
    }

    public class ReportClient
    {
        public string Name { get; set; }
        public double PricePerKg { get; set; }
        public List<Group> Groups { get; set; }
    }

    public class Group
    {
        public string Department { get; set; }
        public List<GroupItem> Items { get; set; }
    }
    public class GroupItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double PriceLaundry { get; set; }
        public double PricePressing { get; set; }
        public double PriceDryCleaning { get; set; }
    }
}
