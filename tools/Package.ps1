param([Parameter(Mandatory=$true)][string]$AAInstallPath)
$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent $PSScriptRoot
& dotnet build (Join-Path $repo 'src\AASkipIntro\AASkipIntro.csproj') -c Release ('-p:AAInstallPath=' + (Resolve-Path -LiteralPath $AAInstallPath).Path) --nologo -v:q
if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
$release = Join-Path $repo ('artifacts\AASkipIntro-0.1.0-' + (Get-Date -Format 'yyyyMMdd-HHmmss'))
if (Test-Path -LiteralPath $release) { throw 'Release path already exists' }
$payload = Join-Path $release 'mods\AASkipIntro\0.1.0'
New-Item -ItemType Directory -Path $payload | Out-Null
Copy-Item -LiteralPath (Join-Path $repo 'src\AASkipIntro\bin\Release\net6.0\AASkipIntro.dll') -Destination $payload
Copy-Item -LiteralPath (Join-Path $repo 'manifest.json') -Destination $payload
foreach($name in @('README.md','THIRD_PARTY_NOTICES.md')) { Copy-Item -LiteralPath (Join-Path $repo $name) -Destination $release }
Copy-Item -LiteralPath (Join-Path $repo 'docs') -Destination $release -Recurse
Copy-Item -LiteralPath (Join-Path $repo 'tools\Install-Mod.ps1') -Destination $release
# Original skip symbol drawn locally; no AA art.
Add-Type -AssemblyName System.Drawing.Common
$bitmap = [Drawing.Bitmap]::new(128,128)
$graphics = [Drawing.Graphics]::FromImage($bitmap)
$brush = [Drawing.SolidBrush]::new([Drawing.Color]::FromArgb(248,229,78))
try {
    $graphics.Clear([Drawing.Color]::FromArgb(40,68,100))
    $graphics.FillPolygon($brush,[Drawing.PointF[]]@([Drawing.PointF]::new(29,26),[Drawing.PointF]::new(84,64),[Drawing.PointF]::new(29,102)))
    $graphics.FillRectangle($brush,89,26,11,76)
    $bitmap.Save((Join-Path $payload 'icon.png'),[Drawing.Imaging.ImageFormat]::Png)
} finally { $brush.Dispose(); $graphics.Dispose(); $bitmap.Dispose() }
$rows = Get-ChildItem -LiteralPath $release -Recurse -File | Sort-Object FullName | ForEach-Object {
    (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant() + '  ' + [IO.Path]::GetRelativePath($release,$_.FullName).Replace('\','/')
}
[IO.File]::WriteAllLines((Join-Path $release 'SHA256SUMS.txt'),$rows,[Text.UTF8Encoding]::new($false))
Compress-Archive -LiteralPath $release -DestinationPath ($release + '.zip')
Get-FileHash -LiteralPath ($release + '.zip') -Algorithm SHA256 | Format-List
