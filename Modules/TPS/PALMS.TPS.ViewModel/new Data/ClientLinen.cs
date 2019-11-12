using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.TPS.ViewModel.new_Data
{
    public class ClientLinen : Entity 
    {
        public int MasterLinenId { get; set; }
        public int StaffId { get; set; }
        public string Rfid { get; set; }
        public int? StatusId { get; set; }
    }
}
