using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.TPS.ViewModel.new_Data
{
    public class ClientInfo : Entity
    {
        public string Address { get; set; }
        public string City { get; set; }
        public int ClientId { get; set; }
    }
}
