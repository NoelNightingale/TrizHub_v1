USE [TRiZHub]
GO

--Sometimes the tables are not created, please add the column if it does not exist in the DB
ALTER TABLE [dbo].[ScorecardTemplatePeriod] ADD [ReviewYear] [int] NOT NULL DEFAULT 0
GO

UPDATE [TRiZHub].[dbo].[ScorecardTemplatePeriod] SET [ReviewYear] = year(StartDate) 
GO
