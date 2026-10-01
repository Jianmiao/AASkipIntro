param([Parameter(Mandatory=$true)][string]$AAInstallPath)
$ErrorActionPreference = 'Stop'
$aaRoot = (Resolve-Path -LiteralPath $AAInstallPath).Path
if (!(Test-Path -LiteralPath (Join-Path $aaRoot 'AzureArchive.exe')) -or
    !(Test-Path -LiteralPath (Join-Path $aaRoot 'BepInEx\patchers\ModTheAzureArchive.dll'))) {
    throw 'Expected AA with BepInEx 6 and ModTheAzureArchive'
}
if (Get-Process AzureArchive -ErrorAction SilentlyContinue) { throw 'Close AA before installation' }
$payload = Join-Path $PSScriptRoot '0.1.0'
if (!(Test-Path -LiteralPath $payload)) {
    $payload = Join-Path $PSScriptRoot 'mods\AASkipIntro\0.1.0'
}
$target = Join-Path $aaRoot 'mods\AASkipIntro\0.1.0'
if (Test-Path -LiteralPath $target) { throw 'This version already exists; nothing overwritten' }
$allowed = @('AASkipIntro.dll','manifest.json','icon.png')
foreach($name in $allowed) { if (!(Test-Path -LiteralPath (Join-Path $payload $name))) { throw "Missing payload: $name" } }
New-Item -ItemType Directory -Path $target | Out-Null
foreach($name in $allowed) {
    Copy-Item -LiteralPath (Join-Path $payload $name) -Destination $target
    if ((Get-FileHash -LiteralPath (Join-Path $payload $name)).Hash -ne (Get-FileHash -LiteralPath (Join-Path $target $name)).Hash) { throw 'Installed hash mismatch' }
}
Write-Output 'Installed 极速启动 (AASkipIntro). Enable it through AA mod management and restart. No other mod or setting was changed.'
