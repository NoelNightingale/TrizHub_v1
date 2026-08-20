-- ============================================================================
-- Entity Framework 6 Migration Diagnostic Script
-- Run this in SQL Server Management Studio against your database
-- ============================================================================

PRINT '============================================================================'
PRINT 'EF6 Migration History Diagnostics'
PRINT '============================================================================'
PRINT ''

-- 1. Check if __MigrationsHistory table exists
PRINT '1. Checking if __MigrationsHistory table exists...'
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = '__MigrationsHistory')
BEGIN
    PRINT '   ? __MigrationsHistory table EXISTS'
    PRINT ''

    -- 2. Show all applied migrations
    PRINT '2. Applied migrations in database:'
    PRINT '   ------------------------------'
    SELECT 
        ROW_NUMBER() OVER (ORDER BY MigrationId) AS [#],
        MigrationId,
        ContextKey,
        Model,
        ProductVersion
    FROM [dbo].[__MigrationsHistory]
    ORDER BY MigrationId

    PRINT ''
    PRINT '3. Latest migration in database:'
    PRINT '   ------------------------------'
    SELECT TOP 1
        MigrationId as [Latest Migration],
        ProductVersion as [EF Version]
    FROM [dbo].[__MigrationsHistory]
    ORDER BY MigrationId DESC

    PRINT ''
    PRINT '4. Total migrations in database:'
    SELECT COUNT(*) as [Total Migrations Count]
    FROM [dbo].[__MigrationsHistory]
END
ELSE
BEGIN
    PRINT '   ? __MigrationsHistory table DOES NOT EXIST'
    PRINT '   This is a problem! EF6 needs this table to track migrations.'
    PRINT ''
    PRINT '   SOLUTION: Run "Update-Database -Verbose" in Package Manager Console'
    PRINT '   This will create the table and apply all pending migrations.'
END

PRINT ''
PRINT '============================================================================'
PRINT 'Expected Latest Migration (from your code):'
PRINT '   202503060742046_ProjectTimeCaptureAssignment'
PRINT '============================================================================'
PRINT ''

-- 5. Check for the EF Core table (should NOT exist for EF6 apps)
PRINT '5. Checking for EF Core migration table (should NOT exist)...'
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    PRINT '   ? WARNING: __EFMigrationsHistory table EXISTS'
    PRINT '   This is an EF Core table and should not be here.'
    PRINT '   You should drop it: DROP TABLE [__EFMigrationsHistory]'
END
ELSE
BEGIN
    PRINT '   ? __EFMigrationsHistory does NOT exist (Good!)'
END

PRINT ''
PRINT '============================================================================'
PRINT 'Next Steps:'
PRINT '============================================================================'
PRINT '1. If the latest migration shown above is NOT 202503060742046_ProjectTimeCaptureAssignment,'
PRINT '   then run in Package Manager Console:'
PRINT '   Update-Database -Verbose'
PRINT ''
PRINT '2. If you need to see what SQL will be executed, run:'
PRINT '   Update-Database -Verbose -Script'
PRINT ''
PRINT '3. To force recreate __MigrationsHistory (ONLY if it is missing/corrupt):'
PRINT '   Update-Database -TargetMigration:$InitialDatabase -Force'
PRINT '   Update-Database -Verbose'
PRINT '============================================================================'
