-- Add Client/Project scope columns to BillingRates
-- (run if EF Add-Migration / Update-Database is not used yet).
-- Safe to re-run: checks for column existence.

IF COL_LENGTH(N'dbo.BillingRates', N'ClientId') IS NULL
BEGIN
    ALTER TABLE dbo.BillingRates ADD ClientId uniqueidentifier NULL;
END
GO

IF COL_LENGTH(N'dbo.BillingRates', N'ProjectId') IS NULL
BEGIN
    ALTER TABLE dbo.BillingRates ADD ProjectId uniqueidentifier NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IDX_BillingRatesClient' AND object_id = OBJECT_ID(N'dbo.BillingRates'))
BEGIN
    CREATE INDEX IDX_BillingRatesClient ON dbo.BillingRates(ClientId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IDX_BillingRatesProject' AND object_id = OBJECT_ID(N'dbo.BillingRates'))
BEGIN
    CREATE INDEX IDX_BillingRatesProject ON dbo.BillingRates(ProjectId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BillingRates_ClientEntity')
BEGIN
    ALTER TABLE dbo.BillingRates
        ADD CONSTRAINT FK_BillingRates_ClientEntity FOREIGN KEY (ClientId) REFERENCES dbo.ClientEntity(Id);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BillingRates_Project')
BEGIN
    ALTER TABLE dbo.BillingRates
        ADD CONSTRAINT FK_BillingRates_Project FOREIGN KEY (ProjectId) REFERENCES dbo.Project(Id);
END
GO
