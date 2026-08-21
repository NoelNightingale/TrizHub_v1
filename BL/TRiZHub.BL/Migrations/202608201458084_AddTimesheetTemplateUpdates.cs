namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTimesheetTemplateUpdates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TimesheetTemplateItem",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TimesheetTemplateId = c.Guid(nullable: false),
                        DayOffset = c.Int(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        ProjectId = c.Guid(nullable: false),
                        SubProjectId = c.Guid(),
                        TeamId = c.Guid(nullable: false),
                        ActivityId = c.Guid(nullable: false),
                        Hours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Comments = c.String(nullable: false),
                        ProjectDescription = c.String(maxLength: 500),
                        ClientEntityName = c.String(maxLength: 500),
                        Billable = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Activity", t => t.ActivityId)
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .ForeignKey("dbo.SubProject", t => t.SubProjectId)
                .ForeignKey("dbo.Team", t => t.TeamId)
                .ForeignKey("dbo.TimesheetTemplate", t => t.TimesheetTemplateId)
                .Index(t => t.TimesheetTemplateId)
                .Index(t => t.ProjectId)
                .Index(t => t.SubProjectId)
                .Index(t => t.TeamId)
                .Index(t => t.ActivityId);
            
            CreateTable(
                "dbo.TimesheetTemplate",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserAccountId = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 200),
                        TemplateType = c.String(nullable: false, maxLength: 20),
                        IsActive = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserAccount", t => t.UserAccountId)
                .Index(t => t.UserAccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TimesheetTemplate", "UserAccountId", "dbo.UserAccount");
            DropForeignKey("dbo.TimesheetTemplateItem", "TimesheetTemplateId", "dbo.TimesheetTemplate");
            DropForeignKey("dbo.TimesheetTemplateItem", "TeamId", "dbo.Team");
            DropForeignKey("dbo.TimesheetTemplateItem", "SubProjectId", "dbo.SubProject");
            DropForeignKey("dbo.TimesheetTemplateItem", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.TimesheetTemplateItem", "ActivityId", "dbo.Activity");
            DropIndex("dbo.TimesheetTemplate", new[] { "UserAccountId" });
            DropIndex("dbo.TimesheetTemplateItem", new[] { "ActivityId" });
            DropIndex("dbo.TimesheetTemplateItem", new[] { "TeamId" });
            DropIndex("dbo.TimesheetTemplateItem", new[] { "SubProjectId" });
            DropIndex("dbo.TimesheetTemplateItem", new[] { "ProjectId" });
            DropIndex("dbo.TimesheetTemplateItem", new[] { "TimesheetTemplateId" });
            DropTable("dbo.TimesheetTemplate");
            DropTable("dbo.TimesheetTemplateItem");
        }
    }
}
