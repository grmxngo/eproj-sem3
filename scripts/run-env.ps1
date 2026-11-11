# run-env.ps1 — starts one environment (mongod + ASP.NET app) from the SSOT matrix.
# Run: powershell -ExecutionPolicy Bypass -File scripts/run-env.ps1 -EnvironmentName Staging
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("Staging", "Production")]
    [string]$EnvironmentName
)

$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$matrixPath = Join-Path $root "app/src/HeritageTrail.Web/Content/config/environments.json"
if (-not (Test-Path $matrixPath)) { throw "Missing $matrixPath" }
$matrix = Get-Content $matrixPath -Raw | ConvertFrom-Json
$cfg = $matrix.environments.$EnvironmentName
if (-not $cfg) { throw "No '$EnvironmentName' entry in $matrixPath" }

# --- .NET 10 ---
$dotnet = $env:DOTNET10
if (-not $dotnet) { $dotnet = Join-Path $env:USERPROFILE ".dotnet10\dotnet.exe" }
if (-not (Test-Path $dotnet)) { throw "dotnet 10 not found. Run scripts/setup-dotnet10.ps1 first." }
$ver = (& $dotnet --version)
if (-not $ver.StartsWith("10.")) { throw "Found dotnet $ver; this project requires .NET 10 (set DOTNET10)." }

# --- mongod ---
$mongod = $env:MONGOD
if (-not $mongod) {
    $found = Get-ChildItem "C:\Program Files\MongoDB\Server\*\bin\mongod.exe" -ErrorAction SilentlyContinue |
        Sort-Object FullName -Descending | Select-Object -First 1
    if ($found) { $mongod = $found.FullName }
}
if (-not $mongod -or -not (Test-Path $mongod)) { throw "mongod.exe not found. Install MongoDB 6.0+ or set MONGOD." }

$dbPath = Join-Path $root $cfg.dbPath
$logs = Join-Path $root "artifacts/logs"
New-Item -ItemType Directory -Force -Path $dbPath, $logs | Out-Null
$slug = $EnvironmentName.ToLowerInvariant()

$listening = Get-NetTCPConnection -State Listen -LocalPort $cfg.mongoPort -ErrorAction SilentlyContinue
if (-not $listening) {
    $mongoProc = Start-Process -FilePath $mongod `
        -ArgumentList @("--dbpath", $dbPath, "--port", $cfg.mongoPort, "--bind_ip", "127.0.0.1", "--quiet") `
        -PassThru -WindowStyle Hidden `
        -RedirectStandardOutput (Join-Path $logs "$slug-mongod.out.log") `
        -RedirectStandardError (Join-Path $logs "$slug-mongod.err.log")
    $ok = $false
    for ($i = 0; $i -lt 40; $i++) {
        Start-Sleep -Milliseconds 500
        if (Get-NetTCPConnection -State Listen -LocalPort $cfg.mongoPort -ErrorAction SilentlyContinue) { $ok = $true; break }
    }
    if (-not $ok) { throw "mongod did not start on port $($cfg.mongoPort). See artifacts/logs/$slug-mongod*.log" }
    "mongod: running for $EnvironmentName (pid $($mongoProc.Id), port $($cfg.mongoPort))"
} else {
    "mongod: already listening on port $($cfg.mongoPort)"
}

# --- app ---
$env:ASPNETCORE_ENVIRONMENT = $EnvironmentName
$project = Join-Path $root "app/src/HeritageTrail.Web/HeritageTrail.Web.csproj"
$urls = "http://127.0.0.1:$($cfg.appPort)"
$logFile = Join-Path $logs "$slug-app.log"
"app: $EnvironmentName -> $urls (log: $logFile)"
Push-Location (Join-Path $root "app")
try {
    & $dotnet run --project $project --urls $urls *> $logFile
} finally {
    Pop-Location
}
