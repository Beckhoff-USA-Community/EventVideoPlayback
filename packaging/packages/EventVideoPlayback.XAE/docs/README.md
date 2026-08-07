# EventVideoPlayback.XAE

TwinCAT XAE workload that installs the complete Event Video Playback engineering tooling.

## What's Included

This workload installs:
* EventVideoPlayback PLC library
* EventVision HMI control
* Documentation package

## Installation

The package feed must be set up once per machine. [Adding the package feed](https://github.com/Beckhoff-USA-Community/BAUS_Community_General_Assets/blob/main/feed-setup.md) walks through it. Then install via TwinCAT Package Manager:

```
tcpkg install Beckhoff-USA-Community.EventVideoPlayback.XAE
```

## Requirements

* TwinCAT 3 XAE (Build 4026 or higher)
* TwinCAT Vision XAE
* TE2000 HMI Engineering (optional, for the HMI control)

## Documentation

Full documentation: https://beckhoff-usa-community.github.io/EventVideoPlayback/

Sample projects: https://github.com/Beckhoff-USA-Community/EventVideoPlayback_Samples
