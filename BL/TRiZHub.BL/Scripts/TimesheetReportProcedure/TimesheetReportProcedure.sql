--Declare --@StartDate datetime,
		--@EndDate datetime,
		--@ShowPhases bit,
 		--@userAccountID VARCHAR(100),
		--@billableClientsAccountID NVARCHAR(max),
		--@nonBillableClientsAccountID NVARCHAR(max),

Declare @sql nvarchar(max)

--set @StartDate = '01-Jan-2016 00:00:00'
--set @EndDate = '01-Jan-2017 00:00:00'
set @ShowPhases = 1 
set @userAccountID = 'ALL'
set @billableClientsAccountID = 'N''C24B95FD-765B-4F85-B6DA-8DF1554EBCA8'',N''8559FE4F-95C1-4CED-AB31-5061925095C4'''
set @nonBillableClientsAccountID = 'N''C24B95FD-765B-4F85-B6DA-8DF1554EBCA8'',N''8559FE4F-95C1-4CED-AB31-5061925095C4'''
--set @billableClientsAccountID = '''00000000-0000-0000-0000-000000000000'''
--set @nonBillableClientsAccountID = '''00000000-0000-0000-0000-000000000000'''



DECLARE @timesheet TABLE (
					UserAccountId VARCHAR(100),
					ProjectId VARCHAR(100),
					SubProjectId VARCHAR(100),
					DateEntry DateTime,
					Hours Decimal(18,5),
					Billable bit,
					ClientID NVarChar(500),					
					Cost Decimal(18,5))


if @ShowPhases = 1 BEGIN
set @sql = 'select ts.UserAccountId, ts.ProjectId, ts.SubProjectId,ts.DateEntry, sum(ts.Hours) Hours, p.Billable, p.ClientId,  sum(Hours * Rate) as Cost '
END
ELSE BEGIN
set @sql = 'select ts.UserAccountId, ts.ProjectId, null SubProjectId,ts.DateEntry, sum(ts.Hours) Hours, p.Billable, p.ClientId,  sum(Hours * Rate) as Cost '
END

set @sql = @sql + 'from 
TimesheetEntry ts 
join Project p on ts.ProjectId = p.Id 
join ClientEntity c on p.ClientId = c.Id
left join BillingRates br on ts.UserAccountId = br.UserAccountId and DateEntry >= br.StartDate and DateEntry <= br.EndDate
where  DateEntry >= @StartDate and DateEntry < @EndDate '

IF @userAccountID != 'ALL' BEGIN
set @sql = @sql + ' and ts.UserAccountId = ''' + @userAccountID + ''''
END

IF @billableClientsAccountID != 'ALL' BEGIN
set @sql = @sql + ' and ((c.Id in (' + @billableClientsAccountID + ') and p.Billable = 1)'
END
ELSE
BEGIN 
set @sql = @sql + ' and ((p.Billable = 1)'
END  


IF @nonBillableClientsAccountID != 'ALL' BEGIN
set @sql = @sql + ' or (c.Id in (' + @nonBillableClientsAccountID + ') and p.Billable = 0))'
END
ELSE
BEGIN 
set @sql = @sql + ' or (p.Billable = 0))'
END  

if @ShowPhases = 1 BEGIN
	set @sql = @sql + ' group by ts.UserAccountId, ts.ProjectId, ts.SubProjectId ,ts.DateEntry, p.Billable, p.ClientId'
END
ELSE BEGIN
	set @sql = @sql + ' group by ts.UserAccountId, ts.ProjectId, ts.DateEntry, p.Billable, p.ClientId'
END
--print @sql

insert into @timesheet EXEC sp_executesql @sql, N'@StartDate datetime, @EndDate datetime', @StartDate, @EndDate
--select * from @timesheet

DECLARE @grid TABLE (UserAccountId VARCHAR(100),
					Person NVarChar(500),
					ProjectId VARCHAR(100),
					ProjectName NVarChar(500),
					SubProjectId VARCHAR(100),
					SubProjectName NVarChar(500),
					Billable bit,
					Client NVarChar(500))

Insert Into @grid select * from 
	(
	select distinct [UserAccountId], ui.FirstName + ' ' + ui.Surname Person from @timesheet t
	join UserIdentity ui on t.UserAccountId = ui.Id
	) t1,
	(
	select distinct t.ProjectId, isnull(p.ProjectNumber,'NA') + ': ' + p.ProjectName as ProjectName, t.SubProjectId, isnull(sp.SubProjectNumber,'NA') + ': ' + sp.ProjectName as SubProjectName, p.Billable, c.EntityName Client from @timesheet t 
	join project p on t.ProjectId = p.Id
	left join Subproject sp on t.SubProjectId = sp.Id
	join ClientEntity c on p.ClientId = c.Id
	) t2

--select * from @grid

DECLARE @time TABLE (UserAccountId VARCHAR(100),
					ProjectId VARCHAR(100),
					SubProjectId VARCHAR(100),
					[Hours] Decimal(18,5),
					[Cost] Decimal(18,5))

Insert Into @time 
	SELECT  t.[UserAccountId],t.[ProjectId],t.[SubProjectId], sum([Hours]) as Hours, sum(Cost) as Cost
	FROM @timesheet t
	group by [ProjectId], t.[UserAccountId], t.SubProjectId,t.Billable

--select * from @time

select a1.Billable,a1.Client, a1.ProjectId, a1.ProjectName, a1.SubProjectId, a1.SubProjectName, a1.UserAccountId, 
	a1.Person, isnull(a2.hours,0) Hours, isnull(a2.Cost,0) Cost 
	from @grid a1
	left join @time a2
	on a1.UserAccountId = a2.UserAccountId and a1.ProjectId = a2.ProjectId and a1.SubProjectId = a2.SubProjectId
	order by a1.Billable desc, a1.Client,a1.ProjectName,a1.SubProjectName, a1.Person


--select * from TimesheetEntry