namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedTeamJobDesignationLineLeaderIdToTypeNullableAndRemovedRequiredAnnotation : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.TeamJobDesignation", new[] { "LineLeaderId" });
            AlterColumn("dbo.TeamJobDesignation", "LineLeaderId", c => c.Guid());
            CreateIndex("dbo.TeamJobDesignation", "LineLeaderId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.TeamJobDesignation", new[] { "LineLeaderId" });
            AlterColumn("dbo.TeamJobDesignation", "LineLeaderId", c => c.Guid(nullable: false));
            CreateIndex("dbo.TeamJobDesignation", "LineLeaderId");
        }
    }
}
