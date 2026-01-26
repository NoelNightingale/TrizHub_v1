DELETE FROM UserIdentityProject

INSERT INTO UserIdentityProject
SELECT NEWID() as Id, 
	[UserAccountId],
 	   p.Id as ProjectId,
       null as SubProjectId
  FROM [TrizHub_Prod].[dbo].[TeamJobDesignation] tjd
  join ClientEntity c on c.Id = tjd.ClientId and GetDate() >= StartDate and getDate() <= EndDate
  join Project p on p.ClientId = c.Id and p.IsActive = 1



INSERT INTO UserIdentityProject
SELECT 
	NEWID() as Id, 
	[UserAccountId],
 	   p.Id as ProjectId,
       sp.Id as SubProjectId
  FROM [TrizHub_Prod].[dbo].[TeamJobDesignation] tjd
  join ClientEntity c on c.Id = tjd.ClientId and GetDate() >= StartDate and getDate() <= EndDate
  join Project p on p.ClientId = c.Id and p.IsActive = 1
  join SubProject sp on p.Id = sp.ProjectId and sp.IsActive = 1

INSERT INTO UserIdentityProject
  SELECT NEWID() as Id, 
	   ui.Id as UserAccountId,
 	   p.Id as ProjectId,
       null as SubProjectId
  FROM  ClientEntity c
  join UserIdentity ui on c.EntityName = 'TRIZ SA' 
  join Project p on p.ClientId = c.Id and p.IsActive = 1

INSERT INTO UserIdentityProject
  SELECT 
	NEWID() as Id, 
	   ui.Id as UserAccountId,
 	   p.Id as ProjectId,
       sp.Id as SubProjectId
  FROM  ClientEntity c
  join UserIdentity ui on c.EntityName = 'TRIZ SA' 
  join Project p on p.ClientId = c.Id and p.IsActive = 1
  join SubProject sp on p.Id = sp.ProjectId and sp.IsActive = 1