$toolsDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$targetDir = "C:\ProgramData\Beckhoff\NuGetPackages"

# The HMI nupkg is staged next to this script at pack time. Glob the file name
# so the version number lives in the nupkg alone, not in this script.
$package = Get-ChildItem -Path $toolsDir -Filter "EventVision.*.nupkg" | Select-Object -First 1
if ($null -eq $package) {
    throw "No EventVision nupkg found in $toolsDir"
}

# Ensure target directory exists
if (-not (Test-Path $targetDir)) {
    New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
    Write-Host "Created directory: $targetDir"
}

# Copy the NuGet package to the Beckhoff NuGet packages directory
Copy-Item -Path $package.FullName -Destination $targetDir -Force
Write-Host "Installed $($package.Name) to $targetDir"
