namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectTimeCaptureAssignment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserIdentityClient",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserAccountId = c.Guid(nullable: false),
                        ClientId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClientEntity", t => t.ClientId)
                .ForeignKey("dbo.UserAccount", t => t.UserAccountId)
                .Index(t => t.UserAccountId, name: "IDX_UserIdentityClient")
                .Index(t => t.ClientId);
            
            AddColumn("dbo.Project", "ExcludeTimeCapture", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserIdentityClient", "UserAccountId", "dbo.UserAccount");
            DropForeignKey("dbo.UserIdentityClient", "ClientId", "dbo.ClientEntity");
            DropIndex("dbo.UserIdentityClient", new[] { "ClientId" });
            DropIndex("dbo.UserIdentityClient", "IDX_UserIdentityClient");
            DropColumn("dbo.Project", "ExcludeTimeCapture");
            DropTable("dbo.UserIdentityClient");
        }
    }
}
