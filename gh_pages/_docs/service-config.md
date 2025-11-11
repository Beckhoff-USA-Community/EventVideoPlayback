---
layout: doc
title: Service Configuration
description: Configure the EventVideoPlayback Windows service for optimal performance
permalink: /docs/service-config/
---

## Overview

The EventVideoPlayback Service is a Windows background service that handles video creation and automatic file management. This guide covers all configuration options and troubleshooting steps.

## Configuration File Location

The service configuration file is located at:

```
C:\Program Files\Beckhoff USA Community\EventVideoPlayback\Service\EventVideoPlaybackService.config.json
```

Open this file with Notepad, Visual Studio, or any text editor.

## Configuration Parameters

### Default Configuration

```json
{
  "CodecFourCC": "avc1",
  "VideoDeleteTime": 1,
  "AdsPort": 26129,
  "MaxFolderSize": 250
}
```

### CodecFourCC

**Type:** String
**Default:** `"avc1"`
**Description:** The video codec used to create MP4 videos from image sequences.

The `avc1` codec (H.264) is recommended as it provides:
- Excellent compression
- Wide compatibility with web browsers
- Support for TwinCAT HMI playback
- Good balance between quality and file size

#### Supported Codecs

The service supports multiple codecs, but not all are web-compatible. For TwinCAT HMI compatibility, stick with these recommended options:

| Codec | FourCC | Description | Web Compatible | Recommended |
|-------|--------|-------------|----------------|-------------|
| H.264 | `avc1` | Most common, best compatibility | ✓ | **Yes** |
| H.264 | `avc3` | Alternative H.264 variant | ✓ | Yes |
| H.265/HEVC | `hev1` | Better compression, newer | ✓ | Maybe |
| H.265/HEVC | `hvc1` | Alternative HEVC variant | ✓ | Maybe |
| MPEG-4 | `mp4v` | Older standard | ✓ | No |
| VP9 | `vp09` | Google's codec | ✓ | No |
| AV1 | `av01` | Newest, best compression | ⚠ | No |

#### Changing the Codec

To change the codec, edit the configuration file:

```json
{
  "CodecFourCC": "hev1",  // Changed to H.265
  "VideoDeleteTime": 1,
  "AdsPort": 26129,
  "MaxFolderSize": 250
}
```

**⚠ Important:** After changing any configuration, you must restart the service for changes to take effect.

### VideoDeleteTime

**Type:** Decimal (floating-point)
**Default:** `1`
**Units:** Days
**Description:** How long video files are kept before automatic deletion.

The service automatically cleans up old videos based on this setting. This helps manage disk space and keep only relevant videos.

#### Examples

```json
// Keep videos for 1 day (default)
"VideoDeleteTime": 1

// Keep videos for 12 hours
"VideoDeleteTime": 0.5

// Keep videos for 1 week
"VideoDeleteTime": 7

// Keep videos for 30 days
"VideoDeleteTime": 30

// Keep videos for 2 hours
"VideoDeleteTime": 0.083
```

#### Calculation Reference

| Duration | Value | Calculation |
|----------|-------|-------------|
| 1 hour | 0.042 | 1/24 |
| 6 hours | 0.25 | 6/24 |
| 12 hours | 0.5 | 12/24 |
| 1 day | 1.0 | 24/24 |
| 3 days | 3.0 | - |
| 1 week | 7.0 | - |
| 1 month | 30.0 | - |

### AdsPort

**Type:** Integer
**Default:** `26129`
**Description:** The ADS port number the service listens on.

**⚠ WARNING: DO NOT CHANGE THIS VALUE**

The PLC function blocks are configured to communicate with the service on port 26129. Changing this value will break PLC communication.

If you have a specific need to use a different port, you must:
1. Change this configuration value
2. Recompile the PLC library with the new port number
3. Update all PLC projects using the library

### MaxFolderSize

**Type:** Integer
**Default:** `250`
**Units:** Megabytes (MB)
**Description:** Maximum total size of the video folder.

When a new video is created, the service checks the total folder size. If it exceeds this limit, the oldest videos are automatically deleted to free space.

#### Examples

```json
// Limit to 250 MB (default)
"MaxFolderSize": 250

// Limit to 1 GB
"MaxFolderSize": 1024

// Limit to 5 GB
"MaxFolderSize": 5120

// Limit to 500 MB
"MaxFolderSize": 500
```

#### Size Planning

Consider these factors when setting folder size:

- **Video duration**: Longer pre/post event times = larger files
- **Image resolution**: Higher resolution = larger files
- **Frame rate**: More frames per second = larger files
- **Codec**: Different codecs have different compression ratios
- **Event frequency**: More frequent events = more videos

**Example Calculation:**
- Average video duration: 30 seconds
- Average file size: 5 MB per video
- MaxFolderSize: 250 MB
- Approximate capacity: ~50 videos

## Applying Configuration Changes

After modifying the configuration file:

### Option 1: Restart the Service

1. Open **Windows Services** (services.msc)
2. Find **EventVideoPlayback Service**
3. Right-click and select **Restart**

### Option 2: Use Command Line

```powershell
# Stop the service
net stop "EventVideoPlayback Service"

# Start the service
net start "EventVideoPlayback Service"
```

### Option 3: Reboot

A system reboot will also restart the service with the new configuration.

## Advanced Configuration

### Example: High-Quality, Long Retention

For critical systems where video quality and retention are important:

```json
{
  "CodecFourCC": "avc1",
  "VideoDeleteTime": 30,      // Keep for 30 days
  "AdsPort": 26129,
  "MaxFolderSize": 10240      // 10 GB
}
```

### Example: Space-Constrained, Short Retention

For systems with limited disk space:

```json
{
  "CodecFourCC": "avc1",
  "VideoDeleteTime": 0.5,     // Keep for 12 hours
  "AdsPort": 26129,
  "MaxFolderSize": 100        // 100 MB
}
```

### Example: Maximum Compression

For maximum file size reduction:

```json
{
  "CodecFourCC": "hev1",      // H.265 for better compression
  "VideoDeleteTime": 7,       // Keep for 1 week
  "AdsPort": 26129,
  "MaxFolderSize": 500        // 500 MB
}
```

## Service Management

### Checking Service Status

**Windows Services GUI:**
1. Press `Win + R`, type `services.msc`, press Enter
2. Find "EventVideoPlayback Service"
3. Check the Status column (should show "Running")

**PowerShell:**
```powershell
Get-Service -Name "EventVideoPlayback*"
```

**Command Prompt:**
```cmd
sc query "EventVideoPlayback Service"
```

### Starting the Service

```powershell
Start-Service -Name "EventVideoPlayback Service"
```

### Stopping the Service

```powershell
Stop-Service -Name "EventVideoPlayback Service"
```

### Setting Startup Type

**Automatic (recommended):**
```powershell
Set-Service -Name "EventVideoPlayback Service" -StartupType Automatic
```

**Manual:**
```powershell
Set-Service -Name "EventVideoPlayback Service" -StartupType Manual
```

## Troubleshooting

### Service Won't Start

**Check Event Viewer:**
1. Open Event Viewer (eventvwr.msc)
2. Navigate to **Windows Logs** > **Application**
3. Look for errors from source "EventVideoPlayback"

**Common Causes:**
- Missing .NET 8 Runtime
- Invalid configuration file (JSON syntax error)
- Port 26129 already in use
- Insufficient permissions

**Solutions:**
```powershell
# Verify .NET 8 Runtime is installed
dotnet --list-runtimes

# Check if port is in use
netstat -ano | findstr "26129"

# Run as administrator
net start "EventVideoPlayback Service"
```

### Configuration Not Taking Effect

- Verify you saved the configuration file
- Ensure JSON syntax is valid (use a JSON validator)
- Restart the service after changes
- Check file permissions (service must be able to read the file)

### Videos Not Being Deleted

- Verify `VideoDeleteTime` is set appropriately
- Check that the service has write permissions to the video folder
- Ensure the system clock is correct
- Review service logs for cleanup errors

### Disk Space Issues

If you're running out of disk space:

1. **Reduce MaxFolderSize:**
   ```json
   "MaxFolderSize": 100  // Reduce to 100 MB
   ```

2. **Reduce VideoDeleteTime:**
   ```json
   "VideoDeleteTime": 0.5  // Keep only 12 hours
   ```

3. **Manually clean old videos:**
   - Navigate to the video output folder
   - Delete old MP4 files
   - Service will manage space going forward

### Performance Issues

If video creation is slow:

- **Check CPU usage**: Video encoding is CPU-intensive
- **Use faster storage**: SSD is recommended for video output
- **Reduce image resolution**: Lower resolution = faster encoding
- **Consider codec**: H.264 (avc1) is generally fastest

## Monitoring and Logging

### Log File Location

Service logs are typically located at:
```
C:\Program Files\Beckhoff USA Community\EventVideoPlayback\Service\Logs\
```

### Log Contents

Logs include:
- Service startup and shutdown events
- Video creation requests and completions
- File cleanup operations
- Errors and warnings
- ADS communication status

### Viewing Logs

Use any text editor or PowerShell:

```powershell
# View latest log file
Get-Content "C:\Program Files\Beckhoff USA Community\EventVideoPlayback\Service\Logs\*.log" -Tail 50
```

## Best Practices

1. **Regular Monitoring**: Check service status weekly
2. **Disk Space**: Ensure adequate free space (at least 2x MaxFolderSize)
3. **Backup Configuration**: Keep a copy of your configuration file
4. **Test Changes**: Test configuration changes in a development environment first
5. **Document Settings**: Document why you chose specific values

## Next Steps

- Learn about [PLC Library Usage]({{ '/docs/plc-usage/' | relative_url }})
- Set up [HMI Controls]({{ '/docs/hmi-usage/' | relative_url }})
- Review [Getting Started]({{ '/docs/getting-started/' | relative_url }})
