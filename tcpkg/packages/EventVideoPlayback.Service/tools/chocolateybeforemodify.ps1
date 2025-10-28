# Stop the service before upgrade or uninstall
$serviceName = "EventVideoPlaybackService"

$existingService = Get-Service -Name $serviceName -ErrorAction SilentlyContinue

if ($existingService) {
    Write-Host "Stopping service '$serviceName' before modification..."
    Stop-Service -Name $serviceName -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
}
