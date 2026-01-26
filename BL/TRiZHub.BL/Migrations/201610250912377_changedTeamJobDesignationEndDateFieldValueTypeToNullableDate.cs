namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedTeamJobDesignationEndDateFieldValueTypeToNullableDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TeamJobDesignation", "EndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TeamJobDesignation", "EndDate", c => c.DateTime(nullable: false));
        }
    }
}
