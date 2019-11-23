namespace PALMS.WPFClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            return;
            CreateTable(
                "dbo.ClientInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Comment = c.String(),
                        TicketId = c.Int(nullable: false),
                        NoteId = c.Int(nullable: false),
                        InvoiceId = c.Int(nullable: false),
                        Address = c.String(),
                        Express = c.Double(nullable: false),
                        WeighPrice = c.Double(nullable: false),
                        Start = c.Int(nullable: false),
                        End = c.Int(nullable: false),
                        TRNNumber = c.String(),
                        Logo = c.Binary(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShortName = c.String(),
                        Color = c.String(),
                        Active = c.Boolean(nullable: false),
                        Name = c.String(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        ParentId = c.Int(),
                        AllFree = c.Boolean(nullable: false),
                        Labeling = c.Boolean(nullable: false),
                        Name = c.String(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.ParentId)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .Index(t => t.ClientId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.DepartmentContracts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DepartmentId = c.Int(nullable: false),
                        FamilyLinenId = c.Int(),
                        Quantity = c.Int(),
                        Percentage = c.Double(nullable: false),
                        OrderNumber = c.Int(nullable: false),
                        InvoiceId = c.Int(),
                        Name = c.String(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.FamilyLinens", t => t.FamilyLinenId)
                .ForeignKey("dbo.Invoices", t => t.InvoiceId)
                .Index(t => t.DepartmentId)
                .Index(t => t.FamilyLinenId)
                .Index(t => t.InvoiceId);
            
            CreateTable(
                "dbo.FamilyLinens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MasterLinens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        LinenTypeId = c.Int(nullable: false),
                        FamilyLinenId = c.Int(nullable: false),
                        GroupLinenId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FamilyLinens", t => t.FamilyLinenId, cascadeDelete: true)
                .ForeignKey("dbo.GroupLinens", t => t.GroupLinenId, cascadeDelete: true)
                .ForeignKey("dbo.LinenTypes", t => t.LinenTypeId, cascadeDelete: true)
                .Index(t => t.LinenTypeId)
                .Index(t => t.FamilyLinenId)
                .Index(t => t.GroupLinenId);
            
            CreateTable(
                "dbo.GroupLinens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Name = c.String(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LinenLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                        MasterLinenId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Weight = c.Int(nullable: false),
                        UnitId = c.Int(nullable: false),
                        Laundry = c.Double(nullable: false),
                        Pressing = c.Double(nullable: false),
                        DryCleaning = c.Double(nullable: false),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.MasterLinens", t => t.MasterLinenId, cascadeDelete: true)
                .Index(t => t.ClientId)
                .Index(t => t.DepartmentId)
                .Index(t => t.MasterLinenId);
            
            CreateTable(
                "dbo.LeasingLinens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LinenListId = c.Int(nullable: false),
                        OriginalPrice = c.Double(nullable: false),
                        Name = c.String(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LinenLists", t => t.LinenListId, cascadeDelete: true)
                .Index(t => t.LinenListId);
            
            CreateTable(
                "dbo.NoteRows",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NoteHeaderId = c.Int(nullable: false),
                        LinenListId = c.Int(nullable: false),
                        PrimeCollectionQty = c.Double(nullable: false),
                        PrimeDeliveryQty = c.Double(nullable: false),
                        ClientReceivedQty = c.Double(nullable: false),
                        Comment = c.String(),
                        ServiceTypeId = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        PriceUnit = c.Int(nullable: false),
                        Weight = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LinenLists", t => t.LinenListId, cascadeDelete: true)
                .ForeignKey("dbo.NoteHeaders", t => t.NoteHeaderId)
                .Index(t => t.NoteHeaderId)
                .Index(t => t.LinenListId);
            
            CreateTable(
                "dbo.NoteHeaders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                        DeliveryTypeId = c.Int(nullable: false),
                        CollectionDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeliveredDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ClientReceivedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ExpressCharge = c.Double(nullable: false),
                        WeightPrice = c.Double(nullable: false),
                        CollectionWeight = c.Double(nullable: false),
                        DeliveryWeight = c.Double(nullable: false),
                        Comment = c.String(),
                        InvoiceId = c.Int(),
                        NoteStatus = c.Int(),
                        Name = c.String(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.Invoices", t => t.InvoiceId)
                .Index(t => t.ClientId)
                .Index(t => t.DepartmentId)
                .Index(t => t.InvoiceId);
            
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        DateStart = c.DateTime(nullable: false),
                        DateEnd = c.DateTime(nullable: false),
                        VAT = c.Double(nullable: false),
                        Name = c.String(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.ExtraCharges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InvoiceId = c.Int(),
                        Amount = c.Double(nullable: false),
                        Name = c.String(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Invoices", t => t.InvoiceId)
                .Index(t => t.InvoiceId);
            
            CreateTable(
                "dbo.TaxAndFees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        UnitId = c.Int(nullable: false),
                        Number = c.Double(nullable: false),
                        Priority = c.Int(nullable: false),
                        InvoiceId = c.Int(),
                        Name = c.String(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Invoices", t => t.InvoiceId)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .Index(t => t.ClientId)
                .Index(t => t.InvoiceId);
            
            CreateTable(
                "dbo.TpsRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        LinenListId = c.Int(),
                        Quantity = c.Int(),
                        Package = c.Int(),
                        TicketType = c.String(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.LinenLists", t => t.LinenListId)
                .Index(t => t.ClientId)
                .Index(t => t.LinenListId);
            
            CreateTable(
                "dbo.LinenTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrackingServices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        TrackingTypeId = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                        Description = c.String(),
                        DateOpen = c.DateTime(nullable: false),
                        DateClose = c.DateTime(nullable: false),
                        Name = c.String(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.TrackingTypes", t => t.TrackingTypeId, cascadeDelete: true)
                .Index(t => t.ClientId)
                .Index(t => t.UserId)
                .Index(t => t.TrackingTypeId);
            
            CreateTable(
                "dbo.TrackingServiceRows",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TrackingServiceId = c.Int(nullable: false),
                        Comment = c.String(),
                        OrderNumber = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        StaffName = c.String(),
                        DateOpen = c.DateTime(nullable: false),
                        DateClose = c.DateTime(nullable: false),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrackingServices", t => t.TrackingServiceId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.TrackingServiceId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Password = c.String(),
                        RoleId = c.Int(nullable: false),
                        Comment = c.String(),
                        Name = c.String(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrackingTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PrimeInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TRNNumber = c.String(),
                        VAT = c.Double(nullable: false),
                        Address = c.String(),
                        Logo = c.Binary(),
                        Name = c.String(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            return;
            DropForeignKey("dbo.ClientInfoes", "Id", "dbo.Clients");
            DropForeignKey("dbo.TrackingServices", "TrackingTypeId", "dbo.TrackingTypes");
            DropForeignKey("dbo.TrackingServices", "UserId", "dbo.Users");
            DropForeignKey("dbo.TrackingServiceRows", "UserId", "dbo.Users");
            DropForeignKey("dbo.TrackingServiceRows", "TrackingServiceId", "dbo.TrackingServices");
            DropForeignKey("dbo.TrackingServices", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.TaxAndFees", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.Invoices", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.Departments", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.Departments", "ParentId", "dbo.Departments");
            DropForeignKey("dbo.MasterLinens", "LinenTypeId", "dbo.LinenTypes");
            DropForeignKey("dbo.TpsRecords", "LinenListId", "dbo.LinenLists");
            DropForeignKey("dbo.TpsRecords", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.NoteRows", "NoteHeaderId", "dbo.NoteHeaders");
            DropForeignKey("dbo.TaxAndFees", "InvoiceId", "dbo.Invoices");
            DropForeignKey("dbo.NoteHeaders", "InvoiceId", "dbo.Invoices");
            DropForeignKey("dbo.ExtraCharges", "InvoiceId", "dbo.Invoices");
            DropForeignKey("dbo.DepartmentContracts", "InvoiceId", "dbo.Invoices");
            DropForeignKey("dbo.NoteHeaders", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.NoteHeaders", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.NoteRows", "LinenListId", "dbo.LinenLists");
            DropForeignKey("dbo.LinenLists", "MasterLinenId", "dbo.MasterLinens");
            DropForeignKey("dbo.LeasingLinens", "LinenListId", "dbo.LinenLists");
            DropForeignKey("dbo.LinenLists", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.LinenLists", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.MasterLinens", "GroupLinenId", "dbo.GroupLinens");
            DropForeignKey("dbo.MasterLinens", "FamilyLinenId", "dbo.FamilyLinens");
            DropForeignKey("dbo.DepartmentContracts", "FamilyLinenId", "dbo.FamilyLinens");
            DropForeignKey("dbo.DepartmentContracts", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.TrackingServiceRows", new[] { "UserId" });
            DropIndex("dbo.TrackingServiceRows", new[] { "TrackingServiceId" });
            DropIndex("dbo.TrackingServices", new[] { "TrackingTypeId" });
            DropIndex("dbo.TrackingServices", new[] { "UserId" });
            DropIndex("dbo.TrackingServices", new[] { "ClientId" });
            DropIndex("dbo.TpsRecords", new[] { "LinenListId" });
            DropIndex("dbo.TpsRecords", new[] { "ClientId" });
            DropIndex("dbo.TaxAndFees", new[] { "InvoiceId" });
            DropIndex("dbo.TaxAndFees", new[] { "ClientId" });
            DropIndex("dbo.ExtraCharges", new[] { "InvoiceId" });
            DropIndex("dbo.Invoices", new[] { "ClientId" });
            DropIndex("dbo.NoteHeaders", new[] { "InvoiceId" });
            DropIndex("dbo.NoteHeaders", new[] { "DepartmentId" });
            DropIndex("dbo.NoteHeaders", new[] { "ClientId" });
            DropIndex("dbo.NoteRows", new[] { "LinenListId" });
            DropIndex("dbo.NoteRows", new[] { "NoteHeaderId" });
            DropIndex("dbo.LeasingLinens", new[] { "LinenListId" });
            DropIndex("dbo.LinenLists", new[] { "MasterLinenId" });
            DropIndex("dbo.LinenLists", new[] { "DepartmentId" });
            DropIndex("dbo.LinenLists", new[] { "ClientId" });
            DropIndex("dbo.MasterLinens", new[] { "GroupLinenId" });
            DropIndex("dbo.MasterLinens", new[] { "FamilyLinenId" });
            DropIndex("dbo.MasterLinens", new[] { "LinenTypeId" });
            DropIndex("dbo.DepartmentContracts", new[] { "InvoiceId" });
            DropIndex("dbo.DepartmentContracts", new[] { "FamilyLinenId" });
            DropIndex("dbo.DepartmentContracts", new[] { "DepartmentId" });
            DropIndex("dbo.Departments", new[] { "ParentId" });
            DropIndex("dbo.Departments", new[] { "ClientId" });
            DropIndex("dbo.ClientInfoes", new[] { "Id" });
            DropTable("dbo.PrimeInfoes");
            DropTable("dbo.TrackingTypes");
            DropTable("dbo.Users");
            DropTable("dbo.TrackingServiceRows");
            DropTable("dbo.TrackingServices");
            DropTable("dbo.LinenTypes");
            DropTable("dbo.TpsRecords");
            DropTable("dbo.TaxAndFees");
            DropTable("dbo.ExtraCharges");
            DropTable("dbo.Invoices");
            DropTable("dbo.NoteHeaders");
            DropTable("dbo.NoteRows");
            DropTable("dbo.LeasingLinens");
            DropTable("dbo.LinenLists");
            DropTable("dbo.GroupLinens");
            DropTable("dbo.MasterLinens");
            DropTable("dbo.FamilyLinens");
            DropTable("dbo.DepartmentContracts");
            DropTable("dbo.Departments");
            DropTable("dbo.Clients");
            DropTable("dbo.ClientInfoes");
        }
    }
}
