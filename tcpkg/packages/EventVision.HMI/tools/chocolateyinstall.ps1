$toolsDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$fileLocation = Join-Path $toolsDir "EventVision.1.1.3.nupkg"
$targetDir = "C:\ProgramData\Beckhoff\NuGetPackages"

# Ensure target directory exists
if (-not (Test-Path $targetDir)) {
    New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
    Write-Host "Created directory: $targetDir"
}

# Copy the NuGet package to the Beckhoff NuGet packages directory
Copy-Item -Path $fileLocation -Destination $targetDir -Force
Write-Host "Installed EventVision.1.1.3.nupkg to $targetDir"
