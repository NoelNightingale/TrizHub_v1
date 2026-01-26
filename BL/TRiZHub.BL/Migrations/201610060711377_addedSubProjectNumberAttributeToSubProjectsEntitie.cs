namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedSubProjectNumberAttributeToSubProjectsEntitie : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubProject", "SubProjectNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubProject", "SubProjectNumber");
        }
    }
}
