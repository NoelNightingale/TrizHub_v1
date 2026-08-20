-- Create TimesheetTemplate tables (run if EF Add-Migration / Update-Database is not used yet).
-- Safe to re-run: checks for table existence.

IF OBJECT_ID(N'dbo.TimesheetTemplate', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TimesheetTemplate (
        Id uniqueidentifier NOT NULL PRIMARY KEY,
        UserAccountId uniqueidentifier NOT NULL,
        Name nvarchar(200) NOT NULL,
        TemplateType nvarchar(20) NOT NULL,
        IsActive bit NOT NULL,
        DateCreated datetime NOT NULL,
        DateModified datetime NOT NULL,
        CONSTRAINT FK_TimesheetTemplate_UserAccount FOREIGN KEY (UserAccountId) REFERENCES dbo.UserAccount(Id)
    );
    CREATE INDEX IX_TimesheetTemplate_UserAccountId ON dbo.TimesheetTemplate(UserAccountId);
END
GO

IF OBJECT_ID(N'dbo.TimesheetTemplateItem', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TimesheetTemplateItem (
        Id uniqueidentifier NOT NULL PRIMARY KEY,
        TimesheetTemplateId uniqueidentifier NOT NULL,
        DayOffset int NOT NULL,
        SortOrder int NOT NULL,
        ProjectId uniqueidentifier NOT NULL,
        SubProjectId uniqueidentifier NULL,
        TeamId uniqueidentifier NOT NULL,
        ActivityId uniqueidentifier NOT NULL,
        Hours decimal(18,2) NOT NULL,
        Comments nvarchar(max) NOT NULL,
        ProjectDescription nvarchar(500) NULL,
        ClientEntityName nvarchar(500) NULL,
        Billable bit NULL,
        CONSTRAINT FK_TimesheetTemplateItem_Template FOREIGN KEY (TimesheetTemplateId) REFERENCES dbo.TimesheetTemplate(Id) ON DELETE CASCADE,
        CONSTRAINT FK_TimesheetTemplateItem_Project FOREIGN KEY (ProjectId) REFERENCES dbo.Project(Id),
        CONSTRAINT FK_TimesheetTemplateItem_SubProject FOREIGN KEY (SubProjectId) REFERENCES dbo.SubProject(Id),
        CONSTRAINT FK_TimesheetTemplateItem_Team FOREIGN KEY (TeamId) REFERENCES dbo.Team(Id),
        CONSTRAINT FK_TimesheetTemplateItem_Activity FOREIGN KEY (ActivityId) REFERENCES dbo.Activity(Id)
    );
    CREATE INDEX IX_TimesheetTemplateItem_TemplateId ON dbo.TimesheetTemplateItem(TimesheetTemplateId);
END
GO
