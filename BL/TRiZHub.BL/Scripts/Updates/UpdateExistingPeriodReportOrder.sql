USE [TRiZHub]
GO

--SELECT * FROM [dbo].[ScorecardTemplatePeriod] ORDER BY ScorecardTemplateId, StartDate
--UPDATE [dbo].[ScorecardTemplatePeriod] SET [ReportSortOrder] = 0

DECLARE @id uniqueidentifier;
DECLARE myCursor CURSOR
    FOR SELECT DISTINCT(ScorecardTemplateId) FROM [dbo].[ScorecardTemplatePeriod];		
OPEN myCursor;
FETCH NEXT FROM myCursor INTO @id;
MERGE INTO [dbo].[ScorecardTemplatePeriod] stp
		 USING (SELECT [Id], [StartDate], Row_Number()Over(ORDER BY StartDate) Rn FROM [dbo].[ScorecardTemplatePeriod] WHERE [ScorecardTemplateId]= @id) tmpstp
		 ON stp.Id = tmpstp.Id
		  WHEN MATCHED THEN		  
			UPDATE SET stp.[ReportSortOrder] = Rn;

WHILE @@FETCH_STATUS = 0  
    BEGIN
        FETCH NEXT FROM myCursor INTO @id;

		MERGE INTO [dbo].[ScorecardTemplatePeriod] stp
		 USING (SELECT [Id], [StartDate], Row_Number()Over(ORDER BY StartDate) Rn FROM [dbo].[ScorecardTemplatePeriod] WHERE [ScorecardTemplateId]= @id) tmpstp
		 ON stp.Id = tmpstp.Id
		  WHEN MATCHED THEN		  
			UPDATE SET stp.[ReportSortOrder] = Rn;		
    END;

CLOSE myCursor;
DEALLOCATE myCursor;