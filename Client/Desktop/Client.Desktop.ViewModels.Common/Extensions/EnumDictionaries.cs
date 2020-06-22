using System.ComponentModel;

namespace Client.Desktop.ViewModels.Common.Extensions
{
    public enum LinenStatusEnum
    {
        [Description("Soiled")] Soiled = 1,
        [Description("Collection")] Collection = 2,
        [Description("Received")] Received = 3,
        [Description("Washing")] Washing = 4,
        [Description("Conveyor Hanged")] ConveyorHanged = 5,
        [Description("Conveyor Packed")] ConveyorPacked = 6,
        [Description("Ready")] Ready = 7,
        [Description("Send")] Send = 8,
        [Description("In Use")] InUse = 9,
    }

    public enum DepartmentTypeEnum
    {
        [Description("Housekeeping")] Housekeeping = 1,
        [Description("Food & Beverage")] FnB = 2,
        [Description("Recreation")] Recreation = 3,
        [Description("Uniform")] Uniform = 4,
        [Description("Guest laundry")] GuestLaundry = 5,
    }

    public enum CitiesEnum
    {
        [Description("Abu Dhabi")] AbuDhabi = 1,
        [Description("Dubai")] Dubai = 2,
        [Description("Sharjah")] Sharjah = 3,
        [Description("Ajman")] Ajman = 4,
        [Description("Ras Al Khaimah")] RAK = 5,
        [Description("Fujairah")] Fujairah = 6,
        [Description("Umm Al Quwain")] UAQ = 7,
    }

    public enum TextBoxMask
    {
        Decimal,
        Double,
    }
}
