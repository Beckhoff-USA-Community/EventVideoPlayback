# Event Video Playback

This repository includes both the source files and the release package for the TwinCAT Event Video Playback package. The package provides an easy to use PLC interface for assembling images captured with TwinCAT Vision into a single video file. When the video file is created, a corresponding alarm event is logged into the TwinCAT Event Logger for later viewing. In addition, an HMI Control component is supplied for easy viewing and playback of logged video events on TwinCAT HMI.

**WARNING: Remove any installs from the legacy TC_EventVideoPlayback before using the new 4026 version**

## Quick Start

**For end users**: Instead of building from source, install from the Beckhoff USA Community package feed with TwinCAT Package Manager. From an administrator PowerShell, entering your GitHub personal access token when it prompts for a password:

```powershell
tcpkg config unset -n VerifySignatures
tcpkg source add -n "Beckhoff-USA-Community" -s https://nuget.pkg.github.com/Beckhoff-USA-Community/index.json -u <your GitHub user name> --take 100
tcpkg install Beckhoff-USA-Community.EventVideoPlayback.XAE -y
```

Install `Beckhoff-USA-Community.EventVideoPlayback.XAR` instead on a runtime target. The first two commands set up the feed and are only needed once per machine. [Adding the package feed](https://github.com/Beckhoff-USA-Community/BAUS_Community_General_Assets/blob/main/feed-setup.md) walks through creating the token, explains why both are required and why the order matters, and covers the common failures. The [installation guide](https://beckhoff-usa-community.github.io/EventVideoPlayback/docs/installation/) covers this project specifically, including offline installs.

The former community feed at packages.beckhoff-usa-community.com has been retired. If you added it previously, remove it and add the feed above instead.

**For developers**: See [Building from Source](#building-from-source) below.

## Documentation

Find the latest up-to-date [documentation here](https://beckhoff-usa-community.github.io/EventVideoPlayback/) on GitHub pages, or you can find them after install under the install path `/Documentation`.

## Example Projects

The latest examples for using EventVideoPlayback are held in a seperate [GitHub repository here](https://github.com/Beckhoff-USA-Community/EventVideoPlayback_Samples).

## Repository Structure

```
EventVideoPlayback/
├── src/                       # EventVideoPlaybackService (C# Windows service)
├── plc/                       # EventVideoPlayback PLC library (TwinCAT PLC)
├── hmi/                       # EventVision HMI control (TwinCAT HMI)
├── tests/                     # Service unit tests
├── packaging/                 # TwinCAT Package build system
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
dotnet restore EventVideoPlayback.sln
dotnet build EventVideoPlayback.sln --configuration Release
dotnet test EventVideoPlayback.sln
```

#### 2. TwinCAT PLC Library

1. Open `plc/EventVideoPlayback/EventVideoPlayback.sln` in TwinCAT XAE
2. Build the solution (F7)
3. Install as library via TwinCAT Library Repository

#### 3. TwinCAT HMI Control

1. Open `hmi/TcHmiEventVideo/TcEventVideoPlayback-TcHMIControl.sln` in Visual Studio
2. Restore NuGet packages
3. Build solution (Ctrl+Shift+B)

#### 4. TwinCAT Packages

```powershell
cd packaging
.\Build.ps1 -CleanBuild
```

Built packages will be in `packaging/release/`.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

### Third-Party Licenses

This project includes or depends on the following third-party components:

- **OpenCvSharp4**: Apache License 2.0 - see [LICENSE-OpenCvSharp4](LICENSE-OpenCvSharp4)
- **OpenH264 Codec**: BSD 2-Clause License - see [LICENSE-openh264.txt](LICENSE-openh264.txt)

All third-party licenses are included in this repository and must be retained in any distribution.

## How to get support

Should you have any questions regarding the provided sample code, please post to the issue tracker for this repository.

## Disclaimer

All sample code provided by Beckhoff Automation LLC are for illustrative purposes only and are provided "as is" and without any warranties, express or implied. Actual implementations in applications will vary significantly. Beckhoff Automation LLC shall have no liability for, and does not waive any rights in relation to, any code samples that it provides or the use of such code samples for any purpose.
