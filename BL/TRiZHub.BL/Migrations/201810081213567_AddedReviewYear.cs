namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedReviewYear : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScorecardTemplatePeriod", "ReviewYear", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScorecardTemplatePeriod", "ReviewYear");
        }
    }
}
