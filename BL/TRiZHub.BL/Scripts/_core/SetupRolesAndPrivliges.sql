-- Migration script:

--SELECT * FROM UserIdentity where FirstName = 'Jean'
--0A339D3E-5C50-46AE-9033-197153348971
--UPDATE UserIdentity set IsSystemAdmin = 0 where Id = '0A339D3E-5C50-46AE-9033-197153348971'

-- Delete old Role Permissions
Delete from RolePrivilege

--SELECT * FROM RolePrivilege rp
--join Privilege p on p.Id = rp.PrivilegeId
--join Role r on r.Id = rp.RoleId
--where RoleName = 'General Employees'


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
	(NEWID(), 7, 'ReportGenerationUserProjects'),
	(NEWID(), 8, 'ReportGenerationUserRoles'),
	(NEWID(), 9, 'UserProjectMaintenance'),
	(NEWID(), 10, 'BillingCycleMaintenance'),
	(NEWID(), 11, 'TeamMaintenance'),
	(NEWID(), 12, 'TimesheetCapture'),
	(NEWID(), 13, 'TimesheetCaptureForOtherAccounts'),
	(NEWID(), 14, 'UserMaintenance'),
	(NEWID(), 15, 'UserEmergencyContactMaintenance'),
	(NEWID(), 16, 'UserTravelInformationMaintenance'),
	(NEWID(), 17, 'UserBillingRatesMaintenance'),
	(NEWID(), 18, 'UserAssetRegisterMaintenance'),
	(NEWID(), 19, 'UserPersonalInformationMaintenance'),
	(NEWID(), 20, 'UserTeamJobDesignationMaintenance'),
	(NEWID(), 21, 'ScorecardTemplateMaintenance'),
	(NEWID(), 22, 'PerformanceManagementCreateScoreCards'),
	(NEWID(), 23, 'PerformanceManagementViewMyScoreCards'),
	(NEWID(), 24, 'PerformanceManagementViewMyTeamScoreCards'),	
	(NEWID(), 25, 'CustomerReportAccess'),
	(NEWID(), 26, 'EmployerMaintenance'),
	(NEWID(), 27, 'PerformanceManagementAdmin')

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