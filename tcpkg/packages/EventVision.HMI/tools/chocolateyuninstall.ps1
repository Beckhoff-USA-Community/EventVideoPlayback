$toolsDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$targetDir = "C:\ProgramData\Beckhoff\NuGetPackages"

# The staged nupkg next to this script tells us which file this package
# installed, so the version number stays out of this script.
$staged = Get-ChildItem -Path $toolsDir -Filter "EventVision.*.nupkg"
foreach ($package in $staged) {
    $targetFile = Join-Path $targetDir $package.Name
    if (Test-Path $targetFile) {
        Remove-Item -Path $targetFile -Force
        Write-Host "Removed $($package.Name) from $targetDir"
    } else {
        Write-Host "$($package.Name) not found in $targetDir"
    }
}
