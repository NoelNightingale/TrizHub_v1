USE [TRiZHub]
GO
/****** Object:  StoredProcedure [dbo].[lsp_getScoreCardSummary]    Script Date: 04/10/2019 08:59:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[lsp_getScoreCardSummary] 
		@reviewYears VARCHAR(MAX),
		@reviewPeriods VARCHAR(MAX),
		@submitted int,
		@locked int,
		@employeeHasScorecard int,
 		@employees VARCHAR(MAX),
		@clientsAccountID VARCHAR(MAX),
		@lineManagers VARCHAR(MAX),
		@evaluators VARCHAR(MAX),
		@scorecards VARCHAR(MAX)
		AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @reportitems TABLE (
					EmployeeAccountId uniqueidentifier,
					EmployeeName VARCHAR(100),
					EmployeeIsActive VARCHAR(10),
					ScorecardID uniqueidentifier,					
					ScorecardName VARCHAR(50),
					ScorecardVariableStart DateTime NULL,
					ScorecardVariableEnd DateTime NULL,
					ScorecardVariableYear INT NULL,
					ReviewYear INT,
					StartDate DateTime,
					EndDate DateTime,
					LineManagerName VARCHAR(50),
					EvaluatorId uniqueidentifier,
					EvaluatorFirstName VARCHAR(50),
					EvaluatorSurname VARCHAR(50),
					DateCreated DateTime,
					Submitted VARCHAR(10),
					locked VARCHAR(10))


	Declare @sctSql varchar(max)
	Set @sctSql = 'select * from ScorecardTemplatePeriod where Id = ID '
	If @reviewYears != 'All' begin
		SET @sctSql = @sctSql + + ' and reviewYear in (' + @reviewYears + ')'
	end 
	if @reviewPeriods != 'All' begin
		SET @sctSql = @sctSql + + ' and Id in (' + @reviewPeriods + ')'
	end 
	
	
	Declare	@sql nvarchar(max)
	SET @sql = 'SELECT emp.Id as EmployeeAccountId
		  ,emp.FirstName + '' '' + isnull(emp.Surname,'''') as EmployeeName
		  ,case when emp.Active = 1 then ''True'' else ''False'' end as EmployeeIsActive
		  ,s.Id as ScorecardID
		  ,st.ScorecardName
		  ,s.VariableStart
		  ,s.VariableEnd
		  ,s.VariableYear
		  ,stp.ReviewYear
		  ,stp.StartDate
		  ,stp.EndDate
		  ,ll.FirstName + '' '' + ll.Surname as LineManagerName
		  ,s.EvaluatorId
		  ,eval.FirstName as EvaluatorFirstName
		  ,eval.Surname as EvaluatorSurname
		  ,s.DateCreated
		  ,case when s.Completed = 1 then ''True'' else ''False'' end as Submitted
		  ,case when s.locked = 1 then ''True'' else ''False'' end as locked
	FROM UserIdentity emp '

	IF @employeeHasScorecard != 1 BEGIN
		SET @sql = @sql + 'cross join  (' + @sctSql + ') stp   
		  left join Scorecard s on emp.Id = s.EmployeeId and s.ScorecardTemplatePeriodId = stp.Id '
	END 
	ELSE BEGIN
		SET @sql = @sql + 'join Scorecard s on emp.Id = s.EmployeeId '
		IF @locked = 0 BEGIN SET @sql = @sql + ' and s.locked = 1 ' END
		IF @locked = 1 BEGIN SET @sql = @sql + ' and s.locked = 0 ' END
		IF @submitted = 0 BEGIN SET @sql = @sql + ' and s.completed = 1 ' END
		IF @submitted = 1 BEGIN SET @sql = @sql + ' and s.completed = 0 ' END
		IF @employees != 'All' BEGIN SET @sql = @sql + 'and s.EmployeeId in (' + @employees + ')' END 
		SET @sql = @sql + 'join (' + @sctSql + ') stp on s.ScorecardTemplatePeriodId = stp.Id '
	END
	SET @sql = @sql + 'join UserIdentity eval on eval.Id = s.EvaluatorId '
	IF @evaluators != 'All' BEGIN
		SET @sql = @sql + + ' and eval.Id in (' + @evaluators + ')'
	END

    SET @sql = @sql + ' join ScorecardTemplate st on s.ScorecardTemplateId = st.Id '
	IF @scorecards != 'All' BEGIN
		SET @sql = @sql + + ' and st.Id in (' + @scorecards + ')'
	END

	IF @lineManagers != 'All' or @clientsAccountID != 'All' BEGIN
		SET @sql = @sql + ' join TeamJobDesignation tjd on emp.Id = tjd.UserAccountId and s.DateCreated >= tjd.StartDate and s.DateCreated <= tjd.EndDate '
		IF @lineManagers != 'All' BEGIN
			SET @sql = @sql + ' and tjd.LineLeaderId in (' + @lineManagers + ')'
		END
		IF @clientsAccountID != 'All' BEGIN
			SET @sql = @sql + ' and tjd.ClientId in (' + @clientsAccountID + ')'
		END
		 
		SET @sql = @sql + '	join UserIdentity ll on tjd.LineLeaderId = ll.Id '
	END
	ELSE BEGIN
		SET @sql = @sql + ' Left join TeamJobDesignation tjd on emp.Id = tjd.UserAccountId and s.DateCreated >= tjd.StartDate and s.DateCreated <= tjd.EndDate 
			  Left join UserIdentity ll on tjd.LineLeaderId = ll.Id '
	END


--	print @sql

	insert into @reportitems EXEC sp_executesql @sql

	select * from @reportitems
	order by EmployeeName, ScorecardName, ReviewYear, StartDate, EndDate, LineManagerName, DateCreated, Submitted, locked

END
