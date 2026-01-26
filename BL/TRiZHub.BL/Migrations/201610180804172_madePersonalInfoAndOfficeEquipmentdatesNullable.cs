namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class madePersonalInfoAndOfficeEquipmentdatesNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OfficeEquipments", "ReturnDate", c => c.DateTime());
            AlterColumn("dbo.PersonalInformation", "EmploymentEndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PersonalInformation", "EmploymentEndDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.OfficeEquipments", "ReturnDate", c => c.DateTime(nullable: false));
    }
    }
}
