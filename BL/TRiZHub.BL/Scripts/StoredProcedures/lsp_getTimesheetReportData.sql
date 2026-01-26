USE [TRiZHub]
GO
/****** Object:  StoredProcedure [dbo].[lsp_getTimesheetReportData]    Script Date: 04/10/2019 08:57:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[lsp_getTimesheetReportData] 
	-- Add the parameters for the stored procedure here
		@StartDate DATETIME,
		@EndDate DATETIME,
		@ShowPhases BIT,
		@OnlyBillable BIT,
 		@userAccountID VARCHAR(MAX),
		@clientsAccountID VARCHAR(MAX),
		@projectIDs VARCHAR(MAX),
		@projectWildCardSearch VARCHAR(MAX) AS
BEGIN
DECLARE @timesheet TABLE (
					UserAccountId VARCHAR(MAX),
					ProjectId VARCHAR(MAX),
					SubProjectId VARCHAR(MAX),
					DateEntry DATETIME,
					Hours DECIMAL(18,5),
					Billable BIT,
					ClientID NVARCHAR(MAX),
					Cost DECIMAL(18,5)
					)

DECLARE	@sql NVARCHAR(MAX)

IF @ShowPhases = 1 BEGIN
SET @sql = ' select ts.UserAccountId, 
					ts.ProjectId, 
					ts.SubProjectId,
					ts.DateEntry, 
					sum(ts.Hours) Hours, 
					p.Billable, 
					p.ClientId,  
					sum(Hours * Rate) as Cost '
END
ELSE BEGIN
SET @sql = ' select ts.UserAccountId, 
					ts.ProjectId, 
					null SubProjectId,
					ts.DateEntry, 
					sum(ts.Hours) Hours,
					p.Billable, 
					p.ClientId, 
					sum(Hours * Rate) as Cost '
END

SET @sql = @sql + ' from TimesheetEntry ts 
					join Project p 
					  on ts.ProjectId = p.Id 
					join ClientEntity c 
					  on p.ClientId = c.Id
					left join BillingRates br 
					  on ts.UserAccountId = br.UserAccountId 
					 and DateEntry >= br.StartDate 
					 and DateEntry <= br.EndDate
				   where DateEntry >= @StartDate 
				     and DateEntry < @EndDate '

IF @userAccountID != 'All' BEGIN
SET @sql = @sql + ' and ts.UserAccountId in (' + @userAccountID + ')'
END

IF @OnlyBillable = 1 BEGIN
SET @sql = @sql + ' and p.Billable = 1 '
END

IF @clientsAccountID != 'All' BEGIN
SET @sql = @sql + ' and c.Id in (' + @clientsAccountID + ')'
END

IF @projectIDs != 'All' BEGIN
SET @sql = @sql + ' and ts.ProjectId in (' + @projectIDs + ')'
END

IF @ShowPhases = 1 BEGIN
	SET @sql = @sql + ' group by ts.UserAccountId, 
								 ts.ProjectId, 
								 ts.SubProjectId,
								 ts.DateEntry, 
								 p.Billable, 
								 p.ClientId'
END
ELSE BEGIN
	SET @sql = @sql + ' group by ts.UserAccountId, 
								 ts.ProjectId, 
								 ts.DateEntry, 
								 p.Billable, 
								 p.ClientId'
END

--print @sql

INSERT INTO @timesheet EXEC sp_executesql @sql, N'@StartDate datetime, @EndDate datetime', @StartDate, @EndDate
--select * from @timesheet

DECLARE @grid TABLE (UserAccountId VARCHAR(100),
					Person NVarChar(500),
					ProjectId VARCHAR(100),
					ProjectName NVarChar(500),
					ProjectType NVarChar(MAX),
					SubProjectId VARCHAR(100),
					SubProjectName NVarChar(500),
					SubProjectTypeId NVarChar(MAX),					
					Billable BIT,
					Client NVarChar(500))

INSERT 
  INTO @grid 
SELECT * 
  FROM (
		SELECT DISTINCT [UserAccountId],
			   ui.FirstName + ' ' + ui.Surname Person 
		  FROM @timesheet t
		  JOIN UserIdentity ui 
			ON t.UserAccountId = ui.Id
	   ) t1,
	   (
		SELECT DISTINCT t.ProjectId, 
			   ISNULL(p.ProjectNumber,'NA') + ': ' + p.ProjectName AS ProjectName, 
			   pt.Name,
			   t.SubProjectId, 
			   ISNULL(p.ProjectNumber,'NA') + ': ' + ISNULL(sp.SubProjectNumber,'NA') + ' ' + sp.ProjectName AS SubProjectName, 
			   sp.SubProjectTypeId,
			   p.Billable, 
			   c.EntityName Client 
		  FROM @timesheet t 
		  JOIN project p 
			ON t.ProjectId = p.Id
		  JOIN ProjectType pt 
			ON p.ProjectTypeId = pt.Id		  
		  LEFT JOIN Subproject sp
			ON t.SubProjectId = sp.Id
		  JOIN ClientEntity c 
			ON p.ClientId = c.Id
	    ) t2

DECLARE @time TABLE (UserAccountId VARCHAR(100),
					ProjectId VARCHAR(100),
					SubProjectId VARCHAR(100),
					[Hours] DECIMAL(18,5),
					[Cost] DECIMAL(18,5))

INSERT 
  INTO @time 
SELECT t.[UserAccountId],
       t.[ProjectId],
	   t.[SubProjectId], 
	   sum([Hours]) AS Hours, 
	   sum(Cost) AS Cost
  FROM @timesheet t
 GROUP BY [ProjectId], 
       t.[UserAccountId], 
	   t.SubProjectId,
	   t.Billable

--select * from @time
IF(@projectWildCardSearch IS NULL OR @projectWildCardSearch = '*')
BEGIN
SELECT a1.Billable,
       a1.Client, 
	   a1.ProjectId, 
	   a1.ProjectName, 
	   a1.ProjectType,
	   a1.SubProjectId, 
	   ISNULL(a1.SubProjectName,'') PhaseName, 	  
	   (SELECT TOP 1 Name FROM ProjectType where Id = a1.SubProjectTypeId) as SubProjectType,
	   a1.UserAccountId, 
	   a1.Person, 
	   (SELECT EntityName FROM ClientEntity WHERE Id = (SELECT TOP 1 ClientId FROM TeamJobDesignation WHERE UserAccountId = a1.UserAccountId ORDER BY EndDate DESC)) as CurrentClientName,
	   ISNULL(a2.HOURS,0) Hours, 
	   ISNULL(a2.Cost,0) Cost 
  FROM @grid a1
  LEFT JOIN @time a2
	ON a1.UserAccountId = a2.UserAccountId 
   AND a1.ProjectId = a2.ProjectId 
   AND ISNULL(a1.SubProjectId,-1) = ISNULL(a2.SubProjectId,-1)
 ORDER BY a1.Billable DESC, 
       a1.Client,
	   a1.ProjectName,
	   a1.SubProjectName, 
	   a1.Person;
END
ELSE
BEGIN
	SELECT a1.Billable,
		   a1.Client, 
		   a1.ProjectId, 
		   a1.ProjectName, 
		   a1.ProjectType,
		   a1.SubProjectId, 
		   ISNULL(a1.SubProjectName,'') PhaseName, 
		   (SELECT TOP 1 Name FROM ProjectType where Id = a1.SubProjectTypeId) as SubProjectType,
		   a1.UserAccountId, 
		   a1.Person, 
		   (SELECT EntityName FROM ClientEntity WHERE Id = (SELECT TOP 1 ClientId FROM TeamJobDesignation WHERE UserAccountId = a1.UserAccountId ORDER BY EndDate DESC)) as CurrentClientName,
		   ISNULL(a2.HOURS,0) Hours, 		   
		   ISNULL(a2.Cost,0) Cost 
	  FROM @grid a1
	  LEFT JOIN @time a2
		ON a1.UserAccountId = a2.UserAccountId 
	   AND a1.ProjectId = a2.ProjectId 
	   AND ISNULL(a1.SubProjectId,-1) = ISNULL(a2.SubProjectId,-1)
	 WHERE a1.SubProjectName like CASE WHEN (@projectWildCardSearch IS NULL OR @projectWildCardSearch = '*') THEN a1.SubProjectName ELSE '%'+@projectWildCardSearch+'%' END
		OR a1.ProjectName like CASE WHEN (@projectWildCardSearch IS NULL OR @projectWildCardSearch = '*') THEN a1.ProjectName ELSE '%'+@projectWildCardSearch+'%' END
	 ORDER BY a1.Billable DESC, 
		   a1.Client,
		   a1.ProjectName,
		   a1.SubProjectName, 
		   a1.Person
END
END
