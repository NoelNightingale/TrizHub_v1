namespace TRiZHub.BL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class WorkGroups : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WorkGroup",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 500),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.WorkGroupClient",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WorkGroupId = c.Guid(nullable: false),
                        ClientId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClientEntity", t => t.ClientId)
                .ForeignKey("dbo.WorkGroup", t => t.WorkGroupId)
                .Index(t => new { t.WorkGroupId, t.ClientId }, unique: true, name: "UIDX_WorkGroupClient");

            CreateTable(
                "dbo.WorkGroupManager",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WorkGroupId = c.Guid(nullable: false),
                        UserAccountId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserAccount", t => t.UserAccountId)
                .ForeignKey("dbo.WorkGroup", t => t.WorkGroupId)
                .Index(t => new { t.WorkGroupId, t.UserAccountId }, unique: true, name: "UIDX_WorkGroupManager");

            CreateTable(
                "dbo.WorkGroupManagerCapability",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WorkGroupManagerId = c.Guid(nullable: false),
                        Capability = c.Int(nullable: false),
                        IsAllowed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkGroupManager", t => t.WorkGroupManagerId, cascadeDelete: true)
                .Index(t => new { t.WorkGroupManagerId, t.Capability }, unique: true, name: "UIDX_WorkGroupManagerCapability");

            CreateTable(
                "dbo.WorkTeam",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WorkGroupId = c.Guid(nullable: false),
                        TeamTypeId = c.Guid(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkGroup", t => t.WorkGroupId)
                .ForeignKey("dbo.Team", t => t.TeamTypeId)
                .Index(t => new { t.WorkGroupId, t.TeamTypeId }, unique: true, name: "UIDX_WorkTeamGroupType");

            CreateTable(
                "dbo.WorkTeamMember",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WorkTeamId = c.Guid(nullable: false),
                        UserAccountId = c.Guid(nullable: false),
                        Role = c.Int(nullable: false),
                        IsHomeTeam = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkTeam", t => t.WorkTeamId)
                .ForeignKey("dbo.UserAccount", t => t.UserAccountId)
                .Index(t => new { t.WorkTeamId, t.UserAccountId }, unique: true, name: "UIDX_WorkTeamMember");

            CreateTable(
                "dbo.WorkTeamMemberCapability",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WorkTeamMemberId = c.Guid(nullable: false),
                        Capability = c.Int(nullable: false),
                        IsAllowed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkTeamMember", t => t.WorkTeamMemberId, cascadeDelete: true)
                .Index(t => new { t.WorkTeamMemberId, t.Capability }, unique: true, name: "UIDX_WorkTeamMemberCapability");
        }

        public override void Down()
        {
            DropForeignKey("dbo.WorkTeamMemberCapability", "WorkTeamMemberId", "dbo.WorkTeamMember");
            DropForeignKey("dbo.WorkTeamMember", "UserAccountId", "dbo.UserAccount");
            DropForeignKey("dbo.WorkTeamMember", "WorkTeamId", "dbo.WorkTeam");
            DropForeignKey("dbo.WorkTeam", "TeamTypeId", "dbo.Team");
            DropForeignKey("dbo.WorkTeam", "WorkGroupId", "dbo.WorkGroup");
            DropForeignKey("dbo.WorkGroupManagerCapability", "WorkGroupManagerId", "dbo.WorkGroupManager");
            DropForeignKey("dbo.WorkGroupManager", "UserAccountId", "dbo.UserAccount");
            DropForeignKey("dbo.WorkGroupManager", "WorkGroupId", "dbo.WorkGroup");
            DropForeignKey("dbo.WorkGroupClient", "ClientId", "dbo.ClientEntity");
            DropForeignKey("dbo.WorkGroupClient", "WorkGroupId", "dbo.WorkGroup");
            DropIndex("dbo.WorkTeamMemberCapability", "UIDX_WorkTeamMemberCapability");
            DropIndex("dbo.WorkTeamMember", "UIDX_WorkTeamMember");
            DropIndex("dbo.WorkTeam", "UIDX_WorkTeamGroupType");
            DropIndex("dbo.WorkGroupManagerCapability", "UIDX_WorkGroupManagerCapability");
            DropIndex("dbo.WorkGroupManager", "UIDX_WorkGroupManager");
            DropIndex("dbo.WorkGroupClient", "UIDX_WorkGroupClient");
            DropTable("dbo.WorkTeamMemberCapability");
            DropTable("dbo.WorkTeamMember");
            DropTable("dbo.WorkTeam");
            DropTable("dbo.WorkGroupManagerCapability");
            DropTable("dbo.WorkGroupManager");
            DropTable("dbo.WorkGroupClient");
            DropTable("dbo.WorkGroup");
        }
    }
}
