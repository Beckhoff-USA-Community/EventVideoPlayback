---
layout: doc
title: Getting Started
description: Learn how to install and configure Event Video Playback for your TwinCAT project
permalink: /docs/getting-started/
---
## Overview

Event Video Playback is a comprehensive solution for transforming TwinCAT Vision images into event-driven video recordings. This guide will walk you through the installation and initial setup process.

## Prerequisites

Before you begin, ensure you have the following installed on your system:

For Engineering Development:
- **Windows 10/11**
- **TwinCAT Package Manager**
- **TwinCAT 3.1 XAE** Build 4026 or higher
- **TwinCAT Vision XAE** 5.8.4 or higher
- **TwinCAT HMI XAE** 14.9 or higher

For Runtime Targets:
- **Windows 10/11**
- **TwinCAT Package Manager**
- **TwinCAT 3.1 XAR** Build 4026 or higher
- **TwinCAT Vision XAR** 5.8.4 or higher

<div class="alert alert-danger">
<strong>Warning:</strong> Previous versions of the 4024 Tc_EventVideoPlayback legacy project must be uninstalled before using the new 4026 build.
</div>

### TcPkg Package Signing Requirement

TwinCAT Package Manager only accepts officially signed packages from Beckhoff Automation GmbH & Co. KG by default. To use community packages, you need to temporarily disable signature verification. This applies to both locally hosted packages and remote packages; any 3rd party developed packages.

<div class="alert alert-warning">
  <strong>Security Notice:</strong> Disabling signature verification allows installation of third-party packages. Only use packages from trusted community sources. Review package contents and source code before installation. All community packages are provided "as is" without warranties.
</div>

Run this command in PowerShell as Administrator to disable signature checks:
```PowerShell
tcpkg config unset -n VerifySignatures
```

## Installation Methods

Choose the installation method that best fits your environment:

- **[Online Installation](#online-installation)** - For systems with internet access (recommended)
- **[Offline Installation](#offline-installation)** - For air-gapped systems or manual installation

---

## Online Package Feed Connection

**Best for:** Systems with internet connectivity and access to the Beckhoff USA Community package feed.

### Add Package Feed via GUI

If you haven't already, add the Beckhoff USA Community package feed to TwinCAT Package Manager:

1. Open the TwinCAT Package Manager GUI
2. Click the Settings (Gear Icon) in the bottom left corner
3. Select Feeds

For the feed settings use:
```bash
https://packages.beckhoff-usa-community.com/stable/v3/index.json
```
For the feed name use:
```bash
Beckhoff USA Community Stable
```
Deselect the **Set credentials** option, as we do not need login for the feed. Select **Save** and agree to the disclaimer to be connected.

### Add Package Feed via Powershell

```bash
tcpkg source add -n "Beckhoff USA Community Stable" -s "https://packages.beckhoff-usa-community.com/stable/v3/index.json"
```
Agree to the disclaimer to be connected.

---

## Offline Package Configuration

**Best for:** Air-gapped systems, manual installations, or when you need a specific version.

<div class="alert alert-success">
  <strong>Tip:</strong> Instead of downloading the packages from the releases section as noted below, you can also use the PowerShell command <strong>tcpkg download [package name] -o [output location]</strong>
</div>

### Download from GitHub Releases

1. Navigate to the [GitHub Releases page](https://github.com/Beckhoff-USA-Community/EventVideoPlayback/releases)
2. Find the latest release (or the version you need)
3. Download the package file (`.zip`)
4. Transfer the files to your target system in an easy to remember location (Example: C:\Program Files\Beckhoff USA Community\Feeds\Local)

### Add Local Package Feed via GUI

If you haven't already, add the Beckhoff USA Community package feed to TwinCAT Package Manager:

1. Open the TwinCAT Package Manager GUI
2. Click the Settings (Gear Icon) in the bottom left corner
3. Select Feeds

For the feed settings use:
```bash
C:\Program Files\Beckhoff USA Community\Feeds\Local
```
For the feed name use:
```bash
Beckhoff USA Community Local
```
Deselect the **Set credentials** option, as we do not need login for the feed. Select **Save**.

### Add Package Feed via Powershell

```bash
tcpkg source add -n "Beckhoff USA Community Local" -s "C:\Program Files\Beckhoff USA Community\Feeds\Local"
```

## Install Workloads

After the feed is added (locally or remote), and the VerifySignatures is disabled, you can now install the Workloads and Packages on the feed like you would normal Beckhoff Automation packages.



## Next Steps

- Learn about [PLC Library Usage]({{ '/docs/plc-usage/' | relative_url }}) for advanced features
- Configure [Service Settings]({{ '/docs/service-config/' | relative_url }}) for your environment
- Add [HMI Controls]({{ '/docs/hmi-usage/' | relative_url }}) for video playback

## Troubleshooting

### Service Won't Start

- Verify .NET 8 Runtime is installed
- Check Windows Event Viewer for error messages
- Ensure no other service is using ADS port 26129

### Videos Not Being Created

- Confirm TwinCAT Vision is saving images to the configured path
- Check service configuration file for correct paths
- Verify sufficient disk space is available

### PLC Function Block Errors

- Ensure the library reference is added correctly
- Verify the service is running
- Check ADS communication settings

## Support

Need help? Here are some resources:

- [GitHub Issues](https://github.com/Beckhoff-USA-Community/EventVideoPlayback/issues) - Report bugs or request features
- [Beckhoff USA Community](https://github.com/Beckhoff-USA-Community) - Community support and discussions
- [Documentation]({{ '/' | relative_url }}) - Additional guides and references