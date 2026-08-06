---
layout: doc
title: Installation
description: Installation guide for Event Video Playback packages using TwinCAT Package Manager
permalink: /docs/installation/
---
## Overview

Event Video Playback is a comprehensive solution for transforming TwinCAT Vision images into event-driven video recordings. This guide will walk you through the installation process using TwinCAT Package Manager.

<div class="alert alert-info">
  <strong>Prerequisites:</strong> Before starting, ensure you have TwinCAT Package Manager installed and have the necessary permissions to install packages.
</div>

## System Requirements

Before you begin, ensure you have the following installed on your system:

### For Engineering Development

- **Windows 10/11**
- **TwinCAT Package Manager**
- **TwinCAT 3.1 XAE** - Build 4026 or higher
- **TwinCAT Vision XAE** - Version 5.8.4 or higher
- **TwinCAT HMI Engineering** - Version v14.8.1

### For Runtime Targets

- **Windows 10/11**
- **TwinCAT Package Manager**
- **TwinCAT 3.1 XAR** - Build 4026 or higher
- **TwinCAT Vision XAR** - Version 5.8.4 or higher
- **TwinCAT HMI Server** - Version v14.8.0

### NuGet packages:

- **Beckhoff.TwinCAT.HMI.Controls** - Version 14.3.258
- **Beckhoff.TwinCAT.HMI.EventLogger** - Version 22.0.7066
- **Beckhoff.TwinCAT.HMI.Framework** - Version 14.3.258
- **Beckhoff.TwinCAT.HMI.Functions** - Version 14.3.258
- **Beckhoff.TwinCAT.HMI.Server.Engineering** - Version 22.0.7303
- **Beckhoff.TwinCAT.HMI.Vision** - Version 13.7.6856
- **EventVision** - Version 1.1.3

<div class="alert alert-danger">
<strong>Warning:</strong> Previous versions of the 4024 Tc_EventVideoPlayback legacy project must be uninstalled before using the new 4026 build.
</div>

---

## Package Signing Configuration

TwinCAT Package Manager only accepts officially signed packages from Beckhoff Automation GmbH & Co. KG by default. To use community packages, you need to temporarily disable signature verification.

<div class="alert alert-warning">
  <strong>Security Notice:</strong> Disabling signature verification allows installation of third-party packages. Only use packages from trusted community sources. Review package contents and source code before installation. All community packages are provided "as is" without warranties.
</div>

**To disable signature checks, run this command in PowerShell as Administrator:**

```powershell
tcpkg config unset -n VerifySignatures
```

---

## Installation Methods

Choose the installation method that best fits your environment:

- **[Online Installation](#online-package-feed-connection)** for systems with internet access (recommended, and how you get updates)
- **[Offline Installation](#offline-package-configuration)** for air-gapped systems or when you need a specific version


---

## Offline Package Configuration

**Best for:** Air-gapped systems, manual installations, or when you need a specific version.

### Step 1: Download from GitHub Releases

1. Navigate to the [GitHub Releases page](https://github.com/Beckhoff-USA-Community/EventVideoPlayback/releases)
2. Find the latest release (or the version you need)
3. Download the package file (`.zip`)
4. Transfer the files to your target system in a convenient location
   - **Example:** `C:\Program Files\Beckhoff USA Community\Feeds\Local`


### Step 2: Add Local Package Feed

#### Option 1: Add via GUI

1. Open the **TwinCAT Package Manager GUI**
2. Click the **Settings** (Gear Icon) in the bottom left corner
3. Select **Feeds**
4. Add a new feed with the following settings:

   - **Feed Path:** `C:\Program Files\Beckhoff USA Community\Feeds\Local`
   - **Feed Name:** `Beckhoff USA Community Local`
   - **Set credentials:** Deselect (not required)

5. Click **Save**

#### Option 2: Add via PowerShell

Run the following command in PowerShell:

```powershell
tcpkg source add -n "Beckhoff USA Community Local" -s "C:\Program Files\Beckhoff USA Community\Feeds\Local"
```

---

## Online Package Feed Connection

**Best for:** Systems with internet connectivity.

The packages are published to the GitHub package feed of the Beckhoff USA Community organization.

<div class="alert alert-warning">
  <strong>Feed change:</strong> The old community feed at packages.beckhoff-usa-community.com has been retired. If you added it previously, remove it with <code>tcpkg source remove -n "Beckhoff USA Community Stable"</code> and add the feed below instead.
</div>

### Step 1: Create a GitHub personal access token

GitHub requires a login for its package feeds, even for public packages, so you need a GitHub account and a personal access token. In your GitHub account settings, under Developer settings, create a token of the **classic** type with the **read:packages** scope.

<div class="alert alert-info">
  <strong>Step by step:</strong> <a href="https://github.com/Beckhoff-USA-Community/BAUS_Community_General_Assets/blob/main/feed-setup.md" target="_blank">Adding the package feed</a> walks through creating the token click by click, and covers the common failures. It applies to every Beckhoff USA Community package, not just this project.
</div>

### Step 2: Add the feed

Run both commands from an administrator PowerShell, in this order:

```powershell
tcpkg config unset -n VerifySignatures
tcpkg source add -n "Beckhoff-USA-Community" -s https://nuget.pkg.github.com/Beckhoff-USA-Community/index.json -u <your GitHub user name> --take 100
```

Enter the token as the password when prompted, then accept the community disclaimer.

Both steps are required, and the order matters:

- Community packages carry no Beckhoff signature, including the disclaimer package the feed presents while it is being added, so verification must be off before the feed is added.
- GitHub rejects search requests larger than 100 results per page while the package manager asks for 500 by default. Without `--take 100` the command fails with "Failed to retrieve metadata from source".

If adding the feed fails, confirm the token is the classic type (it starts with `ghp_`), that it has the read:packages scope, and that it has not expired.

---

## Install Workloads

After completing the steps above (adding the package feed and disabling signature verification), you can now install the Event Video Playback workloads and packages through TwinCAT Package Manager, just like any other Beckhoff Automation package.

1. Open **TwinCAT Package Manager**
2. Browse the available packages from the Beckhoff USA Community feed
3. Select **Event Video Playback** workloads/packages
4. Click **Install**

<div class="alert alert-info">
  <strong>Note:</strong> The installation will include the necessary XAE/XAR components, PLC libraries, HMI packages, and the Windows service.
</div>


---

## Next Steps

After installation is complete, proceed to the [Getting Started]({{ '/docs/getting-started/' | relative_url }}) guide to learn how to configure and use Event Video Playback in your projects.

---

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
