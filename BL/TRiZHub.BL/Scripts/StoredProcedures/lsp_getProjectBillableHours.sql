USE [TRiZHub]
GO
/****** Object:  StoredProcedure [dbo].[lsp_getProjectBillableHours]    Script Date: 1/10/2017 9:55:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[lsp_getProjectBillableHours] 
	-- Add the parameters for the stored procedure here
		@StartDate datetime,
		@EndDate datetime,
		@projectID uniqueIdentifier AS
BEGIN
DECLARE @timesheet TABLE (
					Contact VARCHAR(MAX),
					DateEntry DateTime,
					Client NVarChar(MAX),					
					ProjectNumber VARCHAR(MAX),
					ProjectName VARCHAR(MAX),
					Billable bit,
					SubProjectNumber VARCHAR(MAX),
					SubProjectName VARCHAR(MAX),
					TeamName VARCHAR(MAX),
					ActivityName VARCHAR(MAX),
					Comments VARCHAR(MAX),
					Hours Decimal(18,5),
					Rate Decimal(18,5))

--exec [lsp_getProjectBillableHours] '01-Jan-2016 00:00:00','01-Jan-2017 00:00:00','5633e637-08e5-4b4e-8598-36bda0e27b18' --,''5633e637-08e5-4b4e-8598-36bda0e27b18'''
--exec [lsp_getTimesheetReportDetail] '01-Jan-2016 00:00:00','01-Jan-2017 00:00:00','All','All','All'
--set @userAccountID = 'A1B91489-5C47-4164-A20C-743A7684A3C6'
--set @billableClientsAccountID = '''C24B95FD-765B-4F85-B6DA-8DF1554EBCA8'',''8559FE4F-95C1-4CED-AB31-5061925095C4'''
--set @billableClientsAccountID = '''00000000-0000-0000-0000-000000000000'''

select case when sp.SubProjectNumber is null then p.ProjectNumber else p.ProjectNumber + ':' + sp.SubProjectNumber end  Phase, u.FirstName + ' ' + u.Surname as Contact, t.TeamName, a.ActivityName, sum(ts.Hours) Hours, br.Rate from TimesheetEntry ts
join UserIdentity u on ts.UserAccountId = u.Id 
	and ts.DateEntry >= @StartDate and ts.DateEntry < @EndDate 
    and ts.IsActive = 1
	and ts.ProjectId = @projectID
join Team t on ts.TeamId = t.Id
join Activity a on ts.ActivityId = a.Id
left join BillingRates br on ts.UserAccountId = br.UserAccountId and ts.DateEntry >= br.StartDate and DateEntry <= br.EndDate 
left join SubProject sp on ts.SubProjectId = sp.Id
left join Project p on ts.ProjectId = p.Id
group by case when sp.SubProjectNumber is null then p.ProjectNumber else p.ProjectNumber + ':' + sp.SubProjectNumber end , u.FirstName + ' ' + u.Surname , t.TeamName, a.ActivityName,  br.Rate
order by 1, a.ActivityName, t.TeamName, u.FirstName + ' ' + u.Surname 


END