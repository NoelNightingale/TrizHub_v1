namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserAccount_Project : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserIdentityProject",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserAccountId = c.Guid(nullable: false),
                        ClientId = c.Guid(),
                        ProjectId = c.Guid(),
                        SubProjectId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClientEntity", t => t.ClientId)
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .ForeignKey("dbo.SubProject", t => t.SubProjectId)
                .ForeignKey("dbo.UserAccount", t => t.UserAccountId)
                .Index(t => t.UserAccountId, name: "IDX_UserIdentityProject")
                .Index(t => t.ClientId)
                .Index(t => t.ProjectId)
                .Index(t => t.SubProjectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserIdentityProject", "UserAccountId", "dbo.UserAccount");
            DropForeignKey("dbo.UserIdentityProject", "SubProjectId", "dbo.SubProject");
            DropForeignKey("dbo.UserIdentityProject", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.UserIdentityProject", "ClientId", "dbo.ClientEntity");
            DropIndex("dbo.UserIdentityProject", new[] { "SubProjectId" });
            DropIndex("dbo.UserIdentityProject", new[] { "ProjectId" });
            DropIndex("dbo.UserIdentityProject", new[] { "ClientId" });
            DropIndex("dbo.UserIdentityProject", "IDX_UserIdentityProject");
            DropTable("dbo.UserIdentityProject");
        }
    }
}
