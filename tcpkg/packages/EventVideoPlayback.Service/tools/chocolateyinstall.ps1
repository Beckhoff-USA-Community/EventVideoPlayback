$ErrorActionPreference = 'Stop'
$toolsDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$serviceDir = Join-Path $toolsDir "service"
$serviceName = "EventVideoPlaybackService"
$serviceDisplayName = "EventVideoPlayback Service"
$serviceDescription = "Background service for video playback event management"
$serviceExecutable = Join-Path $serviceDir "EventVideoPlaybackService.exe"

# Target installation directory
$installDir = "C:\Program Files\Beckhoff USA Community\EventVideoPlayback\Service"

# Create installation directory if it doesn't exist
if (-not (Test-Path $installDir)) {
    New-Item -ItemType Directory -Path $installDir -Force | Out-Null
    Write-Host "Created directory: $installDir"
}

# Copy service files to installation directory
Write-Host "Copying service files to $installDir..."
Copy-Item -Path "$serviceDir\*" -Destination $installDir -Recurse -Force

# Update the service executable path to installation directory
$serviceExecutable = Join-Path $installDir "EventVideoPlaybackService.exe"

# Check if service already exists
$existingService = Get-Service -Name $serviceName -ErrorAction SilentlyContinue

if ($existingService) {
    Write-Host "Service '$serviceName' already exists. Stopping and removing..."
    Stop-Service -Name $serviceName -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2

    # Use sc.exe to delete the service
    & sc.exe delete $serviceName
    Start-Sleep -Seconds 2
}

# Create and configure the Windows service
Write-Host "Creating Windows service '$serviceName'..."
New-Service -Name $serviceName `
    -BinaryPathName $serviceExecutable `
    -DisplayName $serviceDisplayName `
    -Description $serviceDescription `
    -StartupType Automatic

# Configure service for delayed auto-start
Write-Host "Configuring service for delayed auto-start..."
& sc.exe config $serviceName start=delayed-auto

# Set service recovery options (restart on failure)
& sc.exe failure $serviceName reset=86400 actions=restart/60000/restart/60000/restart/60000

# Start the service
Write-Host "Starting service '$serviceName'..."
Start-Service -Name $serviceName

Write-Host "EventVideoPlayback Service installed and started successfully."
Write-Host "Service Location: $installDir"
