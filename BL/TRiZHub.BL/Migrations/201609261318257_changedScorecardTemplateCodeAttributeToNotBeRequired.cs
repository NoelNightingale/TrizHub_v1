namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedScorecardTemplateCodeAttributeToNotBeRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ScorecardTemplate", "ScorecardCode", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ScorecardTemplate", "ScorecardCode", c => c.String(nullable: false, maxLength: 500));
        }
    }
}
