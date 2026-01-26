namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectTypes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectType",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Project", "ProjectTypeId", c => c.Guid());
            AddColumn("dbo.SubProject", "SubProjectTypeId", c => c.Guid());
            CreateIndex("dbo.Project", "ProjectTypeId");
            CreateIndex("dbo.SubProject", "SubProjectTypeId");
            AddForeignKey("dbo.Project", "ProjectTypeId", "dbo.ProjectType", "Id");
            AddForeignKey("dbo.SubProject", "SubProjectTypeId", "dbo.ProjectType", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubProject", "SubProjectTypeId", "dbo.ProjectType");
            DropForeignKey("dbo.Project", "ProjectTypeId", "dbo.ProjectType");
            DropIndex("dbo.SubProject", new[] { "SubProjectTypeId" });
            DropIndex("dbo.Project", new[] { "ProjectTypeId" });
            DropColumn("dbo.SubProject", "SubProjectTypeId");
            DropColumn("dbo.Project", "ProjectTypeId");
            DropTable("dbo.ProjectType");
        }
    }
}
