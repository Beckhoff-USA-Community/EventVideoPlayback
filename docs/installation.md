# Installation

## Overview

Event Video Playback records what a TwinCAT Vision camera saw around the moment an event fired. The PLC library keeps a ring buffer of camera images and listens for events, a Windows service encodes the buffered images into an MP4 file, and an HMI control plays the video from the alarm's details popup. This guide covers installing all three components with TwinCAT Package Manager, Beckhoff's installer for TwinCAT workloads and packages.

!!! note
    Install TwinCAT Package Manager before you start. Every `tcpkg` command in this guide runs from an administrator PowerShell, because package installation changes the system.

## System Requirements

Before you begin, make sure the following is installed on your system.

### For Engineering Development

* Windows 10 or 11
* TwinCAT Package Manager
* TwinCAT 3.1 XAE, build 4026 or higher
* TwinCAT Vision XAE, version 5.8.4 or higher
* TwinCAT HMI Engineering, version 14.16 or newer

### For Runtime Targets

* Windows 10 or 11
* TwinCAT Package Manager
* TwinCAT 3.1 XAR, build 4026 or higher
* TwinCAT Vision XAR, version 5.8.4 or higher
* TwinCAT HMI Server, version 14.16 or newer

Version 14.16 is the tested TwinCAT HMI Engineering and Server line for the current release.

### HMI NuGet Packages

You do not need to pick HMI NuGet package versions by hand. The EventVision control package declares its minimum dependency versions (Beckhoff.TwinCAT.HMI.Framework 14.5.1, Beckhoff.TwinCAT.HMI.Controls 14.7.1, Beckhoff.TwinCAT.HMI.EventLogger 23.0.275) and NuGet resolves them automatically when you add the control package to an HMI project. The current control version is 1.2.1. The tested combinations for the 1.2.x line are recorded in the [EventVision version matrix](https://github.com/Beckhoff-USA-Community/EventVideoPlayback/blob/main/packaging/packages/EventVision.HMI/docs/README.md).

!!! danger
    Previous versions of the legacy 4024 Tc_EventVideoPlayback project must be uninstalled before you use the new 4026 build. The 4026 packages replace the legacy project rather than upgrade it in place.

## Package Signing Configuration

TwinCAT Package Manager only accepts packages officially signed by Beckhoff Automation GmbH & Co. KG by default. Community packages carry no Beckhoff signature, so you need to disable signature verification before adding the feed or installing packages.

!!! warning
    Disabling signature verification allows installation of third party packages. Only use packages from trusted community sources, and review package contents and source code before installation. All community packages are provided as is, without warranties.

To disable signature checks, run this command in an administrator PowerShell:

```powershell
tcpkg config unset -n VerifySignatures
```

## Installation Methods

Choose the installation method that fits your environment:

* [Online installation](#online-package-feed-connection) for systems with internet access. This is the recommended path and the one that delivers updates.
* [Offline installation](#offline-package-configuration) for air gapped systems, or when you need a specific version.

## Online Package Feed Connection

The packages are published to the GitHub package feed of the Beckhoff USA Community organization.

!!! warning
    The old community feed at packages.beckhoff-usa-community.com has been retired. If you added it previously, remove it with `tcpkg source remove -n "Beckhoff USA Community Stable"` and add the feed below instead.

### Step 1: Create a GitHub personal access token

GitHub requires a login for its package feeds, even for public packages, so you need a GitHub account and a personal access token. In your GitHub account settings, under Developer settings, create a token of the classic type with the read:packages scope.

!!! tip
    [Adding the package feed](https://github.com/Beckhoff-USA-Community/BAUS_Community_General_Assets/blob/main/feed-setup.md) walks through creating the token click by click, and covers the common failures. It applies to every Beckhoff USA Community package, not just this project.

### Step 2: Add the feed

Run both commands from an administrator PowerShell, in this order:

```powershell
tcpkg config unset -n VerifySignatures
tcpkg source add -n "Beckhoff-USA-Community" -s https://nuget.pkg.github.com/Beckhoff-USA-Community/index.json -u <your GitHub user name> --take 100
```

Enter the token as the password when prompted, then accept the community disclaimer.

Both steps are required, and the order matters:

* Community packages carry no Beckhoff signature, including the disclaimer package the feed presents while it is being added, so verification must be off before the feed is added.
* GitHub rejects search requests larger than 100 results per page while the package manager asks for 500 by default. Without `--take 100` the command fails with "Failed to retrieve metadata from source".

If adding the feed fails, confirm the token is the classic type (it starts with `ghp_`), that it has the read:packages scope, and that it has not expired.

## Offline Package Configuration

Use this method for air gapped systems, manual installations, or when you need a specific version.

### Step 1: Download from GitHub Releases

1. Open the [GitHub Releases page](https://github.com/Beckhoff-USA-Community/EventVideoPlayback/releases)
2. Find the latest release, or the version you need
3. Download the package file (`.zip`)
4. Transfer the files to a convenient location on your target system, for example `C:\Program Files\Beckhoff USA Community\Feeds\Local`

### Step 2: Add a Local Package Feed

#### Option 1: Add via GUI

1. Open the TwinCAT Package Manager GUI
2. Click the gear icon in the bottom left corner
3. Select **Feeds**
4. Add a new feed with the following settings:
    * Feed Path: `C:\Program Files\Beckhoff USA Community\Feeds\Local`
    * Feed Name: `Beckhoff USA Community Local`
    * Set credentials: leave deselected, a local folder feed needs none
5. Click **Save**

#### Option 2: Add via PowerShell

```powershell
tcpkg source add -n "Beckhoff USA Community Local" -s "C:\Program Files\Beckhoff USA Community\Feeds\Local"
```

## Install Workloads

With the feed added and signature verification disabled, install the Event Video Playback workloads through TwinCAT Package Manager just like any other Beckhoff Automation package. A workload is a grouping package that pulls in every package a use case needs.

1. Open TwinCAT Package Manager
2. Browse the available packages from the Beckhoff USA Community feed
3. Select the Event Video Playback workloads
4. Click **Install**

!!! note
    The installation includes the necessary XAE or XAR components, the PLC library, the HMI packages, and the Windows service.

## Next Steps

After installation is complete, continue with the [Getting Started](./getting-started.md) guide to configure and use Event Video Playback in your project.

## Troubleshooting

### Service does not start

The service ships self contained, so there is no separate .NET runtime to install and a missing runtime is not a possible cause.

* The service appears in the Windows Services list as **EventVideoPlayback Service**. From PowerShell, check it with `Get-Service EventVideoPlaybackService` and start it with `Start-Service EventVideoPlaybackService`
* Check Windows Event Viewer for error messages from the service
* Make sure no other application is using ADS port 26129. The service hosts its own ADS server on that port and cannot start while another program has it registered

### Videos are not created

* Confirm TwinCAT Vision is saving images to the configured path
* The image and video paths are not in the service configuration file; they are set on the PLC function block. Check `VideoOutputDirectory` on your `FB_ImageToVideo` instance and confirm the service can reach that path
* If `VideoRootDirectory` is set in the service configuration, confirm the PLC paths fall inside it
* Verify sufficient disk space is available

### Video player stays black in the HMI

* The HMI server's default Content Security Policy blocks media. See [Content Security Policy](./hmi-control.md#content-security-policy) for the required server setting

### PLC function block errors

* Make sure the library reference is added to the PLC project
* Verify the service is running (`Get-Service EventVideoPlaybackService`)
* Check the ADS communication settings

## Support

Need help? Here are some resources:

* [GitHub Issues](https://github.com/Beckhoff-USA-Community/EventVideoPlayback/issues) to report bugs or request features
* [Beckhoff USA Community](https://github.com/Beckhoff-USA-Community) for community support and discussions
* [Documentation](./index.md) for additional guides and references
