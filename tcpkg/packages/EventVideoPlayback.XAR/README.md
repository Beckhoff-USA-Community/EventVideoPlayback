# Event Video Playback XAR Workload Package

## Package Type
TwinCAT XAR Workload Package (Runtime Meta-Package)

## What is a Workload Package?
A workload package is a meta-package that installs multiple related components together as a single unit. This XAR workload is designed for TwinCAT runtime systems and includes all necessary components for running Event Video Playback applications in production.

## Prerequisites
None - all dependencies are handled automatically by tcpkg during installation.

## Building the Package

### Step 1: Pack the Package
```bash
tcpkg pack EventVideoPlayback.XAR.Workload.nuspec -o ../packages
```

Or using the full path:
```bash
tcpkg pack "C:\GitHub\EventVideoPlayback\tcpkg\packages\EventVideoPlayback.XAR\EventVideoPlayback.XAR.Workload.nuspec" -o "C:\GitHub\EventVideoPlayback\tcpkg\packages"
```

## Installation
Once packed, install the package with:
```bash
tcpkg install Beckhoff-USA-Community.XAR.EventVideoPlayback
```

This will automatically install all required components for runtime operation:
- Event Video Playback background service
- All necessary dependencies

## Included Dependencies
This workload package installs the following components:
- **Beckhoff-USA-Community.XAR.Service.EventVideoPlayback** (2.0.0) - Background Windows service for video playback event management

## Uninstallation
```bash
tcpkg uninstall Beckhoff-USA-Community.XAR.EventVideoPlayback
```

This will:
- Stop and remove the Event Video Playback service
- Remove all service files and installation directories
- Clean up Windows service registration

## Customization
Before building, update the `.nuspec` file with:
- Your company name in `<authors>`
- Your project URL in `<projectUrl>`
- Your copyright information
- The correct version number
- Dependency versions if needed

**Note:** Do not modify the `<packageTypes>` section - this defines the package as a workload.
