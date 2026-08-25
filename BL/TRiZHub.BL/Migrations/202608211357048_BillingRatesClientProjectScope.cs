namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BillingRatesClientProjectScope : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BillingRates", "ClientId", c => c.Guid());
            AddColumn("dbo.BillingRates", "ProjectId", c => c.Guid());
            CreateIndex("dbo.BillingRates", "ClientId", name: "IDX_BillingRatesClient");
            CreateIndex("dbo.BillingRates", "ProjectId", name: "IDX_BillingRatesProject");
            AddForeignKey("dbo.BillingRates", "ClientId", "dbo.ClientEntity", "Id");
            AddForeignKey("dbo.BillingRates", "ProjectId", "dbo.Project", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BillingRates", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.BillingRates", "ClientId", "dbo.ClientEntity");
            DropIndex("dbo.BillingRates", "IDX_BillingRatesProject");
            DropIndex("dbo.BillingRates", "IDX_BillingRatesClient");
            DropColumn("dbo.BillingRates", "ProjectId");
            DropColumn("dbo.BillingRates", "ClientId");
        }
    }
}
