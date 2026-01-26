namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateChangeScoreCardSchema : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScorecardTemplateItem", "ScorecardScoring", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScorecardTemplateItem", "ScorecardScoring");
        }
    }
}
