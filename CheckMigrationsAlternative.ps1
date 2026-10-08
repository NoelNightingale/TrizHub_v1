# Entity Framework 6 Migration Commands (Alternative Method)
# Since Package Manager Console is having issues, use these commands in PowerShell

# Navigate to your solution directory first
# cd C:\sandboxes\Triz\repo\TAP1\TrizHub_v1

# Method 1: Use ef6.exe directly (from the NuGet package)
Write-Host "Using EF6.exe to check migrations..." -ForegroundColor Green

# Set the path to ef6.exe
$ef6Path = ".\packages\EntityFramework.6.4.4\tools\net45\any\ef6.exe"

# Check if ef6.exe exists
if (Test-Path $ef6Path) {
    Write-Host "ef6.exe found!" -ForegroundColor Green

    # Get the assembly path
    $assemblyPath = ".\BL\TRiZHub.BL\bin\Debug\TRiZHub.BL.dll"

    if (Test-Path $assemblyPath) {
        Write-Host "Assembly found at: $assemblyPath" -ForegroundColor Green

        # List all migrations
        Write-Host "`n========================================" -ForegroundColor Cyan
        Write-Host "Available Migrations:" -ForegroundColor Cyan
        Write-Host "========================================" -ForegroundColor Cyan
        & $ef6Path migrate --help

        Write-Host "`nTo update database, you can use:" -ForegroundColor Yellow
        Write-Host "  & '$ef6Path' database update --assembly '$assemblyPath' --connection-string-name DefaultConnection" -ForegroundColor Yellow
    }
    else {
        Write-Host "ERROR: Assembly not found. Please build the solution first." -ForegroundColor Red
        Write-Host "Expected path: $assemblyPath" -ForegroundColor Red
    }
}
else {
    Write-Host "ERROR: ef6.exe not found at $ef6Path" -ForegroundColor Red
    Write-Host "Try restoring NuGet packages first." -ForegroundColor Yellow
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Alternative: Fix Package Manager Console" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "1. Close Visual Studio"
Write-Host "2. Delete the packages\EntityFramework.6.4.4 folder"
Write-Host "3. Reopen Visual Studio"
Write-Host "4. Right-click the solution and select 'Restore NuGet Packages'"
Write-Host "5. Try Package Manager Console again"
