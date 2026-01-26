namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEmployerToTeamandjobdesigtaion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TeamJobDesignation", "EmployerId", c => c.Guid());
            CreateIndex("dbo.TeamJobDesignation", "EmployerId");
            AddForeignKey("dbo.TeamJobDesignation", "EmployerId", "dbo.Employer", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeamJobDesignation", "EmployerId", "dbo.Employer");
            DropIndex("dbo.TeamJobDesignation", new[] { "EmployerId" });
            DropColumn("dbo.TeamJobDesignation", "EmployerId");
        }
    }
}
