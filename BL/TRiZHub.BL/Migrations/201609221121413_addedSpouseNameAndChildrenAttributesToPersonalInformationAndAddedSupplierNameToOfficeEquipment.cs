namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedSpouseNameAndChildrenAttributesToPersonalInformationAndAddedSupplierNameToOfficeEquipment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OfficeEquipments", "SupplierName", c => c.String(nullable: false, maxLength: 500));
            AddColumn("dbo.PersonalInformation", "SpouseName", c => c.String(maxLength: 500));
            AddColumn("dbo.PersonalInformation", "Children", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PersonalInformation", "Children");
            DropColumn("dbo.PersonalInformation", "SpouseName");
            DropColumn("dbo.OfficeEquipments", "SupplierName");
        }
    }
}
