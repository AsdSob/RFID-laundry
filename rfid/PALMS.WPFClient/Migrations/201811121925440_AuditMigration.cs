namespace PALMS.WPFClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AuditMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TrackingServiceRows", "UserId", "dbo.Users");
            DropForeignKey("dbo.TrackingServices", "UserId", "dbo.Users");
            DropIndex("dbo.TrackingServices", new[] { "UserId" });
            DropIndex("dbo.TrackingServiceRows", new[] { "UserId" });
            CreateTable(
                "dbo.Audit",
                c => new
                    {
                        AuditEntryID = c.Int(nullable: false, identity: true),
                        EntitySetName = c.String(maxLength: 255),
                        EntityTypeName = c.String(maxLength: 255),
                        State = c.Int(nullable: false),
                        StateName = c.String(maxLength: 255),
                        CreatedBy = c.String(maxLength: 255),
                        CreatedDate = c.DateTime(nullable: false),
                        EntityId = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.AuditEntryID);
            
            CreateTable(
                "dbo.AuditProperties",
                c => new
                    {
                        AuditEntryPropertyID = c.Int(nullable: false, identity: true),
                        AuditEntryID = c.Int(nullable: false),
                        RelationName = c.String(maxLength: 255),
                        PropertyName = c.String(maxLength: 255),
                        OldValue = c.String(),
                        NewValue = c.String(),
                        EntityId = c.Int(),
                        EntityTypeName = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.AuditEntryPropertyID)
                .ForeignKey("dbo.Audit", t => t.AuditEntryID, cascadeDelete: true)
                .Index(t => t.AuditEntryID);
            
            AddColumn("dbo.Clients", "Email", c => c.String());
            AddColumn("dbo.Clients", "Password", c => c.String());
            DropColumn("dbo.ClientInfoes", "CreatedDate");
            DropColumn("dbo.Clients", "CreatedDate");
            DropColumn("dbo.Departments", "CreatedDate");
            DropColumn("dbo.DepartmentContracts", "CreatedDate");
            DropColumn("dbo.FamilyLinens", "CreatedDate");
            DropColumn("dbo.MasterLinens", "CreatedDate");
            DropColumn("dbo.GroupLinens", "CreatedDate");
            DropColumn("dbo.LinenLists", "CreatedDate");
            DropColumn("dbo.LeasingLinens", "CreatedDate");
            DropColumn("dbo.NoteRows", "CreatedDate");
            DropColumn("dbo.NoteHeaders", "CreatedDate");
            DropColumn("dbo.Invoices", "CreatedDate");
            DropColumn("dbo.ExtraCharges", "CreatedDate");
            DropColumn("dbo.TaxAndFees", "CreatedDate");
            DropColumn("dbo.TpsRecords", "CreatedDate");
            DropColumn("dbo.LinenTypes", "CreatedDate");
            DropColumn("dbo.TrackingServices", "CreatedDate");
            DropColumn("dbo.TrackingServiceRows", "CreatedDate");
            DropColumn("dbo.TrackingTypes", "CreatedDate");
            DropColumn("dbo.PrimeInfoes", "CreatedDate");
            DropTable("dbo.Users");
        }
        
        public override void Down()
        {
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
            
            AddColumn("dbo.PrimeInfoes", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.TrackingTypes", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.TrackingServiceRows", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.TrackingServices", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.LinenTypes", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.TpsRecords", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.TaxAndFees", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.ExtraCharges", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Invoices", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.NoteHeaders", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.NoteRows", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.LeasingLinens", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.LinenLists", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.GroupLinens", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.MasterLinens", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.FamilyLinens", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.DepartmentContracts", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Departments", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Clients", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.ClientInfoes", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            DropForeignKey("dbo.AuditProperties", "AuditEntryID", "dbo.Audit");
            DropIndex("dbo.AuditProperties", new[] { "AuditEntryID" });
            DropColumn("dbo.Clients", "Password");
            DropColumn("dbo.Clients", "Email");
            DropTable("dbo.AuditProperties");
            DropTable("dbo.Audit");
            CreateIndex("dbo.TrackingServiceRows", "UserId");
            CreateIndex("dbo.TrackingServices", "UserId");
            AddForeignKey("dbo.TrackingServices", "UserId", "dbo.Users", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TrackingServiceRows", "UserId", "dbo.Users", "Id");
        }
    }
}
