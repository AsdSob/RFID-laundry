using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConveyor.Svoyo
{
    public class EnumDictionaries
    {
        public enum LinenStatus
        {
            [Description("Soiled")] Soiled = 1,
            [Description("Collection")] Collection = 2,
            [Description("Received")] Received = 3,
            [Description("Washing")] Washing = 4,
            [Description("Conveyor")] Conveyor = 5,
            [Description("Packed")] Packed = 6,
            [Description("Ready")] Ready = 7,
            [Description("Send")] Send = 8,
            [Description("In Use")] InUse = 9,
        }

        public enum DepartmentType
        {
            [Description("Housekeeping")] Housekeeping = 1,
            [Description("Food & Beverage")] FnB = 2,
            [Description("Recreation")] Recreation = 3,
            [Description("Uniform")] Uniform = 4,
            [Description("Guest laundry")] GuestLaundry = 5,
        }

        public enum UAECity
        {
            [Description("Abu Dhabi")] AbuDhabi = 1,
            [Description("Dubai")] Dubai = 2,
            [Description("Sharjah")] Sharjah = 3,
            [Description("Ajman")] Ajman = 4,
            [Description("Ras Al Khaimah")] RAK = 5,
            [Description("Fujairah")] Fujairah = 6,
            [Description("Umm Al Quwain")] UAQ = 7,
        }

    }
}

