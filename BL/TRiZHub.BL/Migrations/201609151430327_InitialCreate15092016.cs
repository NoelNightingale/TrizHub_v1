namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate15092016 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activity",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ActivityName = c.String(nullable: false, maxLength: 500),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TimesheetEntry",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserAccountId = c.Guid(nullable: false),
                        ProjectId = c.Guid(nullable: false),
                        SubProjectId = c.Guid(),
                        TeamId = c.Guid(nullable: false),
                        ActivityId = c.Guid(nullable: false),
                        CreatedByAccountId = c.Guid(nullable: false),
                        Comments = c.String(nullable: false),
                        Hours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateEntry = c.DateTime(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        ClientEntity_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Activity", t => t.ActivityId)
                .ForeignKey("dbo.ClientEntity", t => t.ClientEntity_Id)
                .ForeignKey("dbo.SubProject", t => t.SubProjectId)
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .ForeignKey("dbo.UserAccount", t => t.CreatedByAccountId)
                .ForeignKey("dbo.Team", t => t.TeamId)
                .ForeignKey("dbo.UserAccount", t => t.UserAccountId)
                .Index(t => t.UserAccountId)
                .Index(t => t.ProjectId)
                .Index(t => t.SubProjectId)
                .Index(t => t.TeamId)
                .Index(t => t.ActivityId)
                .Index(t => t.CreatedByAccountId)
                .Index(t => t.ClientEntity_Id);
            
            CreateTable(
                "dbo.UserIdentity",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AccountName = c.String(nullable: false, maxLength: 200),
                        IsSystemAdmin = c.Boolean(nullable: false),
                        FirstName = c.String(maxLength: 200),
                        Surname = c.String(maxLength: 200),
                        ProfileImageDataId = c.Guid(nullable: false),
                        Registered = c.DateTime(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ImageData", t => t.ProfileImageDataId)
                .Index(t => new { t.AccountName, t.IsSystemAdmin }, unique: true, name: "UIDX_UserIdentity")
                .Index(t => t.ProfileImageDataId)
                .Index(t => t.Registered, name: "UIDX_UserIdentityRegistered");
            
            CreateTable(
                "dbo.BillingRates",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserAccountId = c.Guid(nullable: false),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserAccount", t => t.UserAccountId)
                .Index(t => t.UserAccountId, name: "IDX_BillingRatesUserAccount");
            
            CreateTable(
                "dbo.EmergancyContact",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserAccountId = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 500),
                        Surname = c.String(nullable: false, maxLength: 500),
                        Relationship = c.String(nullable: false, maxLength: 500),
                        CellphoneNumber = c.String(nullable: false, maxLength: 500),
                        LandLineNumber = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserAccount", t => t.UserAccountId)
                .Index(t => t.UserAccountId, name: "IDX_EmergancyContactUserAccount");
            
            CreateTable(
                "dbo.Scorecard",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ScorecardTemplateId = c.Guid(nullable: false),
                        EvaluatorId = c.Guid(nullable: false),
                        EmployeeId = c.Guid(nullable: false),
                        CreatedBy = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserAccount", t => t.EmployeeId)
                .ForeignKey("dbo.UserAccount", t => t.EvaluatorId)
                .ForeignKey("dbo.ScorecardTemplate", t => t.ScorecardTemplateId)
                .Index(t => t.ScorecardTemplateId)
                .Index(t => t.EvaluatorId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.ScorecardPeriod",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ScorecardId = c.Guid(nullable: false),
                        ScorecardTemplatePeriodId = c.Guid(nullable: false),
                        Rated = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        Completed = c.Boolean(nullable: false),
                        EvaluatorMessage = c.String(),
                        EmployeeMessage = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Scorecard", t => t.ScorecardId)
                .ForeignKey("dbo.ScorecardTemplatePeriod", t => t.ScorecardTemplatePeriodId)
                .Index(t => t.ScorecardId)
                .Index(t => t.ScorecardTemplatePeriodId);
            
            CreateTable(
                "dbo.ScorecardRecord",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ScorecardPeriodId = c.Guid(nullable: false),
                        ScorecardTemplateItemId = c.Guid(nullable: false),
                        Rating = c.Int(),
                        Value = c.Decimal(precision: 18, scale: 2),
                        LastUpdated = c.DateTime(),
                        Completed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ScorecardPeriod", t => t.ScorecardPeriodId)
                .ForeignKey("dbo.ScorecardTemplateItem", t => t.ScorecardTemplateItemId)
                .Index(t => t.ScorecardPeriodId)
                .Index(t => t.ScorecardTemplateItemId);
            
            CreateTable(
                "dbo.ScorecardTemplateItem",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ScorecardTemplateId = c.Guid(nullable: false),
                        Code = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScorecardScoring = c.Int(nullable: false),
                        Minimum = c.Decimal(precision: 18, scale: 2),
                        Maximum = c.Decimal(precision: 18, scale: 2),
                        ManualDefinition = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ScorecardTemplate", t => t.ScorecardTemplateId)
                .Index(t => t.ScorecardTemplateId);
            
            CreateTable(
                "dbo.ScorecardTemplate",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ScorecardName = c.String(nullable: false, maxLength: 500),
                        ScorecardCode = c.String(nullable: false, maxLength: 500),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ScorecardTemplatePeriod",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ScorecardTemplateId = c.Guid(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ScorecardTemplate", t => t.ScorecardTemplateId)
                .Index(t => t.ScorecardTemplateId);
            
            CreateTable(
                "dbo.ScorecardTemplateItemScore",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ScorecardTemplateItemId = c.Guid(nullable: false),
                        ScoreType = c.Int(nullable: false),
                        Score = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Definition = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ScorecardTemplateItem", t => t.ScorecardTemplateItemId)
                .Index(t => t.ScorecardTemplateItemId);
            
            CreateTable(
                "dbo.OfficeEquipments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserAccountId = c.Guid(nullable: false),
                        Type = c.String(nullable: false, maxLength: 500),
                        SerialNumber = c.String(nullable: false, maxLength: 500),
                        Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PurchaseDate = c.DateTime(nullable: false),
                        InvoiceNumber = c.String(nullable: false),
                        AssignedDate = c.DateTime(nullable: false),
                        ReturnDate = c.DateTime(nullable: false),
                        AssetRegister = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserAccount", t => t.UserAccountId)
                .Index(t => t.UserAccountId, name: "IDX_OfficeEquipmentUserAccount");
            
            CreateTable(
                "dbo.PersonalInformation",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserAccountId = c.Guid(nullable: false),
                        FullNames = c.String(nullable: false, maxLength: 500),
                        Surname = c.String(nullable: false, maxLength: 500),
                        Title = c.String(nullable: false, maxLength: 500),
                        IdNumber = c.String(nullable: false, maxLength: 500),
                        Dob = c.DateTime(nullable: false),
                        Company = c.String(nullable: false, maxLength: 500),
                        WorkExperienceStartDate = c.DateTime(nullable: false),
                        EmploymentStartDate = c.DateTime(nullable: false),
                        EmploymentEndDate = c.DateTime(nullable: false),
                        Race = c.String(nullable: false, maxLength: 500),
                        Gender = c.String(nullable: false, maxLength: 500),
                        DoorTagNumber = c.String(nullable: false, maxLength: 500),
                        PhoneExtension = c.String(nullable: false, maxLength: 500),
                        CellPhone = c.String(nullable: false, maxLength: 500),
                        LandLinePhone = c.String(nullable: false, maxLength: 500),
                        CompanyEmail = c.String(nullable: false, maxLength: 500),
                        OtherEmail = c.String(maxLength: 500),
                        AccessLevel = c.String(maxLength: 500),
                        MedicalScheme = c.String(maxLength: 500),
                        MedicalSchemeOption = c.String(maxLength: 500),
                        MedicalAidNumber = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserAccount", t => t.UserAccountId)
                .Index(t => t.UserAccountId, name: "IDX_PersonalInformationUserAccount");
            
            CreateTable(
                "dbo.ImageData",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FileName = c.String(nullable: false, maxLength: 500),
                        FileData = c.Binary(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Project",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ClientId = c.Guid(nullable: false),
                        ProjectLeadId = c.Guid(),
                        ProjectName = c.String(nullable: false, maxLength: 500),
                        ProjectNumber = c.String(),
                        ProjectDescription = c.String(),
                        Billable = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClientEntity", t => t.ClientId)
                .ForeignKey("dbo.UserAccount", t => t.ProjectLeadId)
                .Index(t => t.ClientId, name: "IDX_ProjectClient")
                .Index(t => t.ProjectLeadId);
            
            CreateTable(
                "dbo.ClientEntity",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        EntityName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SubProject",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProjectId = c.Guid(nullable: false),
                        ProjectName = c.String(nullable: false, maxLength: 500),
                        DateCreated = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .Index(t => t.ProjectId, name: "IDX_SubProject");
            
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RoleName = c.String(nullable: false, maxLength: 100),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.RoleName, unique: true, name: "UIDX_RoleRoleName");
            
            CreateTable(
                "dbo.Privilege",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Security = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Security, unique: true, name: "IDX_PrivilegeSecurity");
            
            CreateTable(
                "dbo.TeamJobDesignations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserAccountId = c.Guid(nullable: false),
                        ClientId = c.Guid(nullable: false),
                        LineLeaderId = c.Guid(nullable: false),
                        JobDesignation = c.String(nullable: false, maxLength: 500),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Location = c.String(nullable: false, maxLength: 500),
                        UserAccount_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClientEntity", t => t.ClientId)
                .ForeignKey("dbo.UserAccount", t => t.LineLeaderId)
                .ForeignKey("dbo.UserAccount", t => t.UserAccountId)
                .ForeignKey("dbo.UserAccount", t => t.UserAccount_Id)
                .Index(t => t.UserAccountId, name: "IDX_TeamJobDesignationUserAccount")
                .Index(t => t.ClientId)
                .Index(t => t.LineLeaderId)
                .Index(t => t.UserAccount_Id);
            
            CreateTable(
                "dbo.TravelInformation",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserAccountId = c.Guid(nullable: false),
                        DocumentType = c.String(maxLength: 500),
                        Number = c.String(),
                        ExpiryDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserAccount", t => t.UserAccountId)
                .Index(t => t.UserAccountId, name: "IDX_TravelInformtionUserAccount");
            
            CreateTable(
                "dbo.Team",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TeamName = c.String(nullable: false, maxLength: 500),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AuditLog",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(),
                        UserName = c.String(maxLength: 200),
                        EventDate = c.DateTime(nullable: false),
                        EventType = c.Int(nullable: false),
                        TableName = c.String(nullable: false, maxLength: 200),
                        RecordId = c.Guid(),
                        ColumnName = c.String(nullable: false, maxLength: 200),
                        OriginalValue = c.String(),
                        NewValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.UserName, t.EventDate, t.EventType, t.TableName }, name: "IDX_AuditLog")
                .Index(t => t.UserName, name: "IDX_AuditUser")
                .Index(t => t.EventDate, name: "IDX_AuditDate")
                .Index(t => t.EventType, name: "IDX_EventType")
                .Index(t => t.TableName, name: "IDX_AuditTable");
            
            CreateTable(
                "dbo.BillingCycleEntry",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Cycle = c.Short(nullable: false),
                        Year = c.Short(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Weekdays = c.Short(nullable: false),
                        PublicHolidays = c.Short(nullable: false),
                        WorkDays = c.Short(nullable: false),
                        CreatedByAccountId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserAccount", t => t.CreatedByAccountId)
                .Index(t => t.CreatedByAccountId);
            
            CreateTable(
                "dbo.EmailQueue",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Processed = c.DateTime(),
                        ToAddress = c.String(nullable: false, maxLength: 500),
                        CCAddress = c.String(maxLength: 500),
                        Subject = c.String(nullable: false, maxLength: 500),
                        MessageBody = c.String(nullable: false),
                        SendError = c.String(maxLength: 1000),
                        SendAttempts = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.Status, t.Created }, name: "IDX_EmailQueue");
            
            CreateTable(
                "dbo.EmailAttachment",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmailQueueId = c.Guid(nullable: false),
                        FileName = c.String(nullable: false),
                        FileData = c.Binary(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailQueue", t => t.EmailQueueId)
                .Index(t => t.EmailQueueId, name: "IDX_EmailAttachment");
            
            CreateTable(
                "dbo.SystemLogs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EventTime = c.DateTime(nullable: false),
                        Sender = c.String(),
                        UserIdentityId = c.Guid(),
                        EventType = c.Int(nullable: false),
                        Message = c.String(),
                        StackTrace = c.String(),
                        InnerException = c.String(),
                        InnerExceptionStackTrace = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SystemParameter",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmailFromAddress = c.String(nullable: false, maxLength: 500),
                        EmailFromName = c.String(nullable: false, maxLength: 500),
                        AboutApp = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RolePrivilege",
                c => new
                    {
                        RoleId = c.Guid(nullable: false),
                        PrivilegeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleId, t.PrivilegeId })
                .ForeignKey("dbo.Role", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Privilege", t => t.PrivilegeId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.PrivilegeId);
            
            CreateTable(
                "dbo.AdminUserRole",
                c => new
                    {
                        AdminUserId = c.Guid(nullable: false),
                        RoleId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.AdminUserId, t.RoleId })
                .ForeignKey("dbo.UserAccount", t => t.AdminUserId, cascadeDelete: true)
                .ForeignKey("dbo.Role", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.AdminUserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.UserAccount",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProfileComplete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserIdentity", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserAccount", "Id", "dbo.UserIdentity");
            DropForeignKey("dbo.EmailAttachment", "EmailQueueId", "dbo.EmailQueue");
            DropForeignKey("dbo.BillingCycleEntry", "CreatedByAccountId", "dbo.UserAccount");
            DropForeignKey("dbo.TimesheetEntry", "UserAccountId", "dbo.UserAccount");
            DropForeignKey("dbo.TimesheetEntry", "TeamId", "dbo.Team");
            DropForeignKey("dbo.TimesheetEntry", "CreatedByAccountId", "dbo.UserAccount");
            DropForeignKey("dbo.TravelInformation", "UserAccountId", "dbo.UserAccount");
            DropForeignKey("dbo.TeamJobDesignations", "UserAccount_Id", "dbo.UserAccount");
            DropForeignKey("dbo.TeamJobDesignations", "UserAccountId", "dbo.UserAccount");
            DropForeignKey("dbo.TeamJobDesignations", "LineLeaderId", "dbo.UserAccount");
            DropForeignKey("dbo.TeamJobDesignations", "ClientId", "dbo.ClientEntity");
            DropForeignKey("dbo.AdminUserRole", "RoleId", "dbo.Role");
            DropForeignKey("dbo.AdminUserRole", "AdminUserId", "dbo.UserAccount");
            DropForeignKey("dbo.RolePrivilege", "PrivilegeId", "dbo.Privilege");
            DropForeignKey("dbo.RolePrivilege", "RoleId", "dbo.Role");
            DropForeignKey("dbo.TimesheetEntry", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.TimesheetEntry", "SubProjectId", "dbo.SubProject");
            DropForeignKey("dbo.SubProject", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.Project", "ProjectLeadId", "dbo.UserAccount");
            DropForeignKey("dbo.TimesheetEntry", "ClientEntity_Id", "dbo.ClientEntity");
            DropForeignKey("dbo.Project", "ClientId", "dbo.ClientEntity");
            DropForeignKey("dbo.UserIdentity", "ProfileImageDataId", "dbo.ImageData");
            DropForeignKey("dbo.PersonalInformation", "UserAccountId", "dbo.UserAccount");
            DropForeignKey("dbo.OfficeEquipments", "UserAccountId", "dbo.UserAccount");
            DropForeignKey("dbo.ScorecardPeriod", "ScorecardTemplatePeriodId", "dbo.ScorecardTemplatePeriod");
            DropForeignKey("dbo.ScorecardRecord", "ScorecardTemplateItemId", "dbo.ScorecardTemplateItem");
            DropForeignKey("dbo.ScorecardTemplateItemScore", "ScorecardTemplateItemId", "dbo.ScorecardTemplateItem");
            DropForeignKey("dbo.ScorecardTemplatePeriod", "ScorecardTemplateId", "dbo.ScorecardTemplate");
            DropForeignKey("dbo.ScorecardTemplateItem", "ScorecardTemplateId", "dbo.ScorecardTemplate");
            DropForeignKey("dbo.Scorecard", "ScorecardTemplateId", "dbo.ScorecardTemplate");
            DropForeignKey("dbo.ScorecardRecord", "ScorecardPeriodId", "dbo.ScorecardPeriod");
            DropForeignKey("dbo.ScorecardPeriod", "ScorecardId", "dbo.Scorecard");
            DropForeignKey("dbo.Scorecard", "EvaluatorId", "dbo.UserAccount");
            DropForeignKey("dbo.Scorecard", "EmployeeId", "dbo.UserAccount");
            DropForeignKey("dbo.EmergancyContact", "UserAccountId", "dbo.UserAccount");
            DropForeignKey("dbo.BillingRates", "UserAccountId", "dbo.UserAccount");
            DropForeignKey("dbo.TimesheetEntry", "ActivityId", "dbo.Activity");
            DropIndex("dbo.UserAccount", new[] { "Id" });
            DropIndex("dbo.AdminUserRole", new[] { "RoleId" });
            DropIndex("dbo.AdminUserRole", new[] { "AdminUserId" });
            DropIndex("dbo.RolePrivilege", new[] { "PrivilegeId" });
            DropIndex("dbo.RolePrivilege", new[] { "RoleId" });
            DropIndex("dbo.EmailAttachment", "IDX_EmailAttachment");
            DropIndex("dbo.EmailQueue", "IDX_EmailQueue");
            DropIndex("dbo.BillingCycleEntry", new[] { "CreatedByAccountId" });
            DropIndex("dbo.AuditLog", "IDX_AuditTable");
            DropIndex("dbo.AuditLog", "IDX_EventType");
            DropIndex("dbo.AuditLog", "IDX_AuditDate");
            DropIndex("dbo.AuditLog", "IDX_AuditUser");
            DropIndex("dbo.AuditLog", "IDX_AuditLog");
            DropIndex("dbo.TravelInformation", "IDX_TravelInformtionUserAccount");
            DropIndex("dbo.TeamJobDesignations", new[] { "UserAccount_Id" });
            DropIndex("dbo.TeamJobDesignations", new[] { "LineLeaderId" });
            DropIndex("dbo.TeamJobDesignations", new[] { "ClientId" });
            DropIndex("dbo.TeamJobDesignations", "IDX_TeamJobDesignationUserAccount");
            DropIndex("dbo.Privilege", "IDX_PrivilegeSecurity");
            DropIndex("dbo.Role", "UIDX_RoleRoleName");
            DropIndex("dbo.SubProject", "IDX_SubProject");
            DropIndex("dbo.Project", new[] { "ProjectLeadId" });
            DropIndex("dbo.Project", "IDX_ProjectClient");
            DropIndex("dbo.PersonalInformation", "IDX_PersonalInformationUserAccount");
            DropIndex("dbo.OfficeEquipments", "IDX_OfficeEquipmentUserAccount");
            DropIndex("dbo.ScorecardTemplateItemScore", new[] { "ScorecardTemplateItemId" });
            DropIndex("dbo.ScorecardTemplatePeriod", new[] { "ScorecardTemplateId" });
            DropIndex("dbo.ScorecardTemplateItem", new[] { "ScorecardTemplateId" });
            DropIndex("dbo.ScorecardRecord", new[] { "ScorecardTemplateItemId" });
            DropIndex("dbo.ScorecardRecord", new[] { "ScorecardPeriodId" });
            DropIndex("dbo.ScorecardPeriod", new[] { "ScorecardTemplatePeriodId" });
            DropIndex("dbo.ScorecardPeriod", new[] { "ScorecardId" });
            DropIndex("dbo.Scorecard", new[] { "EmployeeId" });
            DropIndex("dbo.Scorecard", new[] { "EvaluatorId" });
            DropIndex("dbo.Scorecard", new[] { "ScorecardTemplateId" });
            DropIndex("dbo.EmergancyContact", "IDX_EmergancyContactUserAccount");
            DropIndex("dbo.BillingRates", "IDX_BillingRatesUserAccount");
            DropIndex("dbo.UserIdentity", "UIDX_UserIdentityRegistered");
            DropIndex("dbo.UserIdentity", new[] { "ProfileImageDataId" });
            DropIndex("dbo.UserIdentity", "UIDX_UserIdentity");
            DropIndex("dbo.TimesheetEntry", new[] { "ClientEntity_Id" });
            DropIndex("dbo.TimesheetEntry", new[] { "CreatedByAccountId" });
            DropIndex("dbo.TimesheetEntry", new[] { "ActivityId" });
            DropIndex("dbo.TimesheetEntry", new[] { "TeamId" });
            DropIndex("dbo.TimesheetEntry", new[] { "SubProjectId" });
            DropIndex("dbo.TimesheetEntry", new[] { "ProjectId" });
            DropIndex("dbo.TimesheetEntry", new[] { "UserAccountId" });
            DropTable("dbo.UserAccount");
            DropTable("dbo.AdminUserRole");
            DropTable("dbo.RolePrivilege");
            DropTable("dbo.SystemParameter");
            DropTable("dbo.SystemLogs");
            DropTable("dbo.EmailAttachment");
            DropTable("dbo.EmailQueue");
            DropTable("dbo.BillingCycleEntry");
            DropTable("dbo.AuditLog");
            DropTable("dbo.Team");
            DropTable("dbo.TravelInformation");
            DropTable("dbo.TeamJobDesignations");
            DropTable("dbo.Privilege");
            DropTable("dbo.Role");
            DropTable("dbo.SubProject");
            DropTable("dbo.ClientEntity");
            DropTable("dbo.Project");
            DropTable("dbo.ImageData");
            DropTable("dbo.PersonalInformation");
            DropTable("dbo.OfficeEquipments");
            DropTable("dbo.ScorecardTemplateItemScore");
            DropTable("dbo.ScorecardTemplatePeriod");
            DropTable("dbo.ScorecardTemplate");
            DropTable("dbo.ScorecardTemplateItem");
            DropTable("dbo.ScorecardRecord");
            DropTable("dbo.ScorecardPeriod");
            DropTable("dbo.Scorecard");
            DropTable("dbo.EmergancyContact");
            DropTable("dbo.BillingRates");
            DropTable("dbo.UserIdentity");
            DropTable("dbo.TimesheetEntry");
            DropTable("dbo.Activity");
        }
    }
}
