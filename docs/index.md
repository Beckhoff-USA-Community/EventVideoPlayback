# Overview

Event Video Playback records video of machine events from TwinCAT Vision cameras. A PLC function block keeps a rolling buffer of camera images in memory. When your code raises an event, the images from before and after that moment are written to disk, a Windows service encodes them into an MP4 video, and the video can be played straight from the matching alarm entry in the TwinCAT HMI.

## Components

The system consists of three parts that install through TwinCAT Package Manager:

* **The PLC library.** `FB_ImageToVideo` maintains an image ring buffer in router memory, raises a TwinCAT event when a capture is triggered, writes the buffered images to disk, and commands the service to encode them. See the [PLC Library](./plc-library.md) reference.
* **The Windows service.** EventVideoPlayback runs on the runtime target, listens on its own ADS port, and turns the saved images into an MP4 file using the H.264 codec. See [Service Configuration](./service-configuration.md).
* **The HMI control.** EventVision extends the stock event grid control. Alarms raised by the PLC library carry the video filename, and the control adds a details popup that plays the matching video. See the [HMI Control](./hmi-control.md) reference.

How the three parts talk to each other, and what each one writes to disk, is covered on the [Architecture](./architecture.md) page.

## Where to start

1. [Installation](./installation.md) covers the package feed setup and the workload installs for the engineering and runtime machines.
2. [Getting Started](./getting-started.md) walks through wiring the function block into a TwinCAT Vision project and adding the control to your HMI.

A working sample PLC and HMI project is available in the [EventVideoPlayback_Samples](https://github.com/Beckhoff-USA-Community/EventVideoPlayback_Samples) repository.

## Requirements

Event Video Playback requires TwinCAT 4026 or newer and TwinCAT Vision. The full requirements list is on the [Installation](./installation.md) page.

!!! note
    Videos play from a published HMI project viewed in a web browser. The HMI Live display inside the engineering environment cannot play them.
