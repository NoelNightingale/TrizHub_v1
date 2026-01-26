--DECLARE @StartDate DATETIME;
--SET @StartDate = '01 Jun 2014 00:00:00';

--DECLARE @EndDate DATETIME;
--SET @EndDate = '01 Jul 2014 00:00:00';
-- Maarnet om te toets



-- SET NOCOUNT ON added to prevent extra result sets from
-- interfering with SELECT statements.
SET NOCOUNT ON;

DECLARE @grid TABLE (UserAccountId VARCHAR(100),
					Person NVarChar(500),
					ProjectId VARCHAR(100),
					ProjectName NVarChar(500),
					Billable bit,
					Client NVarChar(500))

Insert Into @grid select * from 
	(select distinct [UserAccountId], ui.FirstName + ' ' + ui.Surname Person from [TimesheetEntry] t
	join UserIdentity ui on t.UserAccountId = ui.Id
	WHERE DateEntry >= @StartDate and DateEntry < @EndDate) t1,
		(select distinct ProjectId, isnull(p.ProjectNumber,'NA') + ': ' + p.ProjectName as ProjectName, p.Billable, c.EntityName Client from [TimesheetEntry] t 
		join project p on t.ProjectId = p.Id
		join ClientEntity c on p.ClientId = c.Id
		WHERE DateEntry >= @StartDate and DateEntry < @EndDate) t2

--select * from @grid

DECLARE @time TABLE (UserAccountId VARCHAR(100),
					ProjectId VARCHAR(100),
					[Hours] Decimal(18,5),
					[Cost] Decimal(18,5))

Insert Into @time 
	SELECT  t.[UserAccountId],t.[ProjectId], sum([Hours]) as Hours, sum(Hours * Rate) as Cost
	FROM [TimesheetEntry] t
	join project p on t.ProjectId = p.Id and DateEntry >= @StartDate and DateEntry < @EndDate
	left join BillingRates br on t.UserAccountId = br.UserAccountId and DateEntry >= br.StartDate and DateEntry <= br.EndDate
	group by [ProjectId], t.[UserAccountId], p.ProjectName,p.Billable

--select * from @time

select a1.Billable,a1.Client, a1.ProjectId, a1.ProjectName, a1.UserAccountId, 
	a1.Person, isnull(a2.hours,0) Hours, isnull(a2.Cost,0) Cost 
	from @grid a1
	left join @time a2
	on a1.UserAccountId = a2.UserAccountId and a1.ProjectId = a2.ProjectId
	order by a1.Billable desc, a1.Client,a1.ProjectName, a1.Person
