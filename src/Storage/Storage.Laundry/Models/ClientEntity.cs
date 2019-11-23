using System;
using System.Collections.Generic;
using System.Text;
using Storage.Laundry.Models.Abstract;

namespace Storage.Laundry.Models
{
    public class ClientEntity : EntityBase
    {
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public bool Active { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
    }
}
