namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWorkTeams : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WorkTeam",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 500),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        TeamTypeId = c.Guid(nullable: false),
                        AllowTeamTimesheets = c.Boolean(nullable: false),
                        AllowTeamScorecards = c.Boolean(nullable: false),
                        LeadsManageAllocations = c.Boolean(nullable: false),
                        LeadsManageRates = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Team", t => t.TeamTypeId)
                .Index(t => t.TeamTypeId, name: "IDX_WorkTeamTeamType");
            
            CreateTable(
                "dbo.WorkTeamMember",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WorkTeamId = c.Guid(nullable: false),
                        UserAccountId = c.Guid(nullable: false),
                        Role = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkTeam", t => t.WorkTeamId)
                .ForeignKey("dbo.UserAccount", t => t.UserAccountId)
                .Index(t => t.WorkTeamId, name: "IDX_WorkTeamMemberTeam")
                .Index(t => t.UserAccountId, name: "IDX_WorkTeamMemberUser");
            
            CreateTable(
                "dbo.WorkTeamClient",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WorkTeamId = c.Guid(nullable: false),
                        ClientId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkTeam", t => t.WorkTeamId)
                .ForeignKey("dbo.ClientEntity", t => t.ClientId)
                .Index(t => new { t.WorkTeamId, t.ClientId }, unique: true, name: "UIDX_WorkTeamClient");
            
            CreateTable(
                "dbo.WorkTeamProject",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WorkTeamId = c.Guid(nullable: false),
                        ProjectId = c.Guid(nullable: false),
                        SubProjectId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkTeam", t => t.WorkTeamId)
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .ForeignKey("dbo.SubProject", t => t.SubProjectId)
                .Index(t => t.WorkTeamId, name: "IDX_WorkTeamProjectTeam")
                .Index(t => t.ProjectId, name: "IDX_WorkTeamProjectProject")
                .Index(t => t.SubProjectId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkTeamProject", "SubProjectId", "dbo.SubProject");
            DropForeignKey("dbo.WorkTeamProject", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.WorkTeamProject", "WorkTeamId", "dbo.WorkTeam");
            DropForeignKey("dbo.WorkTeamClient", "ClientId", "dbo.ClientEntity");
            DropForeignKey("dbo.WorkTeamClient", "WorkTeamId", "dbo.WorkTeam");
            DropForeignKey("dbo.WorkTeamMember", "UserAccountId", "dbo.UserAccount");
            DropForeignKey("dbo.WorkTeamMember", "WorkTeamId", "dbo.WorkTeam");
            DropForeignKey("dbo.WorkTeam", "TeamTypeId", "dbo.Team");
            DropIndex("dbo.WorkTeamProject", new[] { "SubProjectId" });
            DropIndex("dbo.WorkTeamProject", "IDX_WorkTeamProjectProject");
            DropIndex("dbo.WorkTeamProject", "IDX_WorkTeamProjectTeam");
            DropIndex("dbo.WorkTeamClient", "UIDX_WorkTeamClient");
            DropIndex("dbo.WorkTeamMember", "IDX_WorkTeamMemberUser");
            DropIndex("dbo.WorkTeamMember", "IDX_WorkTeamMemberTeam");
            DropIndex("dbo.WorkTeam", "IDX_WorkTeamTeamType");
            DropTable("dbo.WorkTeamProject");
            DropTable("dbo.WorkTeamClient");
            DropTable("dbo.WorkTeamMember");
            DropTable("dbo.WorkTeam");
        }
    }
}
