namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateChangeScoreCardSchemaCorrectedWrongForieghnKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ScorecardRecord", "Id", "dbo.Scorecard");
            AddForeignKey("dbo.ScorecardRecord", "ScorecardId", "dbo.Scorecard", "Id");
        }

        public override void Down()
        {
/*            RenameIndex(table: "dbo.ScorecardRecord", name: "IX_ScorecardId", newName: "IX_Id");
            RenameColumn(table: "dbo.ScorecardRecord", name: "ScorecardId", newName: "Id");
            AddColumn("dbo.ScorecardRecord", "ScorecardId", c => c.Guid(nullable: false)); */
        }
    }
}
