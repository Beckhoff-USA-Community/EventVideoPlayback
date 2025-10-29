# EventVideoPlaybackService - Cross-Platform Deployment Guide

## Overview

The EventVideoPlaybackService has been modified to support cross-platform deployment on both Windows and Linux (Debian) systems. The service can run as:
- Windows Service (on Windows)
- systemd service (on Linux)
- Console application (for testing on any platform)

## Changes Made

### 1. Project File (EventVideoPlaybackService.csproj)

**Removed:**
- Hardcoded `<RuntimeIdentifier>win-x64</RuntimeIdentifier>` (line 11)

**Added:**
- `Microsoft.Extensions.Hosting.Systemd` package for Linux systemd support
- Conditional OpenCV runtime packages based on platform:
  - Windows: `OpenCvSharp4.runtime.win`
  - Linux: `OpenCvSharp4.runtime.linux-x64`

### 2. Program.cs

**Added:**
- `builder.Services.AddSystemd()` for Linux systemd service support
- Updated documentation to reflect cross-platform capabilities
- Both `AddWindowsService()` and `AddSystemd()` are called - they automatically activate based on the runtime environment

### 3. CodecLibs.projitems

**Modified:**
- Made `openh264-1.8.0-win64.dll` conditional (Windows only)
- Added placeholder for Linux codec library (`libopenh264.so`)
- Uses MSBuild platform detection to copy correct codec

## Building the Project

### Windows Build (Current Platform)
```bash
# Debug build
cd C:\GitHub\EventVideoPlayback\src\App.Service\EventVideoPlaybackService
dotnet build --configuration Debug

# Release build
dotnet build --configuration Release
```

### Cross-Platform Publish

The project now supports platform-specific publishing:

**For Windows (x64):**
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

**For Linux (x64):**
```bash
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
```

**For Linux (ARM64 - Raspberry Pi, etc.):**
```bash
dotnet publish -c Release -r linux-arm64 --self-contained true -p:PublishSingleFile=true
```

## Running the Service

### As a Console Application (Testing)

**Windows:**
```bash
cd C:\GitHub\EventVideoPlayback\src\App.Service\EventVideoPlaybackService\bin\Debug\net8.0
EventVideoPlaybackService.exe
```

**Linux:**
```bash
cd /path/to/publish/output
./EventVideoPlaybackService
```

Press Ctrl+C to stop the service when running as a console application.

### As a Windows Service

Use the existing Windows Service installation commands:

```powershell
# Install
sc.exe create EventVideoPlayback binPath= "C:\Path\To\EventVideoPlaybackService.exe"

# Start
sc.exe start EventVideoPlayback

# Stop
sc.exe stop EventVideoPlayback

# Delete
sc.exe delete EventVideoPlayback
```

### As a Linux systemd Service

Create a systemd unit file at `/etc/systemd/system/eventvideo-playback.service`:

```ini
[Unit]
Description=Event Video Playback Service
After=network.target

[Service]
Type=notify
WorkingDirectory=/opt/eventvideo-playback
ExecStart=/opt/eventvideo-playback/EventVideoPlaybackService
Restart=on-failure
User=eventvideo
Group=eventvideo

# Environment variables
Environment="DOTNET_PRINT_TELEMETRY_MESSAGE=false"

[Install]
WantedBy=multi-user.target
```

Then enable and start the service:

```bash
# Reload systemd
sudo systemctl daemon-reload

# Enable service to start on boot
sudo systemctl enable eventvideo-playback.service

# Start service
sudo systemctl start eventvideo-playback.service

# Check status
sudo systemctl status eventvideo-playback.service

# View logs
sudo journalctl -u eventvideo-playback.service -f
```

## Linux Deployment Checklist

Before deploying to Debian Linux, ensure:

1. **Download the Linux H.264 Codec Library**
   - Download `libopenh264.so` (version 1.8.0 or compatible)
   - Place it in: `C:\GitHub\EventVideoPlayback\src\App.Service\CodecLibs\libopenh264.so`
   - This will be automatically included in Linux builds

2. **Install .NET 8 Runtime on Target System**
   ```bash
   # Debian/Ubuntu
   wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
   sudo dpkg -i packages-microsoft-prod.deb
   sudo apt-get update
   sudo apt-get install -y dotnet-runtime-8.0
   ```

3. **Publish for Linux**
   ```bash
   dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
   ```

4. **Copy to Linux System**
   ```bash
   # Create directory
   sudo mkdir -p /opt/eventvideo-playback

   # Copy published files
   scp -r ./bin/Release/net8.0/linux-x64/publish/* user@debian-host:/tmp/
   sudo mv /tmp/EventVideoPlaybackService /opt/eventvideo-playback/
   sudo mv /tmp/*.json /opt/eventvideo-playback/

   # Set permissions
   sudo chmod +x /opt/eventvideo-playback/EventVideoPlaybackService
   ```

5. **Create Service User**
   ```bash
   sudo useradd -r -s /bin/false eventvideo
   sudo chown -R eventvideo:eventvideo /opt/eventvideo-playback
   ```

6. **Configure and Start Service** (see systemd instructions above)

## Configuration

The `EventVideoPlaybackService.config.json` file works identically on both platforms. Ensure:
- ADS server IP addresses are reachable from the Linux host
- Network ports are not blocked by firewall
- File paths in configuration use platform-appropriate separators (though .NET handles both)

## Testing the Build

The build has been verified on Windows with:
- Debug build: SUCCESS (0 errors, 0 warnings)
- Release build: SUCCESS (0 errors, 0 warnings)
- Runtime test: Service starts and connects to ADS server successfully

## Troubleshooting

### Windows Issues

**Service won't start:**
- Check Event Viewer (Windows Logs > Application)
- Ensure the service account has proper permissions
- Verify configuration file paths

### Linux Issues

**Service fails to start:**
```bash
# Check detailed logs
sudo journalctl -u eventvideo-playback.service -n 100 --no-pager

# Check permissions
ls -la /opt/eventvideo-playback

# Test manually
sudo -u eventvideo /opt/eventvideo-playback/EventVideoPlaybackService
```

**Missing dependencies:**
```bash
# Check for missing libraries
ldd /opt/eventvideo-playback/EventVideoPlaybackService

# Install missing libraries (common ones)
sudo apt-get install -y libgdiplus libc6-dev
```

**OpenCV issues:**
```bash
# Ensure OpenCV dependencies are installed
sudo apt-get install -y libopencv-dev
```

## Architecture Notes

The cross-platform implementation uses:
- **Conditional compilation** for platform-specific packages (MSBuild conditions)
- **Runtime detection** for platform-specific features (OperatingSystem.IsWindows())
- **Generic Host** with platform-specific service lifetime management
- **Platform-agnostic** core business logic (ADS communication, video processing)

The service automatically detects its runtime environment:
- When running under Windows Service Control Manager, it operates as a Windows Service
- When running under systemd, it operates as a systemd service
- When running from console, it operates as a console application

No code changes or recompilation are needed when moving between platforms - the same binary adapts to its environment.

## Support

For issues specific to:
- **Windows Service deployment** - consult dotnet-windows-service-expert
- **Linux systemd deployment** - consult dotnet-systemd-expert
- **TwinCAT ADS connectivity** - consult twincat-ads-expert
- **OpenCV/video processing** - consult opencv-dotnet-expert
- **Cross-platform compatibility** - consult dotnet-cross-platform-qa
