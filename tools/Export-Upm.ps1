$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$out = Join-Path $root 'Artifacts/UPM'
New-Item -ItemType Directory -Force $out | Out-Null
foreach ($package in Get-ChildItem (Join-Path $root 'Packages') -Directory -Filter 'com.vectracode.*') {
    $manifest = Get-Content (Join-Path $package.FullName 'package.json') -Raw | ConvertFrom-Json
    if ($manifest.name -ne $package.Name) { throw "Package name mismatch: $package" }
    $stage = Join-Path $out ($manifest.name + '-' + [Guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory -Force $stage | Out-Null
    $copy = Join-Path $stage 'package'
    if (Test-Path $copy) { throw "Staging exists: $copy. Use a fresh Artifacts/UPM directory." }
    Copy-Item -LiteralPath $package.FullName -Destination $copy -Recurse
    $archive = Join-Path $out ($manifest.name + '-' + $manifest.version + '.tgz')
    & tar -czf $archive -C $stage package
    if ($LASTEXITCODE -ne 0) { throw "tar failed: $archive" }
    & tar -tzf $archive | Select-Object -First 3
    Get-FileHash $archive -Algorithm SHA256
}
