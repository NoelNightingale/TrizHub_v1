namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VariableScorecardTemplatePeriod : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScorecardTemplatePeriod", "IsVariable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScorecardTemplatePeriod", "IsVariable");
        }
    }
}
