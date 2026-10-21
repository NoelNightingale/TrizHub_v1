namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWorkTeamPrivileges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WorkTeamMemberPrivilege",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WorkTeamMemberId = c.Guid(nullable: false),
                        Privilege = c.Int(nullable: false),
                        IsGranted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkTeamMember", t => t.WorkTeamMemberId)
                .Index(t => new { t.WorkTeamMemberId, t.Privilege }, unique: true, name: "UIDX_WorkTeamMemberPrivilege");
            
            CreateTable(
                "dbo.WorkTeamRolePrivilege",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Role = c.Int(nullable: false),
                        Privilege = c.Int(nullable: false),
                        IsGranted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.Role, t.Privilege }, unique: true, name: "UIDX_WorkTeamRolePrivilege");

            // Role: Manager 0, Lead 1, Member 2.
            // Privilege: CaptureTime 0, TeamTimesheets 1, TeamScorecards 2, MaintainClient 3, ManageRates 4, ManageMembers 5.
            // Every role captures time. Managers also manage rates and members. Everything else stays off.
            Sql(@"
INSERT INTO dbo.WorkTeamRolePrivilege (Id, Role, Privilege, IsGranted) VALUES
(NEWID(), 0, 0, 1), (NEWID(), 1, 0, 1), (NEWID(), 2, 0, 1),
(NEWID(), 0, 1, 0), (NEWID(), 1, 1, 0), (NEWID(), 2, 1, 0),
(NEWID(), 0, 2, 0), (NEWID(), 1, 2, 0), (NEWID(), 2, 2, 0),
(NEWID(), 0, 3, 0), (NEWID(), 1, 3, 0), (NEWID(), 2, 3, 0),
(NEWID(), 0, 4, 1), (NEWID(), 1, 4, 0), (NEWID(), 2, 4, 0),
(NEWID(), 0, 5, 1), (NEWID(), 1, 5, 0), (NEWID(), 2, 5, 0);
");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkTeamMemberPrivilege", "WorkTeamMemberId", "dbo.WorkTeamMember");
            DropIndex("dbo.WorkTeamRolePrivilege", "UIDX_WorkTeamRolePrivilege");
            DropIndex("dbo.WorkTeamMemberPrivilege", "UIDX_WorkTeamMemberPrivilege");
            DropTable("dbo.WorkTeamRolePrivilege");
            DropTable("dbo.WorkTeamMemberPrivilege");
        }
    }
}
