namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Added_AllowSubProjectAlternativeType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProjectType", "AllowSubProjectAlternativeType", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.ProjectType", "AllowSubProjectAlternativeType");
        }
    }
}