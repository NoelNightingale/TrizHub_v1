namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addClientReporter : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClientReporter",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ClientId = c.Guid(nullable: false),
                        UserAccountId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClientEntity", t => t.ClientId)
                .ForeignKey("dbo.UserIdentity", t => t.UserAccountId)
                .Index(t => t.ClientId)
                .Index(t => t.UserAccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClientReporter", "UserAccountId", "dbo.UserIdentity");
            DropForeignKey("dbo.ClientReporter", "ClientId", "dbo.ClientEntity");
            DropIndex("dbo.ClientReporter", new[] { "UserAccountId" });
            DropIndex("dbo.ClientReporter", new[] { "ClientId" });
            DropTable("dbo.ClientReporter");
        }
    }
}
