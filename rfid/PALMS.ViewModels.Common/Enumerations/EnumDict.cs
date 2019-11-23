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

    public enum NoteTypeEnum
    {
        [Description("Standard")] Standard = 1,

        [Description("Note With Price")] WithPrice = 2,
    }

    public enum InvoiceTypeEnum
    {
        [Description("Standard")] Standard = 1,
    }

    public enum LabelTypeEnum
    {
        [Description("Regular")] Regular = 1,
        [Description("Damage")] Damage = 2,
        [Description("New")] New = 3,
        [Description("Stain")] Stain = 4,
        [Description("Express")] Express = 5,
        [Description("Client Name")] Name = 6,
    }

    public enum TrackingStatusEnum
    {
        [Description("Accept")] Accept = 1,
        [Description("Open")] Open = 2,
        [Description("In Progress")] InProgress = 3,
        [Description("Close")] Close = 4,
    }
}