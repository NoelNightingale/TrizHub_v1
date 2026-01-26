namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScorecardComments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScorecardRecord", "EvaluatorHtmlComment", c => c.String());
            AddColumn("dbo.ScorecardRecord", "EmployeeHtmlComment", c => c.String());
        }
        
        public override void Down()
        {
            AddColumn("dbo.ScorecardRecord", "HtmlComment", c => c.String());
            DropColumn("dbo.ScorecardRecord", "EmployeeHtmlComment");
            DropColumn("dbo.ScorecardRecord", "EvaluatorHtmlComment");
        }
    }
}
