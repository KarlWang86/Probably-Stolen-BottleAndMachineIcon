# Explicit preparation only; no Steam login, game deployment or upload.
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'common.ps1')
$workspace = Join-Path $repoRoot 'WorkshopUpload-BottleAndMachineIcon'
$content = Join-Path $workspace 'content'
$version = Get-TaskVersion
$receipt = Get-Content (Join-Path $buildFolder 'validation.json') -Raw -Encoding UTF8 | ConvertFrom-Json
if ($receipt.Version -ne $version) { throw 'Scan version mismatch.' }
foreach ($file in @('BottleAndMachineIcon.dll','manifest.xml','使用说明.txt')) {
    if ((Get-FileHash (Join-Path $buildFolder $file) -Algorithm SHA256).Hash -ne $receipt.Files.$file) { throw "Scan hash mismatch: $file" }
}
New-Item -ItemType Directory -Path $content -Force | Out-Null
$configPath = Join-Path $workspace 'workshop.json'
$itemId = [UInt64]0
if (Test-Path $configPath) { $itemId = (Get-Content $configPath -Raw -Encoding UTF8 | ConvertFrom-Json).ItemId }
$idPath = Join-Path $docsRoot 'WorkshopItemId.txt'
if (Test-Path $idPath) {
    $savedId = [UInt64]([IO.File]::ReadAllText($idPath).Trim())
    if ($itemId -ne 0 -and $itemId -ne $savedId) { throw 'Saved workshop IDs differ.' }
    $itemId = $savedId
}
if ($itemId -eq 3836818025) { throw 'Refusing the chemical assistant workshop item.' }
if (Get-ChildItem $content -Force | Where-Object { $_.Name -notin @('BottleAndMachineIcon.dll','manifest.xml','使用说明.txt') }) { throw 'Unexpected upload content.' }
if (Test-Path $configPath) {
    $backup = Join-Path $workspace ('archive/' + (Get-Date -Format 'yyyyMMdd-HHmmss-fff'))
    New-Item -ItemType Directory -Path $backup -Force | Out-Null
    Copy-Item (Join-Path $workspace 'content') $backup -Recurse
    foreach ($name in @('workshop.json','description.txt','changenote.txt','preview.png')) {
        if (Test-Path (Join-Path $workspace $name)) { Copy-Item (Join-Path $workspace $name) $backup }
    }
}
foreach ($file in @('BottleAndMachineIcon.dll','manifest.xml','使用说明.txt')) { Copy-Item (Join-Path $buildFolder $file) $content }
Copy-Item (Join-Path $docsRoot 'WorkshopDescription.txt') (Join-Path $workspace 'description.txt')
Copy-Item (Join-Path $docsRoot 'WorkshopChangelog.txt') (Join-Path $workspace 'changenote.txt')
Copy-Item (Join-Path $docsRoot 'workshop-preview.png') (Join-Path $workspace 'preview.png')
[ordered]@{ AppId=4348910; ModId='bottle_and_machine_icon'; ItemId=$itemId; Version=$version; Title='容器与机器图标 / Bottle and Machine Icons'; Files=$receipt.Files } | ConvertTo-Json -Depth 4 | Set-Content $configPath -Encoding UTF8
Write-Output "Prepared independent workshop workspace: $workspace"
