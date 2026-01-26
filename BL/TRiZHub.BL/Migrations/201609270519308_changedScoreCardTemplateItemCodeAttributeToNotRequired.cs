namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedScoreCardTemplateItemCodeAttributeToNotRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ScorecardTemplateItem", "Code", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ScorecardTemplateItem", "Code", c => c.String(nullable: false));
        }
    }
}
