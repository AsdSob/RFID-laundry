using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PALMS.TPS.ViewModel.EntityViewModel
{

    public enum LinenStatus
    {
        [Description("Soiled")]  Soiled= 1,
        [Description("Collection")]  Collection= 2,
        [Description("Received")]  Received= 3,
        [Description("Washing")]  Washing= 4,
        [Description("Conveyor")]  Conveyor= 5,
        [Description("Packed")]  Packed= 6,
        [Description("Ready")]  Ready= 7,
        [Description("Send")]  Send= 8,
        [Description("In Use")]  InUse= 9,
    }
}
