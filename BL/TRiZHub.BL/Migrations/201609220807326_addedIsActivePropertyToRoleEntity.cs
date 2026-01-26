namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedIsActivePropertyToRoleEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Role", "isActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Role", "isActive");
        }
    }
}
