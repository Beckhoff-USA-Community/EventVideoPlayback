$ErrorActionPreference = 'Stop'
$installDir = "C:\Program Files\Beckhoff USA Community\EventVideoPlayback\Documentation"
$parentDir = "C:\Program Files\Beckhoff USA Community\EventVideoPlayback"

# Remove the Documentation directory
if (Test-Path $installDir) {
    Write-Host "Removing EventVideoPlayback documentation from $installDir..."
    Remove-Item -Path $installDir -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "EventVideoPlayback Documentation uninstalled successfully."
} else {
    Write-Host "EventVideoPlayback Documentation directory not found at $installDir"
}

# Check if EventVideoPlayback parent directory is now empty
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
