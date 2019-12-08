using System.ComponentModel;

namespace PALMS.ViewModels.Common.Enumerations
{
    public enum FeeUnitEnum
    {
        [Description("%")] Percentage = 1,
        [Description("AED")] AED = 2,
    }

    public enum LinenUnitEnum
    {
        [Description("Piece")] Piece = 1,
        [Description("Kg")] Kg = 2,
        [Description("Sqr/Meter")] SqrMeter = 3,
    }

    public enum NoteStatusEnum
    {
        [Description("New Note")] NewNote = 1,
        [Description("Collection Note")] CollectionNote = 2,
        [Description("Delivery Note")] DeliveryNote = 3,
        [Description("Client Note")] ClientNote = 4,
        [Description("Pre-Invoice")] PreInvoice = 5,
        [Description("Invoiced")] Invoiced = 6,
    }

    public enum DeliveryTypeEnum
    {
        [Description("Normal")] Normal = 1,
        [Description("Express")] Express = 2,
        [Description("Re-Wash")] ReWash = 3,
    }

    public enum ServiceTypeEnum
    {
        [Description("Laundry")] Laundry = 1,
        [Description("Pressing")] Pressing = 2,
        [Description("DryCleaning")] DryCleaning = 3,
    }

    public enum NoteReportTypesEnum
    {
        [Description("By Linen")] ByLinen = 1,

        [Description("By Total Piece")] ByTotalPiece = 2,

        [Description("By Department")] ByDepartment = 3,
    }

    public enum LinenStatus
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

    public enum DepartmentType
    {
        [Description("Housekeeping")] Housekeeping = 1,
        [Description("Food & Beverage")] FnB = 2,
        [Description("Recreation")] Recreation = 3,
        [Description("Uniform")] Uniform = 4,
        [Description("Guest laundry")] GuestLaundry = 5,
    }

    public enum Cities
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