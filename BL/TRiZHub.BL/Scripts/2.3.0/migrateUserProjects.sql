  drop table if exists #projectIDs

declare @now datetime = GETDATE()

SELECT p.Id [ProjectId], sp.Id as [SubProjectId] INTO #projectIDs 
from Project p 
	left join SubProject sp on sp.ProjectId = p.Id
where p.IsActive = 1 AND sp.IsActive = 1


--SELECT * FROM #projectIDs

insert into UserIdentityProject(Id, UserAccountId, ProjectId, SubProjectId)
SELECT NEWID(), ui.id as [UserIdentityId], [pi].ProjectId, [pi].SubProjectId
FROM dbo.UserIdentity ui
	CROSS JOIN #projectIDs [pi]
	WHERE ui.Active = 1