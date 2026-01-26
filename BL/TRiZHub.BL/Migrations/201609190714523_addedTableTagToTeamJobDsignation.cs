namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedTableTagToTeamJobDsignation : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TeamJobDesignations", newName: "TeamJobDesignation");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.TeamJobDesignation", newName: "TeamJobDesignations");
        }
    }
}
