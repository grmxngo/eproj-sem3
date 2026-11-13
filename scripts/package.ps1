# package.ps1 — builds the single submission ZIP (source without bin/obj + docs + reference; latest backup included if present).
$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$stamp = Get-Date -Format "yyyyMMdd-HHmmss"
$out = Join-Path $root ("artifacts/HeritageTrail-submission-" + $stamp + ".zip")

$srcFiles = Get-ChildItem (Join-Path $root "app/src") -Recurse -File |
    Where-Object { $_.FullName -notmatch '\\bin\\|\\obj\\' }

$items = @(
    $srcFiles.FullName
    (Join-Path $root "app/HeritageTrail.slnx"),
    (Join-Path $root "app/global.json"),
    (Join-Path $root "docs"),
    (Join-Path $root "reference")
)

$latestBackup = Get-ChildItem (Join-Path $root "artifacts/backup") -Directory -ErrorAction SilentlyContinue |
    Sort-Object Name -Descending | Select-Object -First 1
if ($latestBackup) { $items += $latestBackup.FullName }

Compress-Archive -Path $items -DestinationPath $out -CompressionLevel Optimal
"Submission ZIP: $out ($([math]::Round((Get-Item $out).Length / 1MB, 1)) MB)"
