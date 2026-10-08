SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
  WorkTeams: the User Project Allocation report shows effective allocations.

  Adds a [Source] column: 'Direct' for UserIdentityProject / UserIdentityClient rows, otherwise the work team name for
  allocations a person receives through a team membership (any role) that is current today, in an active team.

  Requires the AddWorkTeams migration. Deploy together with the ReportProvider change that reads [Source].
  Run against a local database first, never straight against production.
*/
ALTER PROCEDURE [dbo].[GetProjectAllocationReport]
	@userAccountIDs VARCHAR(MAX),
	@onlyActiveUsers INT,
	@onlyActiveClients INT,
	@onlyActiveProjects INT,
	@onlyActiveSubProjects INT
AS
BEGIN
SET NOCOUNT ON;

DECLARE @sql NVARCHAR(MAX)
DECLARE @userFilter NVARCHAR(MAX) = ''
DECLARE @userActive NVARCHAR(100) = ''
DECLARE @clientActive NVARCHAR(100) = ''
DECLARE @projectActive NVARCHAR(100) = ''
DECLARE @subProjectActive NVARCHAR(100) = ''

IF @userAccountIDs != 'All' SET @userFilter = ' AND ua.Id IN (' + @userAccountIDs + ') '
IF @onlyActiveUsers = 1 SET @userActive = ' AND ui.Active = 1 '
IF @onlyActiveClients = 1 SET @clientActive = ' AND c.IsActive = 1 '
IF @onlyActiveProjects = 1 SET @projectActive = ' AND p.IsActive = 1 '
IF @onlyActiveSubProjects = 1 SET @subProjectActive = ' AND sp.IsActive = 1 '

-- People (any role) in active teams whose membership is current today.
DECLARE @teamPeople NVARCHAR(MAX) = '
	(SELECT DISTINCT m.UserAccountId, m.WorkTeamId, wt.Name AS TeamName
	 FROM WorkTeamMember m
		JOIN WorkTeam wt ON wt.Id = m.WorkTeamId AND wt.IsActive = 1
	 WHERE m.StartDate <= CAST(GETDATE() AS date)
		AND (m.EndDate IS NULL OR m.EndDate >= CAST(GETDATE() AS date))) tm '

SET @sql = '
SELECT CONCAT(ui.FirstName, '' '', ui.Surname) AS [FullName], ui.Active AS UserActive, c.EntityName AS [ClientName], c.IsActive AS ClientActive,
	p.ProjectNumber, p.ProjectName, p.IsActive AS ProjectActive, sp.SubProjectNumber, sp.ProjectName AS SubProjectName, sp.IsActive AS SubProjectActive,
	''Direct'' AS [Source]
FROM UserIdentityProject uip
	JOIN UserAccount ua ON uip.UserAccountId = ua.Id ' + @userFilter + '
	JOIN UserIdentity ui ON ui.Id = uip.UserAccountId ' + @userActive + '
	JOIN Project p ON p.Id = uip.ProjectId AND p.IsDeleted = 0 ' + @projectActive + '
	JOIN ClientEntity c ON p.ClientId = c.Id AND c.IsDeleted = 0 ' + @clientActive + '
	LEFT JOIN SubProject sp ON sp.Id = uip.SubProjectId AND sp.IsDeleted = 0 ' + @subProjectActive + '

UNION ALL

SELECT CONCAT(ui.FirstName, '' '', ui.Surname) AS [FullName], ui.Active AS UserActive, c.EntityName AS [ClientName], c.IsActive AS ClientActive,
	NULL AS ProjectNumber, NULL AS ProjectName, NULL AS ProjectActive, NULL AS SubProjectNumber, NULL AS SubProjectName, NULL AS SubProjectActive,
	''Direct'' AS [Source]
FROM UserIdentityClient uic
	JOIN UserAccount ua ON uic.UserAccountId = ua.Id ' + @userFilter + '
	JOIN UserIdentity ui ON ui.Id = uic.UserAccountId ' + @userActive + '
	JOIN ClientEntity c ON uic.ClientId = c.Id AND c.IsDeleted = 0 ' + @clientActive + '

UNION ALL

SELECT CONCAT(ui.FirstName, '' '', ui.Surname) AS [FullName], ui.Active AS UserActive, c.EntityName AS [ClientName], c.IsActive AS ClientActive,
	p.ProjectNumber, p.ProjectName, p.IsActive AS ProjectActive, sp.SubProjectNumber, sp.ProjectName AS SubProjectName, sp.IsActive AS SubProjectActive,
	tm.TeamName AS [Source]
FROM ' + @teamPeople + '
	JOIN WorkTeamProject wtp ON wtp.WorkTeamId = tm.WorkTeamId
	JOIN UserAccount ua ON tm.UserAccountId = ua.Id ' + @userFilter + '
	JOIN UserIdentity ui ON ui.Id = tm.UserAccountId ' + @userActive + '
	JOIN Project p ON p.Id = wtp.ProjectId AND p.IsDeleted = 0 ' + @projectActive + '
	JOIN ClientEntity c ON p.ClientId = c.Id AND c.IsDeleted = 0 ' + @clientActive + '
	LEFT JOIN SubProject sp ON sp.Id = wtp.SubProjectId AND sp.IsDeleted = 0 ' + @subProjectActive + '

UNION ALL

SELECT CONCAT(ui.FirstName, '' '', ui.Surname) AS [FullName], ui.Active AS UserActive, c.EntityName AS [ClientName], c.IsActive AS ClientActive,
	NULL AS ProjectNumber, NULL AS ProjectName, NULL AS ProjectActive, NULL AS SubProjectNumber, NULL AS SubProjectName, NULL AS SubProjectActive,
	tm.TeamName AS [Source]
FROM ' + @teamPeople + '
	JOIN WorkTeamClient wtc ON wtc.WorkTeamId = tm.WorkTeamId
	JOIN UserAccount ua ON tm.UserAccountId = ua.Id ' + @userFilter + '
	JOIN UserIdentity ui ON ui.Id = tm.UserAccountId ' + @userActive + '
	JOIN ClientEntity c ON wtc.ClientId = c.Id AND c.IsDeleted = 0 ' + @clientActive + '

ORDER BY FullName, ClientName, ProjectName, SubProjectName, [Source] ASC'

--PRINT @sql

EXEC sp_executesql @sql

END
GO

--EXEC [GetProjectAllocationReport] 'All', 1,1,1,1
--EXEC [GetProjectAllocationReport] '''113bdb21-3dc3-4689-bf97-85235cc7a8db''', 0,0,0,0
