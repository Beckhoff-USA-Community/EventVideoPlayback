# EventVision.HMI

TwinCAT HMI control package providing the EventVisionControl for video playback and management in HMI applications.

## Features

- Custom EventVisionControl for video playback
- Event-based video player interface
- Video file browsing and selection
- Responsive design with theming support
- Timeline and playback controls

## What's Included

- EventVisionControl custom HMI control
- Compiled TypeScript/JavaScript modules
- CSS themes and styling
- HTML templates
- NuGet package dependencies

## Installation

Install via TwinCAT Package Manager:
```
tcpkg install Beckhoff-USA-Community.XAE.HMI.EventVisionControl
```

The control will be available in the TwinCAT HMI control toolbox.

## Requirements

* TwinCAT 3 XAE (Build 4026 or higher)
* TE2000 HMI Engineering
* Beckhoff.TwinCAT.HMI.Framework 14.5.1 or newer and Beckhoff.TwinCAT.HMI.Controls 14.7.1 or newer in the HMI project

## Tested Versions

| EventVision | HMI Framework | HMI Controls | HMI Server |
|---|---|---|---|
| 1.2.0 | 14.5.1 | 14.7.1 | 14.16.0 |
| 1.1.3 | 14.3.154 | 14.3.154 | 14.3.x |

Versions 1.1.3 and older subclass a popup class that newer Controls packages no
longer expose, so they do not work on Controls 14.6 and newer. Version 1.2.0
loads the popup base class from the Controls package at runtime and needs
Controls 14.7.1 or newer.

## Documentation

Full documentation available at: https://github.com/Beckhoff-USA-Community/EventVideoPlayback