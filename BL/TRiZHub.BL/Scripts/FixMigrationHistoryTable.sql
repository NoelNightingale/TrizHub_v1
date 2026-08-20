-- This script helps fix the Entity Framework migration history table issue
-- EF6 uses __MigrationsHistory, while EF Core uses __EFMigrationsHistory

-- Step 1: Check which tables exist
SELECT 
    CASE 
        WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = '__MigrationsHistory') THEN 'Yes'
        ELSE 'No'
    END AS [__MigrationsHistory Exists],
    CASE 
        WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = '__EFMigrationsHistory') THEN 'Yes'
        ELSE 'No'
    END AS [__EFMigrationsHistory Exists]

-- Step 2: If __EFMigrationsHistory exists and __MigrationsHistory doesn't, you can:
-- Option A: Drop the EF Core table (if it was created by accident and has no valid data)
-- DROP TABLE [__EFMigrationsHistory]

-- Option B: Check what's in __EFMigrationsHistory
-- SELECT * FROM [__EFMigrationsHistory]

-- Option C: Check what's in __MigrationsHistory (your correct EF6 table)
-- SELECT * FROM [__MigrationsHistory]

-- Step 3: If you need to verify your EF6 migrations are all recorded:
-- SELECT * FROM [__MigrationsHistory] ORDER BY MigrationId

-- Note: The __EFMigrationsHistory table should NOT be used by this EF6 application.
-- If it exists, it was created by another application or tool that uses EF Core.
-- You can safely drop it if it doesn't contain any migrations your application needs.
