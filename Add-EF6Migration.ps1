# Script to Add a New Migration using EF6.exe
# This is an alternative to Package Manager Console Add-Migration command

param(
    [Parameter(Mandatory=$true)]
    [string]$MigrationName
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Creating EF6 Migration: $MigrationName" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Paths
$ef6Path = ".\packages\EntityFramework.6.4.4\tools\net45\any\ef6.exe"
$projectPath = ".\BL\TRiZHub.BL"
$assemblyPath = "$projectPath\bin\Debug\TRiZHub.BL.dll"
$configPath = "$projectPath\App.config"
$migrationsPath = "$projectPath\Migrations"

# Validate paths
if (-not (Test-Path $ef6Path)) {
    Write-Host "ERROR: ef6.exe not found at $ef6Path" -ForegroundColor Red
    Write-Host "Please restore NuGet packages first." -ForegroundColor Yellow
    exit 1
}

if (-not (Test-Path $assemblyPath)) {
    Write-Host "ERROR: Assembly not found at $assemblyPath" -ForegroundColor Red
    Write-Host "Please build the TRiZHub.BL project first." -ForegroundColor Yellow
    exit 1
}

# Since ef6.exe doesn't have an "add-migration" equivalent, we need to use the .NET Core approach
# or manually guide the user to use Visual Studio

Write-Host "? LIMITATION: ef6.exe doesn't support creating new migrations directly." -ForegroundColor Yellow
Write-Host ""
Write-Host "To create a new migration, you have two options:" -ForegroundColor White
Write-Host ""
Write-Host "OPTION 1: Fix Package Manager Console (Recommended)" -ForegroundColor Green
Write-Host "  1. Close Visual Studio" -ForegroundColor Gray
Write-Host "  2. Delete folder: packages\EntityFramework.6.4.4" -ForegroundColor Gray
Write-Host "  3. Reopen Visual Studio" -ForegroundColor Gray
Write-Host "  4. Build the solution" -ForegroundColor Gray
Write-Host "  5. Open Package Manager Console (Tools > NuGet Package Manager > Package Manager Console)" -ForegroundColor Gray
Write-Host "  6. Set default project to: TRiZHub.BL" -ForegroundColor Gray
Write-Host "  7. Run: Add-Migration $MigrationName" -ForegroundColor Gray
Write-Host ""
Write-Host "OPTION 2: Use EntityFramework.Commands (Install EF Core Tools)" -ForegroundColor Green
Write-Host "  This would install dotnet ef tools, but it's for EF Core, not EF6." -ForegroundColor Gray
Write-Host ""
Write-Host "OPTION 3: Check what changes are pending:" -ForegroundColor Green
Write-Host "  Run: .\CheckPendingChanges.ps1" -ForegroundColor Gray
Write-Host ""

# Try to detect what changed
Write-Host "Attempting to identify pending changes..." -ForegroundColor Yellow
Write-Host ""
Write-Host "Latest migration file:" -ForegroundColor White
$latestMigration = Get-ChildItem "$migrationsPath\*.cs" -Exclude "*.Designer.cs","Configuration.cs" | Sort-Object Name -Descending | Select-Object -First 1
Write-Host "  $($latestMigration.Name)" -ForegroundColor Cyan
Write-Host ""
Write-Host "Recent files modified in project:" -ForegroundColor White
Get-ChildItem "$projectPath\Entities" -Recurse -File | Where-Object { $_.LastWriteTime -gt (Get-Date).AddDays(-7) } | Select-Object -First 5 Name, LastWriteTime | Format-Table
