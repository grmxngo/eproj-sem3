# staging.ps1 — staging environment (port 5181, Mongo 27018, heritagetrail_staging).
# Run: powershell -ExecutionPolicy Bypass -File scripts/staging.ps1
$ErrorActionPreference = "Stop"
& (Join-Path $PSScriptRoot "run-env.ps1") -EnvironmentName Staging
