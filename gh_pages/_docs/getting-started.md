---
layout: doc
title: Getting Started
description: Learn how to install and configure Event Video Playback for your TwinCAT project
permalink: /docs/getting-started/
---

## Overview

Event Video Playback is a comprehensive solution for transforming TwinCAT Vision images into event-driven video recordings. This guide will walk you through the installation and initial setup process.

## Prerequisites

Before you begin, ensure you have the following installed on your system:

- **TwinCAT 3.1** Build 4024 or higher
- **TwinCAT Vision** 4.0 or higher
- **Windows 10/11** or Windows Server 2016+
- **.NET 8 Runtime** (required for the Windows service)
- **TwinCAT Event Logger** (optional, for automatic video logging)

## Installation Steps

### 1. Download the Package

You can obtain Event Video Playback from the [Beckhoff USA Community Package Feed](https://packages.beckhoff-usa-community.com/):

```bash
# Using TwinCAT Package Manager
tcpkg install EventVideoPlayback
```

Or download directly from the [GitHub Releases](https://github.com/Beckhoff-USA-Community/EventVideoPlayback/releases) page.

### 2. Install the Windows Service

The EventVideoPlayback Windows service handles video creation and management:

1. Run the installer with administrator privileges
2. The service will be automatically installed and configured
3. The service starts automatically and runs in the background
4. Default installation path: `C:\Program Files\Beckhoff USA Community\EventVideoPlayback\Service\`

### 3. Install the PLC Library

Add the Event Video Playback library to your TwinCAT project:

1. Open your TwinCAT project in Visual Studio
2. In the Solution Explorer, right-click on **References** in your PLC project
3. Select **Add Library**
4. Search for "EventVideoPlayback"
5. Select the library and click **OK**

### 4. Configure the Service

The service configuration file is located at:

```
C:\Program Files\Beckhoff USA Community\EventVideoPlayback\Service\EventVideoPlaybackService.config.json
```

See the [Service Configuration]({{ '/docs/service-config/' | relative_url }}) guide for detailed configuration options.

### 5. Set Up TwinCAT Vision

Ensure your TwinCAT Vision system is configured to save images to a known location. The Event Video Playback service will monitor this location for image sequences to compile into videos.

## Quick Start Example

Here's a minimal example to get you started:

### PLC Code Example

```iecst
PROGRAM MAIN
VAR
    fbVideoRecorder : FB_EventVideoRecorder;
    bTriggerEvent   : BOOL := FALSE;
    sVideoName      : STRING := 'MachineEvent_001';
END_VAR

// Trigger video recording on event
IF bTriggerEvent THEN
    fbVideoRecorder.Record(
        sVideoName := sVideoName,
        bExecute := TRUE
    );
    bTriggerEvent := FALSE;
END_IF

// Call function block cyclically
fbVideoRecorder();
```

### Expected Behavior

1. When `bTriggerEvent` is set to TRUE, the system captures recent images
2. Images are compiled into an MP4 video file
3. The video is saved with the specified name
4. (Optional) Event is logged to TwinCAT Event Logger
5. Video can be played back via HMI

## Verification

To verify your installation:

1. **Check Service Status**: Open Windows Services and confirm "EventVideoPlayback Service" is running
2. **Test PLC Connection**: Run the PLC code example above
3. **Verify Video Creation**: Check the configured output folder for generated MP4 files
4. **Review Logs**: Check the service logs for any errors or warnings

## Next Steps

- Learn about [PLC Library Usage]({{ '/docs/plc-usage/' | relative_url }}) for advanced features
- Configure [Service Settings]({{ '/docs/service-config/' | relative_url }}) for your environment
- Add [HMI Controls]({{ '/docs/hmi-usage/' | relative_url }}) for video playback

## Troubleshooting

### Service Won't Start

- Verify .NET 8 Runtime is installed
- Check Windows Event Viewer for error messages
- Ensure no other service is using ADS port 26129

### Videos Not Being Created

- Confirm TwinCAT Vision is saving images to the configured path
- Check service configuration file for correct paths
- Verify sufficient disk space is available

### PLC Function Block Errors

- Ensure the library reference is added correctly
- Verify the service is running
- Check ADS communication settings

## Support

Need help? Here are some resources:

- [GitHub Issues](https://github.com/Beckhoff-USA-Community/EventVideoPlayback/issues) - Report bugs or request features
- [Beckhoff USA Community](https://github.com/Beckhoff-USA-Community) - Community support and discussions
- [Documentation]({{ '/' | relative_url }}) - Additional guides and references
