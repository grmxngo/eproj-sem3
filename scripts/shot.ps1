# shot.ps1 — renders a URL to PNG with Brave headless (used to show atoms/pages in chat).
# Run: powershell -ExecutionPolicy Bypass -File scripts/shot.ps1 -Url http://127.0.0.1:5181/atoms -Out artifacts/shots/x.png
param(
    [Parameter(Mandatory = $true)][string]$Url,
    [Parameter(Mandatory = $true)][string]$Out,
    [int]$Width = 1440,
    [int]$Height = 900,
    [int]$Dpr = 1,
    [int]$Budget = 2500
)

$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$outPath = if ([System.IO.Path]::IsPathRooted($Out)) { $Out } else { Join-Path $root $Out }
$outDir = Split-Path $outPath -Parent
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

$brave = $env:BRAVE
if (-not $brave) {
    $candidates = @(
        (Join-Path $env:LOCALAPPDATA "BraveSoftware\Brave-Browser\Application\brave.exe"),
        "C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe",
        "C:\Program Files (x86)\BraveSoftware\Brave-Browser\Application\brave.exe"
    )
    $brave = $candidates | Where-Object { Test-Path $_ } | Select-Object -First 1
}
if (-not $brave) { throw "Brave not found. Set BRAVE to a Chromium-based browser path." }

if (Test-Path $outPath) { Remove-Item $outPath -Force }
$args = @(
    "--headless=new",
    "--disable-gpu",
    "--hide-scrollbars",
    "--force-device-scale-factor=$Dpr",
    "--window-size=$Width,$Height",
    "--virtual-time-budget=$Budget",
    "--screenshot=$outPath",
    $Url
)

for ($i = 1; $i -le 3; $i++) {
    & $brave @args 2>$null | Out-Null
    Start-Sleep -Milliseconds 800
    if (Test-Path $outPath) { break }
    "retry $i for $Url"
}

if (-not (Test-Path $outPath)) { throw "Screenshot failed after retries: $Url" }
"Screenshot OK: $outPath ($((Get-Item $outPath).Length) bytes, ${Width}x${Height})"
