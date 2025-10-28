$targetFile = "C:\ProgramData\Beckhoff\NuGetPackages\EventVision.1.1.3.nupkg"

# Remove the NuGet package from the Beckhoff NuGet packages directory
if (Test-Path $targetFile) {
    Remove-Item -Path $targetFile -Force
    Write-Host "Removed EventVision.1.1.3.nupkg from Beckhoff NuGet packages directory"
} else {
    Write-Host "EventVision.1.1.3.nupkg not found in Beckhoff NuGet packages directory"
}
