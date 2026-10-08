ALTER PROCEDURE [dbo].[lsp_getTimesheetReportDetail]
	-- Add the parameters for the stored procedure here
		@StartDate datetime,
		@EndDate datetime,
 		@userAccountIDs VARCHAR(max),
		@clientsAccountID VARCHAR(max),
		@projectIDs VARCHAR(max),
		@billable bit AS
BEGIN
DECLARE @timesheet TABLE (
					UserAccountId uniqueIdentifier,
					Contact VARCHAR(max),
					DateEntry DateTime,
					Client NVarChar(max),
					ProjectNumber VARCHAR(max),
					ProjectName VARCHAR(max),
					ProjectBillableType VARCHAR(MAX),
					Billable bit,
					SubProjectNumber VARCHAR(max),
					SubProjectName VARCHAR(max),
					SubProjectBillableType VARCHAR(MAX),
					TeamName VARCHAR(max),
					ActivityName VARCHAR(max),
					Comments VARCHAR(max),
					Hours Decimal(18,5),
					Rate Decimal(18,5))

Declare	@sql nvarchar(max)
--exec [lsp_getTimesheetReportDetail] '2021-01-25','2021-02-21','All','All','All',1
--exec [lsp_getTimesheetReportDetail] '01-Jan-2016 00:00:00','01-Jan-2017 00:00:00','All','All','All'
--set @userAccountID = 'A1B91489-5C47-4164-A20C-743A7684A3C6'
--set @billableClientsAccountID = '''C24B95FD-765B-4F85-B6DA-8DF1554EBCA8'',''8559FE4F-95C1-4CED-AB31-5061925095C4'''
--set @billableClientsAccountID = '''00000000-0000-0000-0000-000000000000'''
set @sql = 'select ts.UserAccountId, u.FirstName + '' '' + u.Surname as Contact, ts.DateEntry, c.EntityName Client, p.ProjectNumber, p.ProjectName, pt.Name, p.Billable,
		sp.SubProjectNumber, sp.ProjectName SubProjectName, spt.Name,
		t.TeamName, a.ActivityName, ts.Comments, ts.Hours, br.Rate from TimesheetEntry ts
		join UserIdentity u on ts.UserAccountId = u.Id '

set @sql = @sql + 'and ts.DateEntry >= @StartDate and ts.DateEntry < @EndDate '

IF @userAccountIDs != 'All' BEGIN
set @sql = @sql + ' and ts.UserAccountId in (' + @userAccountIDs + ')'
END
IF @projectIDs != 'All' BEGIN
set @sql = @sql + ' and ts.ProjectId in (' + @projectIDs + ')'
END

set @sql = @sql + ' join Project p on ts.ProjectId = p.Id '

IF @billable = 1 BEGIN
set @sql = @sql + ' and p.Billable = 1'
END


set @sql = @sql + ' join ClientEntity c on p.ClientId = c.Id '


IF @clientsAccountID != 'All' BEGIN
set @sql = @sql + ' and c.Id in (' + @clientsAccountID + ')'
END

set @sql = @sql + ' join Team t on ts.TeamId = t.Id
join Activity a on ts.ActivityId = a.Id
left join SubProject sp on ts.SubProjectId = sp.Id
left join ProjectType pt on pt.Id = p.ProjectTypeId
left join ProjectType spt on spt.Id = sp.SubProjectTypeId
left join BillingRates br on ts.UserAccountId = br.UserAccountId and ts.DateEntry >= br.StartDate and DateEntry <= br.EndDate'

--print @sql

insert into @timesheet EXEC sp_executesql @sql, N'@StartDate datetime, @EndDate datetime', @StartDate, @EndDate

select * from @timesheet
order by Contact, DateEntry, ProjectNumber

END