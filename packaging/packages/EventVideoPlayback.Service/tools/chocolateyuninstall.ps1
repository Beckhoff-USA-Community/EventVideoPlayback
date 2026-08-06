$ErrorActionPreference = 'Stop'
$serviceName = "EventVideoPlaybackService"
$installDir = "C:\Program Files\Beckhoff USA Community\EventVideoPlayback\Service"
$parentDir = "C:\Program Files\Beckhoff USA Community\EventVideoPlayback"

# Check if service exists
$existingService = Get-Service -Name $serviceName -ErrorAction SilentlyContinue

if ($existingService) {
    Write-Host "Stopping service '$serviceName'..."
    Stop-Service -Name $serviceName -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2

    Write-Host "Removing service '$serviceName'..."
    # Use sc.exe to delete the service
    & sc.exe delete $serviceName
    Start-Sleep -Seconds 2

    Write-Host "Service '$serviceName' removed successfully."
} else {
    Write-Host "Service '$serviceName' not found."
}

# Remove Service directory
if (Test-Path $installDir) {
    Write-Host "Removing service files from $installDir..."
    Remove-Item -Path $installDir -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "Service files removed."
}

# Check if EventVideoPlayback parent directory is now empty or only contained Service
if (Test-Path $parentDir) {
    $remainingItems = Get-ChildItem -Path $parentDir -Force -ErrorAction SilentlyContinue
    if ($remainingItems.Count -eq 0) {
        Write-Host "EventVideoPlayback directory is empty. Removing parent directory..."
        Remove-Item -Path $parentDir -Force -ErrorAction SilentlyContinue
        Write-Host "Parent directory removed."
    } else {
        Write-Host "Other EventVideoPlayback components remain installed. Preserving parent directory."
    }
}

Write-Host "EventVideoPlayback Service uninstalled successfully."
