SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetProjectAllocationReport]
	@userAccountIDs VARCHAR(MAX),
	@onlyActiveUsers INT,
	@onlyActiveClients INT,
	@onlyActiveProjects INT,
	@onlyActiveSubProjects INT
AS
BEGIN

DECLARE	@sql NVARCHAR(MAX)

/* TODO: add active/inactive*/


SET @sql = 'SELECT CONCAT(ui.FirstName, '' '', ui.Surname) as [FullName], ui.Active as UserActive, c.EntityName [ClientName], c.IsActive as ClientActive, p.ProjectNumber, p.ProjectName, p.IsActive as ProjectActive, sp.SubProjectNumber, sp.ProjectName as SubProjectName, sp.IsActive as SubProjectActive
	FROM UserIdentityProject uip 
		join UserAccount ua on uip.UserAccountId = ua.id '

END

IF @userAccountIDs != 'All' 
BEGIN
	SET @sql = @sql + 'AND uip.UserAccountId in (' + @userAccountIDs + ') '
END

SET @sql = @sql + 'join UserIdentity ui 
	on ui.Id = uip.UserAccountId '
IF @onlyActiveUsers = 1 
BEGIN
	SET @sql = @sql + 'AND ui.Active = 1 '
END


SET @sql = @sql + '	join Project p 
		on p.Id = uip.ProjectId and p.IsDeleted = 0 '
IF @onlyActiveProjects = 1 
BEGIN
	SET @sql = @sql + 'AND p.IsActive = 1 '
END

SET @sql = @sql + ' join ClientEntity c 
	on p.ClientId = c.Id and c.IsDeleted = 0 '
IF @onlyActiveClients = 1 
BEGIN
	SET @sql = @sql + 'AND c.IsActive = 1 '
END

SET @sql = @sql + ' left join SubProject sp 
	on sp.Id = uip.SubProjectId and sp.IsDeleted = 0 '
IF @onlyActiveSubProjects = 1 
BEGIN
	SET @sql = @sql + 'AND sp.IsActive = 1 '
END


--SET @sql = @sql + ' group by CONCAT(ui.FirstName, '' '', ui.Surname), ui.Active, c.EntityName, c.IsActive, p.ProjectName, p.IsActive '

SET @sql = @sql + ' union all '


SET @sql = @sql + '  
Select CONCAT(ui.FirstName, '' '', ui.Surname) as [FullName],ui.Active as UserActive, c.EntityName [ClientName], c.IsActive as ClientActive, null ProjectNumber, null ProjectName, null as ProjectActive,null SubProjectNumber, null as SubProjectName, null as SubProjectActive 
	from UserIdentityClient uic
	join UserAccount ua on uic.UserAccountId = ua.id '
IF @userAccountIDs != 'All' 
BEGIN
	SET @sql = @sql + 'AND uic.UserAccountId in (' + @userAccountIDs + ') '
END

SET @sql = @sql + 'join UserIdentity ui 
	on ui.Id = uic.UserAccountId '
IF @onlyActiveUsers = 1 
BEGIN
	SET @sql = @sql + 'AND ui.Active = 1 '
END

SET @sql = @sql + ' join ClientEntity c 
	on uic.ClientId = c.Id and c.IsDeleted = 0 '
IF @onlyActiveClients = 1 
BEGIN
	SET @sql = @sql + 'AND c.IsActive = 1 '
END


SET	@sql = @sql + ' order by FullName, ClientName, ProjectName, SubProjectName ASC'



--print @sql

EXEC sp_executesql @sql

--EXEC [GetProjectAllocationReport] 'All', 1,1,1,1
--EXEC [GetProjectAllocationReport] '113bdb21-3dc3-4689-bf97-85235cc7a8db', 0,0,0,0
--exec sp_executesql N'exec [dbo].[GetProjectAllocationReport] @userAccountIDs,@onlyActiveUsers,@onlyActiveClients,@onlyActiveProjects,@onlyActiveSubProjects',N'@userAccountIDs nvarchar(38),@onlyActiveUsers bit,@onlyActiveClients bit,@onlyActiveProjects bit,@onlyActiveSubProjects bit',@userAccountIDs=N'''cbbdf74b-1014-4e38-9c17-328e124f17a1''',@onlyActiveUsers=0,@onlyActiveClients=0,@onlyActiveProjects=0,@onlyActiveSubProjects=0