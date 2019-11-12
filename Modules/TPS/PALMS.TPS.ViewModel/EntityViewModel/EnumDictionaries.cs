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
        [Description("Client Send")]  CSend= 2,
        [Description("Laundry Received")]  LReceived= 3,
        [Description("Laundry Conveyor")]  Conveyor= 4,
        [Description("Laundry Packed")]  Packed= 5,
        [Description("Laundry Send")]  LSend= 6,
        [Description("Client Received")]  CReceived= 7,
        [Description("Client Using")]  CUsing= 8,
    }
}
