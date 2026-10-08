# Fix Entity Framework Package Manager Console Issue
# This script will help restore the EntityFramework package properly

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Fixing EF6 Package Manager Console" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Step 1: Checking if Visual Studio is running..." -ForegroundColor Yellow
$vsProcesses = Get-Process devenv -ErrorAction SilentlyContinue
if ($vsProcesses) {
    Write-Host "? WARNING: Visual Studio is currently running!" -ForegroundColor Red
    Write-Host "Please close Visual Studio before continuing." -ForegroundColor Yellow
    Write-Host ""
    $response = Read-Host "Have you closed Visual Studio? (y/n)"
    if ($response -ne 'y') {
        Write-Host "Exiting. Please close Visual Studio and run this script again." -ForegroundColor Yellow
        exit 0
    }
}
else {
    Write-Host "? Visual Studio is not running." -ForegroundColor Green
}
Write-Host ""

Write-Host "Step 2: Backing up and removing EntityFramework package..." -ForegroundColor Yellow
$efPackagePath = ".\packages\EntityFramework.6.4.4"
if (Test-Path $efPackagePath) {
    $backupPath = ".\packages\EntityFramework.6.4.4.backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
    Write-Host "  Creating backup at: $backupPath" -ForegroundColor Gray
    Rename-Item $efPackagePath $backupPath
    Write-Host "? Package backed up and removed." -ForegroundColor Green
}
else {
    Write-Host "? Package not found at expected location." -ForegroundColor Yellow
}
Write-Host ""

Write-Host "Step 3: Clearing NuGet cache..." -ForegroundColor Yellow
$nugetCache = "$env:LOCALAPPDATA\NuGet\v3-cache"
if (Test-Path $nugetCache) {
    Get-ChildItem $nugetCache -Filter "*EntityFramework*" -Recurse -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "? NuGet cache cleared." -ForegroundColor Green
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "1. Open Visual Studio" -ForegroundColor White
Write-Host "2. Right-click on the SOLUTION in Solution Explorer" -ForegroundColor White
Write-Host "3. Select 'Restore NuGet Packages'" -ForegroundColor White
Write-Host "4. Wait for the restore to complete" -ForegroundColor White
Write-Host "5. Build the solution (Ctrl+Shift+B)" -ForegroundColor White
Write-Host "6. Open Package Manager Console:" -ForegroundColor White
Write-Host "   Tools > NuGet Package Manager > Package Manager Console" -ForegroundColor White
Write-Host "7. Set Default project to: TRiZHub.BL" -ForegroundColor White
Write-Host "8. Test with: Get-Migrations" -ForegroundColor White
Write-Host ""
Write-Host "If Package Manager Console still doesn't work:" -ForegroundColor Yellow
Write-Host "You may need to reinstall the EntityFramework package:" -ForegroundColor Yellow
Write-Host "  Uninstall-Package EntityFramework -Project TRiZHub.BL -Force" -ForegroundColor Gray
Write-Host "  Install-Package EntityFramework -Version 6.4.4 -Project TRiZHub.BL" -ForegroundColor Gray
Write-Host ""
