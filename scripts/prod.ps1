# prod.ps1 — production environment (port 5182, Mongo 27019, heritagetrail_prod).
# Run: powershell -ExecutionPolicy Bypass -File scripts/prod.ps1
$ErrorActionPreference = "Stop"
& (Join-Path $PSScriptRoot "run-env.ps1") -EnvironmentName Production
