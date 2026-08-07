$ErrorActionPreference = 'Stop'
$toolsDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$docsSourceDir = Join-Path $toolsDir "docs"

# Target installation directory - Beckhoff USA Community location
$installDir = "C:\Program Files\Beckhoff USA Community\EventVideoPlayback\Documentation"

# Create installation directory if it doesn't exist
if (-not (Test-Path $installDir)) {
    New-Item -ItemType Directory -Path $installDir -Force | Out-Null
    Write-Host "Created directory: $installDir"
}

# Copy documentation files to installation directory
Write-Host "Installing EventVideoPlayback documentation to $installDir..."
Copy-Item -Path "$docsSourceDir\*" -Destination $installDir -Recurse -Force

Write-Host "EventVideoPlayback Documentation installed successfully."
Write-Host "Documentation Location: $installDir"
Write-Host ""
Write-Host "Available documentation files:"
Get-ChildItem -Path $installDir -Filter '*.md' | ForEach-Object {
    Write-Host "  $($_.Name)"
}
