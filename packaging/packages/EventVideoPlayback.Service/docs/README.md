# EventVideoPlayback.Service

Windows service package for the EventVideoPlayback system that processes image sequences into video files using OpenCV and FFmpeg.

## Features

- Background Windows service for video processing
- OpenCV-based image processing
- H.264 video encoding support
- Configurable codec settings
- Industrial PC optimized

## What's Included

- EventVideoPlaybackService.exe
- OpenCV dependencies (OpenCvSharp, OpenCvSharpExtern)
- Codec libraries (OpenH264, FFmpeg)
- Service configuration files
- Installation and uninstallation scripts

## Installation

The package feed must be set up once per machine. [Adding the package feed](https://github.com/Beckhoff-USA-Community/BAUS_Community_General_Assets/blob/main/feed-setup.md) walks through it. Then install via TwinCAT Package Manager:
```
tcpkg install Beckhoff-USA-Community.XAR.Service.EventVideoPlayback
```

The service will be automatically installed and configured as a Windows service.

## Requirements

- Windows 10 IoT Enterprise or Windows 11
- .NET Framework 4.8 or higher
- TwinCAT 3 XAR Runtime

## Documentation

Full documentation available at: https://github.com/Beckhoff-USA-Community/EventVideoPlayback