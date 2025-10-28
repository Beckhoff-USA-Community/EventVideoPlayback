# Event Video Playback

This repository includes both the source files and the release package for the TwinCAT Event Video Playback package. The package provides an easy to use PLC interface for assembling images captured with TwinCAT Vision into a single video file. When the video file is created, a corresponding alarm event is logged into the TwinCAT Event Logger for later viewing. In addition, an HMI Control component is supplied for easy viewing and playback of logged video events on TwinCAT HMI.

## Quick Start

**For end users**: Instead of building from source, download the installer from the [Releases section](https://github.com/Beckhoff-USA-Community/EventVideoPlayback/releases). The release package includes:
- Sample PLC project
- Sample HMI project
- PLC library
- Windows service installer

**For developers**: See [Building from Source](#building-from-source) below.

## Documentation

Find the latest up-to-date [documentation here](https://Beckhoff-USA-Community.github.io/TC_EventVideoPlayback/) on GitHub pages, or you can find them after install under the install path `/Documentation`.

## Repository Structure

```
EventVideoPlayback/
├── src/
│   ├── App.Service/           # EventVideoPlaybackService (C# Windows Service)
│   ├── HMI/                   # EventVision HMI Control (TwinCAT HMI)
│   └── PLC/                   # EventVideoPlayback PLC Library (TwinCAT PLC)
├── tcpkg/                     # TwinCAT Package build system
├── docs/                      # Documentation assets
└── LICENSE                    # License information
```

## System Requirements

- **TwinCAT XAE**: Version 3.1.4024 or later
- **.NET SDK**: 8.0 or later
- **Visual Studio**: 2022 or later
- **Operating System**: Windows 10/11 (x64)
- **TwinCAT Version**: TwinCAT 3 (for PLC library usage)

## Building from Source

### Prerequisites

1. Install [TwinCAT XAE](https://www.beckhoff.com/en-us/products/automation/twincat/)
2. Install [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
3. Install [Visual Studio 2022](https://visualstudio.microsoft.com/) with:
   - .NET desktop development workload
   - C++ desktop development (for native dependencies)

### Build Instructions

#### 1. Windows Service

```bash
cd src/App.Service
dotnet restore EventVideoPlaybackService.sln
dotnet build EventVideoPlaybackService.sln --configuration Release
```

#### 2. TwinCAT PLC Library

1. Open `src/PLC/EventVideoPlayback/EventVideoPlayback.sln` in TwinCAT XAE
2. Build the solution (F7)
3. Install as library via TwinCAT Library Repository

#### 3. TwinCAT HMI Control

1. Open `src/HMI/TcHmiEventVideo/TcEventVideoPlayback-TcHMIControl.sln` in Visual Studio
2. Restore NuGet packages
3. Build solution (Ctrl+Shift+B)

#### 4. TwinCAT Packages

```powershell
cd tcpkg
.\Build.ps1 -CleanBuild
```

Built packages will be in `tcpkg/release/`.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

### Third-Party Licenses

This project includes or depends on the following third-party components:

- **OpenCvSharp4**: Apache License 2.0 - see [LICENSE-OpenCvSharp4](LICENSE-OpenCvSharp4)
- **OpenH264 Codec**: BSD 2-Clause License - see [LICENSE-openh264.txt](LICENSE-openh264.txt)

All third-party licenses are included in this repository and must be retained in any distribution.

## How to get support

Should you have any questions regarding the provided sample code, please contact your local Beckhoff support team. Contact information can be found on the official Beckhoff website at https://www.beckhoff.com/en-us/support/.

## Disclaimer

All sample code provided by Beckhoff Automation LLC are for illustrative purposes only and are provided "as is" and without any warranties, express or implied. Actual implementations in applications will vary significantly. Beckhoff Automation LLC shall have no liability for, and does not waive any rights in relation to, any code samples that it provides or the use of such code samples for any purpose.
