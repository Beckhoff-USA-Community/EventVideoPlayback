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
Write-Host "  - Introduction.md - Overview of EventVideoPlayback system"
Write-Host "  - Installation.md - Installation instructions"
Write-Host "  - SystemRequirements.md - System requirements and prerequisites"
Write-Host "  - FirstProgram.md - Getting started with your first program"
Write-Host "  - AddToExist.md - Adding EventVideoPlayback to existing projects"
Write-Host "  - ServiceDoc.md - EventVideoPlayback service documentation"
Write-Host "  - Licenses.md - License information"
