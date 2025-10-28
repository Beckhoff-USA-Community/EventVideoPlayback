# EventVideoPlaybackService Package

## Package Type
Windows Service Package

## Prerequisites
Before building this package, you must publish the service application.

### Step 1: Publish the Service
The service needs to be published as a self-contained executable. Run:

```bash
cd "C:\GitHub\EventVideoPlayback\src\App.Service\EventVideoPlaybackService"
dotnet publish -c Release -r win-x64 --self-contained true -o "C:\GitHub\EventVideoPlayback\artifacts\service"
```

This will create a self-contained deployment that includes the .NET runtime.

### Step 2: Copy Service Files to Package
```bash
xcopy "C:\GitHub\EventVideoPlayback\artifacts\service\*" "C:\GitHub\EventVideoPlayback\tcpkg\EventVideoPlaybackService\service\" /E /I /Y
```

### Step 3: Add an Icon (Optional)
Place an icon.png file in this directory, or copy from the reference:
```bash
copy "C:\Agent_Resources\V3\NewPackageTest\NewPackageTest\TF1000-Base.png" "C:\GitHub\EventVideoPlayback\tcpkg\EventVideoPlaybackService\icon.png"
```

### Step 4: Pack the Package
```bash
tcpkg pack "C:\GitHub\EventVideoPlayback\tcpkg\EventVideoPlaybackService\EventVideoPlaybackService.nuspec" -o "C:\GitHub\EventVideoPlayback\tcpkg\packages"
```

## Installation
Once packed, install the package with:
```bash
tcpkg install EventVideoPlaybackService
```

This will:
- Copy service files to `C:\Program Files\Beckhoff USA Community\EventVideoPlayback\Service\`
- Create a Windows service named "EventVideoPlaybackService"
- Configure it for Automatic startup with Delayed Start
- Configure automatic restart on failure
- Start the service

## Service Management
After installation, you can manage the service using:

```powershell
# Check service status
Get-Service EventVideoPlaybackService

# Stop the service
Stop-Service EventVideoPlaybackService

# Start the service
Start-Service EventVideoPlaybackService

# View service details
Get-Service EventVideoPlaybackService | Select-Object *
```

## Uninstallation
```bash
tcpkg uninstall EventVideoPlaybackService
```

This will:
- Stop the service
- Remove the service registration
- Delete service files from `C:\Program Files\Beckhoff USA Community\EventVideoPlayback\Service\`
- Remove the EventVideoPlayback parent directory if no other components remain installed

## Customization
Before building, update the `.nuspec` file with:
- Your company name in `<authors>`
- Your project URL in `<projectUrl>`
- Your copyright information
- The correct version number

## Service Configuration
The service is configured with:
- **Service Name**: EventVideoPlaybackService
- **Display Name**: EventVideoPlayback Service
- **Description**: Background service for video playback event management
- **Startup Type**: Automatic (Delayed Start)
- **Recovery**: Restart after 60 seconds on failure (3 attempts)
- **Installation Path**: C:\Program Files\Beckhoff USA Community\EventVideoPlayback\Service\

## Dependencies
The service has the following NuGet package dependencies:
- Beckhoff.TwinCAT.Ads (6.2.521)
- Microsoft.Extensions.Hosting.WindowsServices (9.0.10)
- OpenCvSharp4 (4.11.0.20250507)
- OpenCvSharp4.runtime.win (4.11.0.20250507)

These are included in the self-contained publish output.
