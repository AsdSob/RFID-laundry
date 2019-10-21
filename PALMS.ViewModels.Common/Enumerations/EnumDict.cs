using System.ComponentModel;

namespace PALMS.ViewModels.Common.Enumerations
{

    public enum RoleEnum
    {
        [Description("NONE")] None = 0,
        [Description("ADMIN")] Admin = 1,
        [Description("ACCOUNT")] Account = 2,
        [Description("SUPERVISOR")] Supervisor = 3,
        [Description("RECEPTION")] Reception = 4,
        [Description("OPERATOR")] Operator = 5,
    }

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
        [Description("No Prime Logo")] NoLogo = 2,
        [Description("Damage")] Damage = 3,
        [Description("New")] New = 4,
        [Description("Stain")] Stain = 5,
        [Description("Express")] Express = 6,
        [Description("Client Name")] Name = 7,
        [Description("Re-Wash")] ReWash = 8,
    }
    public enum DepartmentTypeEnum
    {
        [Description("Linen")] Linen = 1,
        [Description("F&B")] FnB = 2,
        [Description("Spa")] Spa = 3,
        [Description("Management")] Management = 4,
        [Description("Uniform")] Uniform = 5,
        [Description("Guest Laundry")] GuestLaundry = 6,
    }

    public enum CoorTypeEnum
    {
        [Description("Client")] Client = 1,
        [Description("Department")] Department = 2,
        [Description("Note")] Note = 3,
        [Description("Linen")] Linen = 4,
        [Description("Service")] Service = 5,

        [Description("Note Kg")] NoteKg = 6,

        [Description("Pc")] Pc = 7,
        [Description("Price")] Price = 8,
        [Description("Weight")] Weight = 9,
        [Description("Lien Price")] LinenPrice = 10,

        [Description("Day")] Day = 12,
        [Description("Month")] Month = 13,
        [Description("Year")] Year = 14,
    }

    public enum StaffShiftEnum
    {
        [Description("Day")] Day = 1,
        [Description("Night")] Night = 2,
    }

    public enum KgTypeEnum
    {
        [Description("Soiled")] Soiled = 1,
        [Description("Clean")] Clean = 2,
    }
}