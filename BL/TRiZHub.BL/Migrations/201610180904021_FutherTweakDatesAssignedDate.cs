namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FutherTweakDatesAssignedDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OfficeEquipments", "AssignedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OfficeEquipments", "AssignedDate", c => c.DateTime(nullable: false));
        }
    }
}
