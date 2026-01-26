namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedReportOrderToPeriods : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScorecardTemplatePeriod", "ReportSortOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScorecardTemplatePeriod", "ReportSortOrder");
        }
    }
}
