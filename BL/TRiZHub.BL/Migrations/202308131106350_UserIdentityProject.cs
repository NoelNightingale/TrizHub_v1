namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserIdentityProject : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserIdentityProject",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserAccountId = c.Guid(nullable: false),
                        ProjectId = c.Guid(),
                        SubProjectId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .ForeignKey("dbo.SubProject", t => t.SubProjectId)
                .ForeignKey("dbo.UserAccount", t => t.UserAccountId)
                .Index(t => t.UserAccountId, name: "IDX_UserIdentityProject")
                .Index(t => t.ProjectId)
                .Index(t => t.SubProjectId);

            Sql("insert into Privilege values (newid(), 25, 'ReportGenerationUserProjects')");
            Sql("insert into Privilege values (newid(), 26, 'ReportGenerationUserRoles')");
            Sql("insert into Privilege values (newid(), 27, 'UserProjectMaintenance')");


        }

        public override void Down()
        {
            DropForeignKey("dbo.UserIdentityProject", "UserAccountId", "dbo.UserAccount");
            DropForeignKey("dbo.UserIdentityProject", "SubProjectId", "dbo.SubProject");
            DropForeignKey("dbo.UserIdentityProject", "ProjectId", "dbo.Project");
            DropIndex("dbo.UserIdentityProject", new[] { "SubProjectId" });
            DropIndex("dbo.UserIdentityProject", new[] { "ProjectId" });
            DropIndex("dbo.UserIdentityProject", "IDX_UserIdentityProject");
            DropTable("dbo.UserIdentityProject");
        }
    }
}
