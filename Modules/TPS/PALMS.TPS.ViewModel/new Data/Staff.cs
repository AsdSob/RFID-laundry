using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;using PALMS.Data.Objects.EntityModel;

namespace PALMS.TPS.ViewModel.new_Data
{
    public class Staff: NameEntity
    {
        public string StaffId { get; set; }
        public int DepartmentId { get; set; }

    }
}
