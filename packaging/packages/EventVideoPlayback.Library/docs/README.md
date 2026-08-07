# EventVideoPlayback.Library

TwinCAT PLC library package containing the FB_ImageToVideo function block and supporting data structures for event-triggered video capture functionality.

## Features

- FB_ImageToVideo function block for video creation from image sequences
- Event-triggered video capture integration
- TwinCAT Vision camera support
- Alarm-based video recording

## Installation

The package feed must be set up once per machine. [Adding the package feed](https://github.com/Beckhoff-USA-Community/BAUS_Community_General_Assets/blob/main/feed-setup.md) walks through it. Then install via TwinCAT Package Manager:
```
tcpkg install Beckhoff-USA-Community.XAE.PLC.Lib.EventVideoPlayback
```

## Requirements

- TwinCAT 3 XAE (Build 4024 or higher)
- TwinCAT Standard Library
- TF7xxx Vision License

## Documentation

Full documentation available at: https://github.com/Beckhoff-USA-Community/EventVideoPlayback