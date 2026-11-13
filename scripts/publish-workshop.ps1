# Explicit Workshop publication; no game startup or local mod deployment.
param([switch]$CheckOnly, [switch]$SchineseOnly)
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'common.ps1')
$workspace = Join-Path $repoRoot 'WorkshopUpload-BottleAndMachineIcon'
$config = Get-Content (Join-Path $workspace 'workshop.json') -Raw -Encoding UTF8 | ConvertFrom-Json
$receipt = Get-Content (Join-Path $buildFolder 'validation.json') -Raw -Encoding UTF8 | ConvertFrom-Json
if ($config.Version -ne (Get-TaskVersion) -or $receipt.Version -ne $config.Version) { throw 'Workshop version mismatch.' }
foreach ($file in @('BottleAndMachineIcon.dll','manifest.xml','使用说明.txt')) {
    $hash = (Get-FileHash (Join-Path $workspace "content/$file") -Algorithm SHA256).Hash
    if ($hash -ne $receipt.Files.$file -or $hash -ne (Get-FileHash (Join-Path $buildFolder $file) -Algorithm SHA256).Hash) { throw "Unvalidated workshop file: $file" }
}
$output = Join-Path $buildFolder 'tools/WorkshopPublisher'
dotnet build (Join-Path $toolsRoot 'WorkshopPublisher/WorkshopPublisher.csproj') -c Release -o $output --ignore-failed-sources -p:NuGetAudit=false "-p:RestoreConfigFile=$repoRoot/NuGet.Config"
if ($LASTEXITCODE -ne 0) { throw 'WorkshopPublisher build failed.' }
$mode = if ($CheckOnly) { '--check' } elseif ($SchineseOnly) { '--schinese-only' } else { '--publish' }
& dotnet (Join-Path $output 'WorkshopPublisher.dll') $workspace $mode
if ($LASTEXITCODE -ne 0) { throw 'Workshop publication incomplete; inspect the saved item ID and receipt before retrying.' }