namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VariableScorecard : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Scorecard", "VariableStart", c => c.DateTime());
            AddColumn("dbo.Scorecard", "VariableEnd", c => c.DateTime());
            AddColumn("dbo.Scorecard", "VariableYear", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Scorecard", "VariableYear");
            DropColumn("dbo.Scorecard", "VariableEnd");
            DropColumn("dbo.Scorecard", "VariableStart");
        }
    }
}
