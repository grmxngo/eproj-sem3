# backup.ps1 — dumps one environment's Mongo database into artifacts/backup/{env}-{timestamp}
param(
    [ValidateSet("Staging", "Production")]
    [string]$EnvironmentName = "Staging"
)

$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$matrix = Get-Content (Join-Path $root "app/src/HeritageTrail.Web/Content/config/environments.json") -Raw | ConvertFrom-Json
$cfg = $matrix.environments.$EnvironmentName

$mongodump = Get-ChildItem "C:\Program Files\MongoDB\Server\*\bin\mongodump.exe" -ErrorAction SilentlyContinue |
    Sort-Object FullName -Descending | Select-Object -First 1
if (-not $mongodump) { throw "mongodump.exe not found. Install MongoDB tools or set MONGODUMP." }

$out = Join-Path $root ("artifacts/backup/" + $EnvironmentName.ToLower() + "-" + (Get-Date -Format "yyyyMMdd-HHmmss"))
New-Item -ItemType Directory -Force -Path $out | Out-Null
& $mongodump.FullName --port $cfg.mongoPort --db $cfg.database --out $out 2>&1 | Select-Object -Last 3
"Backup written to: $out"
