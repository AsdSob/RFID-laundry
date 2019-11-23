namespace PALMS.WPFClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuditDescriptionMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Audit", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Audit", "Description");
        }
    }
}
