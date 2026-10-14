namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserExcludeClosedBP : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BillingCycleExemption",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BillingCycleEntryId = c.Guid(nullable: false),
                        UserAccountId = c.Guid(nullable: false),
                        CreatedByAccountId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BillingCycleEntry", t => t.BillingCycleEntryId)
                .ForeignKey("dbo.UserAccount", t => t.CreatedByAccountId)
                .ForeignKey("dbo.UserAccount", t => t.UserAccountId)
                .Index(t => new { t.BillingCycleEntryId, t.UserAccountId }, unique: true, name: "UIDX_BillingCycleExemption")
                .Index(t => t.UserAccountId, name: "IDX_BillingCycleExemptionUser")
                .Index(t => t.CreatedByAccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BillingCycleExemption", "UserAccountId", "dbo.UserAccount");
            DropForeignKey("dbo.BillingCycleExemption", "CreatedByAccountId", "dbo.UserAccount");
            DropForeignKey("dbo.BillingCycleExemption", "BillingCycleEntryId", "dbo.BillingCycleEntry");
            DropIndex("dbo.BillingCycleExemption", new[] { "CreatedByAccountId" });
            DropIndex("dbo.BillingCycleExemption", "IDX_BillingCycleExemptionUser");
            DropIndex("dbo.BillingCycleExemption", "UIDX_BillingCycleExemption");
            DropTable("dbo.BillingCycleExemption");
        }
    }
}
