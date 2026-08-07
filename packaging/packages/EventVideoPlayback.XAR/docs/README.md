# EventVideoPlayback.XAR

TwinCAT XAR workload that installs the Event Video Playback Windows service on runtime targets.

## What's Included

This workload installs:
* EventVideoPlayback Windows service, published self contained so no separate .NET runtime is needed
* OpenCV processing libraries and the OpenH264 codec
* Service configuration file

## Installation

The package feed must be set up once per machine. [Adding the package feed](https://github.com/Beckhoff-USA-Community/BAUS_Community_General_Assets/blob/main/feed-setup.md) walks through it. Then install via TwinCAT Package Manager:

```
tcpkg install Beckhoff-USA-Community.EventVideoPlayback.XAR
```

The service is installed and started automatically.

## Requirements

* Windows 10 or Windows 11 (x64)
* TwinCAT 3 XAR runtime

## Documentation

Full documentation: https://beckhoff-usa-community.github.io/EventVideoPlayback/
