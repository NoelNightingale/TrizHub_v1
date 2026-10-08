# =============================================================================
# SUMMARY: How to Create EF6 Migration for Timesheet Template Changes
# =============================================================================

# YOUR SITUATION:
# ----------------
# You have modified TimesheetTemplate.cs and TimesheetTemplateItem.cs entity classes.
# These changes need to be saved as a migration, but Package Manager Console isn't working.

# SOLUTION:
# ---------
# Run this PowerShell script: .\Fix-EF6PackageManagerConsole.ps1
# Then follow the instructions below.

# =============================================================================
# STEP-BY-STEP INSTRUCTIONS
# =============================================================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "How to Fix Your EF6 Migration Issue" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "STEP 1: Fix Package Manager Console" -ForegroundColor Green
Write-Host "---------------------------------------" -ForegroundColor Gray
Write-Host "Run this command in PowerShell:" -ForegroundColor White
Write-Host "  .\Fix-EF6PackageManagerConsole.ps1" -ForegroundColor Yellow
Write-Host ""

Write-Host "STEP 2: Create Migration" -ForegroundColor Green
Write-Host "---------------------------------------" -ForegroundColor Gray
Write-Host "After fixing PMC, open Visual Studio and:" -ForegroundColor White
Write-Host "1. Open Package Manager Console (Tools > NuGet Package Manager > Package Manager Console)" -ForegroundColor White
Write-Host "2. Set Default project dropdown to: TRiZHub.BL" -ForegroundColor White
Write-Host "3. Run:" -ForegroundColor White
Write-Host "   Add-Migration AddTimesheetTemplateFields" -ForegroundColor Yellow
Write-Host "   (or any descriptive name for your changes)" -ForegroundColor Gray
Write-Host ""

Write-Host "STEP 3: Apply Migration" -ForegroundColor Green
Write-Host "---------------------------------------" -ForegroundColor Gray
Write-Host "In Package Manager Console, run:" -ForegroundColor White
Write-Host "   Update-Database -Verbose" -ForegroundColor Yellow
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Alternative: Manual Migration Creation" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "If Package Manager Console still doesn't work, you can:" -ForegroundColor White
Write-Host "1. Review what changed in your entities:" -ForegroundColor White
Write-Host "   - TimesheetTemplate.cs (modified today)" -ForegroundColor Gray
Write-Host "   - TimesheetTemplateItem.cs (modified today)" -ForegroundColor Gray
Write-Host ""
Write-Host "2. Manually create SQL scripts to update the database tables" -ForegroundColor White
Write-Host "3. Then manually add an entry to __MigrationsHistory" -ForegroundColor White
Write-Host ""
Write-Host "However, using Add-Migration is STRONGLY RECOMMENDED as it:" -ForegroundColor Yellow
Write-Host "- Automatically generates correct SQL" -ForegroundColor Gray
Write-Host "- Handles foreign keys and indexes" -ForegroundColor Gray
Write-Host "- Creates a rollback (Down) method" -ForegroundColor Gray
Write-Host "- Keeps your codebase maintainable" -ForegroundColor Gray
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Current Status" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "? Database is currently at migration: 202503060742046_ProjectTimeCaptureAssignment" -ForegroundColor Green
Write-Host "? Pending changes detected in TimesheetTemplate entities" -ForegroundColor Red
Write-Host "? You MUST create a migration before running the application" -ForegroundColor Yellow
Write-Host ""
