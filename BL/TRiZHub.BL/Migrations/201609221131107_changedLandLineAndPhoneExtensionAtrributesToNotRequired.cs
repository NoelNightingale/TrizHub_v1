namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedLandLineAndPhoneExtensionAtrributesToNotRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PersonalInformation", "PhoneExtension", c => c.String(maxLength: 500));
            AlterColumn("dbo.PersonalInformation", "LandLinePhone", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PersonalInformation", "LandLinePhone", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.PersonalInformation", "PhoneExtension", c => c.String(nullable: false, maxLength: 500));
        }
    }
}
