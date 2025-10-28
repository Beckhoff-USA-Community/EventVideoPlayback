$ErrorActionPreference = 'Stop'
$installDir = "C:\Program Files\Beckhoff USA Community\EventVideoPlayback\Documentation"

# Clean up existing documentation before modification
if (Test-Path $installDir) {
    Write-Host "Cleaning up existing EventVideoPlayback documentation..."
    Remove-Item -Path "$installDir\*" -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "Cleanup complete."
}
