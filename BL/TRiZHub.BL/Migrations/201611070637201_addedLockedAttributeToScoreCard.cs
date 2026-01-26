namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedLockedAttributeToScoreCard : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Scorecard", "locked", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Scorecard", "locked");
        }
    }
}
