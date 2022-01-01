[CmdletBinding(PositionalBinding=$false)]
param(
    [bool] $CreatePackages,
    [bool] $RunTests = $true,
    [string] $PullRequestNumber
)

if ($isLinux) {
    Write-Host "> Running on Linux..."  -ForegroundColor Black -BackgroundColor White
} else {
    Write-Host "> Running on Windows..."  -ForegroundColor Black -BackgroundColor White
}

Write-Host "Run Parameters:" -ForegroundColor Cyan
Write-Host "  CreatePackages: $CreatePackages"
Write-Host "  RunTests: $RunTests"
Write-Host "  dotnet --version:" (dotnet --version)

$packageOutputFolder = "$PSScriptRoot\.nupkgs"

if ($PullRequestNumber) {
    Write-Host "Building for a pull request (#$PullRequestNumber), skipping packaging." -ForegroundColor Yellow
    $CreatePackages = $false
}

Write-Host "Building all projects (Vaetech.PowerShell.Tests\Vaetech.PowerShell.Tests.csproj traversal)..." -ForegroundColor Yellow
dotnet build ".\Vaetech.PowerShell.Tests\Vaetech.PowerShell.Tests.csproj" -c Release /p:CI=true
Write-Host "Done building." -ForegroundColor "Green"

if ($RunTests) 
{
    write-host "Running tests :" -foregroundcolor black -backgroundcolor Yellow -nonewline
    write-host " Vaetech.PowerShell.Tests\Vaetech.PowerShell.Tests.csproj traversal (all frameworks)" -ForegroundColor White -BackgroundColor Black
    
    dotnet test ".\Vaetech.PowerShell.Tests\Vaetech.PowerShell.Tests.csproj" -c Release --no-build
    if ($LastExitCode -ne 0) {
        Write-Host "Error with tests, aborting build." -Foreground "Red"
        Exit 1
    }
    Write-Host "Tests passed!" -ForegroundColor "Green"
}

if ($CreatePackages) {
    New-Item -ItemType Directory -Path $packageOutputFolder -Force | Out-Null
    Write-Host "Clearing existing $packageOutputFolder..." -NoNewline
    Get-ChildItem $packageOutputFolder | Remove-Item
    Write-Host "done." -ForegroundColor "Green"

    Write-Host "Building all packages" -ForegroundColor "Green"
    dotnet pack ".\Vaetech.PowerShell.Tests\Vaetech.PowerShell.Tests.csproj" --no-build -c Release /p:PackageOutputPath=$packageOutputFolder /p:CI=true
}
Write-Host "Build Complete." -ForegroundColor "Green"

if ($isLinux) {
    Write-Host "Process finished, in Linux"  -ForegroundColor Black -BackgroundColor White
} else {
    Write-Host "Process finished, in Windows"  -ForegroundColor Black -BackgroundColor White
}