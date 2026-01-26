namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedDefinitionToScorecardTemplateItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScorecardTemplateItem", "Definition", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScorecardTemplateItem", "Definition");
        }
    }
}
