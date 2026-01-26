namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModelToOfficeEquipment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OfficeEquipments", "Model", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OfficeEquipments", "Model");
        }
    }
}
