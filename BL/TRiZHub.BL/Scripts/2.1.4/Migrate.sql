-- Migration script:

-- Delete old Role Permissions
Delete from RolePrivilege

-- Delete old Privelages
Delete from Privilege

-- Create New Privelages
Insert into Privilege (Id, Security, Description) values
	(NEWID(), 0, 'RoleMaintenance'),
	(NEWID(), 1, 'ClientMaintenance'),
	(NEWID(), 2, 'ProjectMaintenance'),
	(NEWID(), 3, 'ActivityMaintenance'),
	(NEWID(), 4, 'ReportGenerationTimesheet'),
	(NEWID(), 5, 'ReportGenerationUserSummary'),
	(NEWID(), 6, 'ReportGenerationScoreCard'),
	(NEWID(), 7, 'BillingCycleMaintenance'),
	(NEWID(), 8, 'TeamMaintenance'),
	(NEWID(), 9, 'TimesheetCapture'),
	(NEWID(), 10, 'TimesheetCaptureForOtherAccounts'),
	(NEWID(), 11, 'UserMaintenance'),
	(NEWID(), 12, 'UserEmergencyContactMaintenance'),
	(NEWID(), 13, 'UserTravelInformationMaintenance'),
	(NEWID(), 14, 'UserBillingRatesMaintenance'),
	(NEWID(), 15, 'UserAssetRegisterMaintenance'),
	(NEWID(), 16, 'UserPersonalInformationMaintenance'),
	(NEWID(), 17, 'UserTeamJobDesignationMaintenance'),
	(NEWID(), 18, 'ScorecardTemplateMaintenance'),
	(NEWID(), 19, 'PerformanceManagementCreateScoreCards'),
	(NEWID(), 20, 'PerformanceManagementViewMyScoreCards'),
	(NEWID(), 21, 'PerformanceManagementViewMyTeamScoreCards'),	
	(NEWID(), 22, 'CustomerReportAccess'),
	(NEWID(), 23, 'EmployerMaintenance'),
	(NEWID(), 24, 'PerformanceManagementAdmin')

-- Assign Privelages to Roles

-- Admin
INSERT INTO RolePrivilege
Select r.Id, p.Id from Privilege p 
join Role r on RoleName = 'Admin'

-- Autocar Engineers
INSERT INTO RolePrivilege
Select r.Id, p.Id from Privilege p 
join Role r on RoleName = 'Autocar Engineers'
where p.Description IN('TimesheetCapture')

-- Autocar/Phoenix GL
INSERT INTO RolePrivilege
Select r.Id, p.Id from Privilege p 
join Role r on RoleName = 'Autocar/Phoenix GL'
where p.Description IN('PerformanceManagementCreateScoreCards')

-- Customer Lead Engineer
INSERT INTO RolePrivilege
Select r.Id, p.Id from Privilege p 
join Role r on RoleName = 'Customer Lead Engineer'
where p.Description IN('PerformanceManagementCreateScoreCards', 'PerformanceManagementViewMyScoreCards', 'PerformanceManagementViewMyTeamScoreCards')

-- Customer Reports
INSERT INTO RolePrivilege
Select r.Id, p.Id from Privilege p 
join Role r on RoleName = 'Customer Reports'
where p.Description IN('CustomerReportAccess')

-- Engineering Managers
INSERT INTO RolePrivilege
Select r.Id, p.Id from Privilege p 
join Role r on RoleName = 'Engineering Managers'
where p.Description IN('ClientMaintenance', 'PerformanceManagementCreateScoreCards','PerformanceManagementViewMyScoreCards','PerformanceManagementViewMyTeamScoreCards',
						'ProjectMaintenance','ReportGenerationTimesheet','ReportGenerationUserSummary','ReportGenerationScoreCard','ScorecardTemplateMaintenance', 
						'TimesheetCapture', 'TimesheetCaptureForOtherAccounts', 'UserTeamJobDesignationMaintenance')

-- Finance Management
INSERT INTO RolePrivilege
Select r.Id, p.Id from Privilege p 
join Role r on RoleName = 'Finance Management'
where p.Description IN('BillingCycleMaintenance','ReportGenerationTimesheet','TeamMaintenance','TimesheetCapture',
						'TimesheetCaptureForOtherAccounts','UserBillingRatesMaintenance')

-- General Employees
INSERT INTO RolePrivilege
Select r.Id, p.Id from Privilege p 
join Role r on RoleName = 'General Employees'
where p.Description IN('PerformanceManagementViewMyScoreCards','TimesheetCapture')

-- Group Leaders
INSERT INTO RolePrivilege
Select r.Id, p.Id from Privilege p 
join Role r on RoleName = 'Group Leaders'
where p.Description IN('PerformanceManagementCreateScoreCards','PerformanceManagementViewMyScoreCards', 'PerformanceManagementViewMyTeamScoreCards', 'TimesheetCapture', 
						'TimesheetCaptureForOtherAccounts')

-- HR Employee Information Management
INSERT INTO RolePrivilege
Select r.Id, p.Id from Privilege p 
join Role r on RoleName = 'HR Employee Information Management'
where p.Description IN('EmployerMaintenance','UserAssetRegisterMaintenance', 'UserEmergencyContactMaintenance', 'UserMaintenance', 'UserPersonalInformationMaintenance',
						'UserTeamJobDesignationMaintenance', 'UserTravelInformationMaintenance')

-- Project Manager
INSERT INTO RolePrivilege
Select r.Id, p.Id from Privilege p 
join Role r on RoleName = 'Project Manager'
where p.Description IN('ClientMaintenance','PerformanceManagementCreateScoreCards', 'PerformanceManagementViewMyScoreCards', 'PerformanceManagementViewMyTeamScoreCards', 
						'ProjectMaintenance', 'TimesheetCapture', 'TimesheetCaptureForOtherAccounts')

-- Talent Management
INSERT INTO RolePrivilege
Select r.Id, p.Id from Privilege p 
join Role r on RoleName = 'Talent Management'
where p.Description IN('PerformanceManagementCreateScoreCards', 'PerformanceManagementViewMyScoreCards', 'ReportGenerationScoreCard', 'TimesheetCapture')