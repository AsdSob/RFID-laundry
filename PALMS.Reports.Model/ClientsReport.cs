using System.Collections.Generic;
using PALMS.Reports.Common;

namespace PALMS.Reports.Model
{
    public class ClientsReport : IReport
    {
        public List<ClientItem> Items { get; set; }

        public ClientsReport()
        {
            Items = new List<ClientItem>();
        }
    }

    public class ClientItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string City { get; set; }
        public int OrderCount { get; set; }
    }
}
