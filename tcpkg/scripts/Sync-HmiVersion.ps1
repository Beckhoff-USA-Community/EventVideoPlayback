<#
.SYNOPSIS
    Stamps the EventVision control version from Description.json into both nuspecs.

.DESCRIPTION
    The control version is defined once, in the HMI project's Description.json.
    This script reads it from there and rewrites the version element in the two
    files that must carry the same number:

    * src\HMI\TcHmiEventVideo\TcEventVideoPlayback-TcHMIControl\EventVision.nuspec
    * tcpkg\packages\EventVision.HMI\EventVision.HMI.nuspec

    Run it after changing the version in Description.json. The chocolatey
    install scripts glob the nupkg file name, so they never need a version edit.

.EXAMPLE
    .\Sync-HmiVersion.ps1
    Reads the version and restamps both nuspecs.

.NOTES
    Requires: run from anywhere inside the repository.
#>

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$descriptionPath = Join-Path $repoRoot 'src\HMI\TcHmiEventVideo\TcEventVideoPlayback-TcHMIControl\EventVisionControl\Description.json'
$projectNuspec = Join-Path $repoRoot 'src\HMI\TcHmiEventVideo\TcEventVideoPlayback-TcHMIControl\EventVision.nuspec'
$tcpkgNuspec = Join-Path $repoRoot 'tcpkg\packages\EventVision.HMI\EventVision.HMI.nuspec'

$description = Get-Content $descriptionPath -Raw | ConvertFrom-Json
$version = '{0}.{1}.{2}' -f $description.version.major, $description.version.minor, $description.version.revision

$expectedFull = "$version.$($description.version.build)"
if ($description.version.full -ne $expectedFull) {
    throw "Description.json version.full '$($description.version.full)' does not match its own major.minor.revision.build '$expectedFull'. Fix Description.json first."
}

foreach ($nuspecPath in @($projectNuspec, $tcpkgNuspec)) {
    $content = Get-Content $nuspecPath -Raw
    $updated = $content -replace '<version>[0-9]+\.[0-9]+\.[0-9]+</version>', "<version>$version</version>"
    if ($updated -ne $content) {
        Set-Content -Path $nuspecPath -Value $updated -NoNewline
        Write-Host "Stamped $version into $nuspecPath"
    } else {
        Write-Host "$nuspecPath already at $version"
    }
}
