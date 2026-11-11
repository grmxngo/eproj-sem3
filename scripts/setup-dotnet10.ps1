# setup-dotnet10.ps1 — installs the .NET 10 SDK per-user (no admin required).
# Run: powershell -ExecutionPolicy Bypass -File scripts/setup-dotnet10.ps1
param(
    [string]$Version = "10.0.404",
    [string]$InstallDir = "$env:USERPROFILE\.dotnet10"
)

$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$dotnetExe = Join-Path $InstallDir "dotnet.exe"

if (Test-Path $dotnetExe) {
    $installed = & $dotnetExe --version
    if ($installed -like "10.*") {
        "dotnet $installed already present at $InstallDir"
        exit 0
    }
    "Found dotnet $installed at $InstallDir but .NET 10 is required; reinstalling..."
}

$script = Join-Path $PSScriptRoot "dotnet-install.ps1"
if (-not (Test-Path $script)) {
    Invoke-WebRequest "https://dot.net/v1/dotnet-install.ps1" -OutFile $script -UseBasicParsing
}

powershell.exe -NoProfile -ExecutionPolicy Bypass -File $script -Channel 10.0 -Version $Version -InstallDir $InstallDir -NoPath
if ($LASTEXITCODE -ne 0) { throw "dotnet-install failed with exit code $LASTEXITCODE" }

$installed = & $dotnetExe --version
if (-not $installed.StartsWith("10.")) { throw "Expected .NET 10, got $installed" }
"Installed dotnet $installed at $InstallDir"
