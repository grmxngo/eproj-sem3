# package.ps1 — builds the single submission ZIP (source + docs + reference; latest backup included if present).
$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$stamp = Get-Date -Format "yyyyMMdd-HHmmss"
$out = Join-Path $root ("artifacts/HeritageTrail-submission-" + $stamp + ".zip")

$items = @(
    (Join-Path $root "app/src"),
    (Join-Path $root "app/HeritageTrail.slnx"),
    (Join-Path $root "app/global.json"),
    (Join-Path $root "docs"),
    (Join-Path $root "reference")
)

$latestBackup = Get-ChildItem (Join-Path $root "artifacts/backup") -Directory -ErrorAction SilentlyContinue |
    Sort-Object Name -Descending | Select-Object -First 1
if ($latestBackup) { $items += $latestBackup.FullName }

Compress-Archive -Path $items -DestinationPath $out -CompressionLevel Optimal
"Submission ZIP: $out ($((Get-Item $out).Length) bytes)"
