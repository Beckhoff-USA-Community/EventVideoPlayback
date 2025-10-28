# EventVideoPlayback Component Descriptions

This document provides concise technical descriptions of all major components in the EventVideoPlayback system.

---

## System Overview

**EventVideoPlayback System**: An integrated TwinCAT Vision solution that captures images from industrial cameras, automatically assembles them into video files when alarms occur, and provides HMI playback capabilities through coordinated PLC, Windows service, and web-based HMI components communicating via TwinCAT ADS protocol.

---

## PLC Library Components

### FB_ImageToVideo
Primary function block that manages the image-to-video workflow by maintaining a ring buffer of captured images, subscribing to TwinCAT Event Logger alarms with JSON attributes, and triggering video creation by writing images to disk and sending JSON-formatted requests to the Windows service via ADS communication. This function block extends FB_ListenerBase2 to handle alarm events and coordinates with TwinCAT Vision to resize/process images before storage.

### Parameters_EventVideoPlayback (TcGVL)
Global variable list containing system-wide configuration parameters including MAX_NUMBER_IMAGES_2_VIDEO (ring buffer size), ADS_PORT_FOR_IMAGE_TO_VIDEO (ADS port for service communication), MAX_PERCENT_ROUTER_MEM_FOR_BUFFER (memory allocation limits), and LARGE_ROUTER_MEMORY (fallback memory size for systems with >4GB router memory).

### Project Information Functions
Utility functions (F_GetCompany, F_GetTitle, F_GetVersion) that provide library metadata and version information for the EventVideoPlayback PLC library, enabling programmatic access to library identification details.

---

## Windows Service Components

### EventVideoPlaybackService (Program.cs)
Entry point for the .NET 8 Windows Service application that configures the host builder with Windows Service support, event logging, and registers the WindowsBackgroundService as the primary hosted service responsible for ADS communication and video processing.

### WindowsBackgroundService (Worker.cs)
Background service that loads configuration from JSON file (codec settings, ADS port, video retention policies, folder size limits), instantiates and manages the AdsImageToVideoServer lifecycle, and handles graceful shutdown with proper resource disposal to prevent memory leaks and ensure clean service termination.

### AdsImageToVideoServer
ADS server implementation that listens on a configurable port (25000-26999 range), receives JSON-formatted video creation requests from PLC clients containing image source paths and video parameters, queues video encoding as background tasks to prevent ADS communication blocking, and manages video file lifecycle through time-based and size-based maintenance policies. This class extends TwinCAT.Ads.Server.AdsServer and implements comprehensive ADS protocol handlers including read, write, device state, and notification management operations.

### ConfigData
Configuration data class defining all service parameters including CodecFourCC (video codec selection like "avc1" for H.264), VideoDeleteTime (retention period in days), AdsPort (ADS communication port), and MaxFolderSize (maximum total video storage in MB), with built-in fallback defaults when configuration file is missing or malformed.

### Logger (ServerLogger)
Wrapper class that integrates Microsoft.Extensions.Logging.ILogger with TwinCAT ADS server logging, providing comprehensive request/response logging for all ADS operations including write indications, read confirmations, device notifications, and state changes for debugging and monitoring purposes.

### NotificationRequestEntry
Data structure that tracks ADS device notification subscriptions, storing the receiver address, index group/offset, data length, and notification settings for managing push-based data change notifications from PLC clients.

---

## HMI Control Component

### EventVisionControl (TypeScript)
TwinCAT HMI custom control that extends TcHmiEventGrid to display alarm events with integrated video playback capability, providing a details popup (DetailsVideoPopup) that checks for video file availability using configurable retry logic and displays videos with timeline scrubbing and playback controls. This control manages virtual directory mappings for video access, timezone conversion for event timestamps, and configurable video player dimensions.

### DetailsVideoPopup (JavaScript)
Popup dialog component that handles video file verification (with retry logic for files still being encoded), creates HTML5 video player instances with custom controls, and correlates alarm timestamps with video playback by applying timezone conversions to ensure operators see videos captured around the event time.

---

## TcPkg Packages

### EventVideoPlayback.Library
TwinCAT PLC library package (.library) containing the FB_ImageToVideo function block and supporting data structures, distributed through TwinCAT Library Repository for installation into PLC projects requiring event-triggered video capture functionality.

### EventVideoPlayback.Service
Windows service installer package containing the compiled EventVideoPlaybackService executable, OpenCV dependencies (OpenCvSharp, OpenCvSharpExtern), codec libraries (OpenH264, FFmpeg), and configuration files for deployment and installation as a Windows service on industrial PCs.

### EventVideoPlayback.XAE
TwinCAT XAE project package containing a complete sample PLC project demonstrating FB_ImageToVideo usage, event logger configuration, and integration patterns for capturing images from TwinCAT Vision cameras and triggering video creation on alarms.

### EventVideoPlayback.XAR
TwinCAT HMI Server Extension package (.tpzip) providing server-side components, virtual directory mappings for video file access, and API extensions required for the EventVisionControl to retrieve and stream video files from the HMI server.

### EventVision.HMI
TwinCAT HMI Control package (.hmiext) containing the EventVisionControl custom control, compiled TypeScript/JavaScript, CSS themes, HTML templates, and NuGet dependencies for installation into TwinCAT HMI projects through the HMI control package manager.

### EventVideoPlayback.Documentation
Documentation package containing user guides, API documentation, architecture diagrams, and sample code that is deployed to the installation directory's Documentation folder and published to GitHub Pages for online access.

---

## Supporting Components

### CodecLibs (Shared Project)
Shared project containing video codec libraries including openh264-1.8.0-win64.dll (BSD-licensed H.264 encoder/decoder) that is referenced by the Windows service for video encoding operations, ensuring consistent codec availability across build configurations.

### EventVideoPlaybackService.config.json
JSON configuration file specifying runtime parameters including CodecFourCC (default "avc1"), VideoDeleteTime (retention in days), AdsPort (default 26128), and MaxFolderSize (storage limit in MB), allowing operators to customize service behavior without recompiling.

### appsettings.json / appsettings.Development.json
.NET service configuration files providing logging levels, event log settings, and environment-specific configurations that control service behavior and diagnostic output during development and production deployment.

### OpenCV Dependencies
OpenCvSharp4 wrapper libraries (OpenCvSharp.dll, OpenCvSharpExtern.dll) and native OpenCV binaries (opencv_videoio_ffmpeg4110_64.dll) that provide computer vision functionality including image resizing, format conversion, video writing with codec support, and PNG file I/O operations.

---

## System Integration Flow

1. **Image Capture**: PLC continuously captures images from TwinCAT Vision cameras and stores them in FB_ImageToVideo's ring buffer with configurable reduction factor for size optimization
2. **Event Trigger**: When an alarm occurs with matching JSON attribute, FB_ImageToVideo writes buffered images to disk (C:\TcAlarmVideo\CameraName\Images\) with timestamped filenames
3. **Video Request**: PLC sends JSON message via ADS to EventVideoPlaybackService containing ImagePathSource, VideoFilename, and VideoFPS parameters
4. **Video Encoding**: Windows service uses OpenCV to read PNG images, encode them into MP4 video using H.264 codec at specified frame rate, and deletes source images after successful encoding
5. **Event Logging**: TwinCAT Event Logger receives alarm with video filename embedded in event data, creating correlation between alarm and video file
6. **HMI Playback**: EventVisionControl displays events in grid, operator clicks to view details popup which loads and plays the correlated video with timeline controls and timestamp synchronization
7. **Maintenance**: Service automatically deletes videos older than configured retention period and enforces maximum folder size by removing oldest files when limit exceeded

---

## Key Technologies

- **TwinCAT 3**: Industrial automation platform providing PLC runtime, Vision system, Event Logger, and HMI server (version 4026+ required)
- **.NET 8**: Modern cross-platform framework for Windows service with improved performance, dependency injection, and structured logging
- **TwinCAT ADS**: Automation Device Specification protocol enabling real-time communication between PLC and Windows service over TCP or shared memory
- **OpenCvSharp4**: .NET wrapper for OpenCV providing computer vision algorithms, image processing, and video codec integration with native performance
- **OpenH264**: Cisco's open-source H.264 codec (BSD license) providing industry-standard video compression with broad player compatibility
- **TwinCAT HMI**: Web-based HTML5/JavaScript HMI framework with TypeScript controls, server extensions, and responsive design capabilities

---

## Configuration Parameters Summary

| Parameter | Default | Description |
|-----------|---------|-------------|
| CodecFourCC | "avc1" | Video codec (H.264 recommended) |
| VideoDeleteTime | 5-10 days | Retention period for videos |
| AdsPort | 26128/26129 | ADS port for PLC-service communication |
| MaxFolderSize | 250 MB | Maximum video storage before cleanup |
| MAX_NUMBER_IMAGES_2_VIDEO | Configured in GVL | Ring buffer size / max frames per video |
| FramesPerSecond | 10 | Video playback frame rate |
| TimeBeforeEvent | 7 seconds | Video duration before alarm |
| TimeAfterEvent | 3 seconds | Video duration after alarm |
| ReductionFactor | 0.25 | Image scaling factor (1.0 = no scaling) |
| VideoOutputDirectory | C:\TcAlarmVideo | Root path for video storage |

---

*Generated for EventVideoPlayback v2.0.0 - For detailed API documentation, see the full documentation package or visit https://Beckhoff-USA-Community.github.io/TC_EventVideoPlayback/*
