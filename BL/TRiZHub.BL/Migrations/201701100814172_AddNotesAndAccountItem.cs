namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNotesAndAccountItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OfficeEquipments", "Notes", c => c.String());
            AddColumn("dbo.OfficeEquipments", "IsAccountingItem", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OfficeEquipments", "IsAccountingItem");
            DropColumn("dbo.OfficeEquipments", "Notes");
        }
    }
}
