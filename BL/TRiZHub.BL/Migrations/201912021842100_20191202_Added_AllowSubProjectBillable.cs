namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20191202_Added_AllowSubProjectBillable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProjectType", "AllowSubProjectBillable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProjectType", "AllowSubProjectBillable");
        }
    }
}
