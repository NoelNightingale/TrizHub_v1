namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20190822_AddedIsDeletedToProjects : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Project", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.SubProject", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubProject", "IsDeleted");
            DropColumn("dbo.Project", "IsDeleted");
        }
    }
}
