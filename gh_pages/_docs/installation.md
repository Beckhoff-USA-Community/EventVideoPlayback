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
- **TwinCAT HMI XAE** - Version 14.9 or higher

### For Runtime Targets

- **Windows 10/11**
- **TwinCAT Package Manager**
- **TwinCAT 3.1 XAR** - Build 4026 or higher
- **TwinCAT Vision XAR** - Version 5.8.4 or higher

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

- **[Offline Installation](#offline-package-configuration)** - For air-gapped systems or manual installation (recommended)
- **[Online Installation](#online-package-feed-connection)** - For systems with internet access


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

## Online Package Feed Connection (Optional - Beta)

**Best for:** Systems with internet connectivity and access to the Beckhoff USA Community package feed.

### Option 1: Add Package Feed via GUI

Add the Beckhoff USA Community package feed to TwinCAT Package Manager:

1. Open the **TwinCAT Package Manager GUI**
2. Click the **Settings** (Gear Icon) in the bottom left corner
3. Select **Feeds**
4. Add a new feed with the following settings:

   - **Feed URL:** `https://packages.beckhoff-usa-community.com/stable/v3/index.json`
   - **Feed Name:** `Beckhoff USA Community Stable`
   - **Set credentials:** Deselect (not required)

5. Click **Save** and agree to the disclaimer

### Option 2: Add Package Feed via PowerShell

Run the following command in PowerShell:

```powershell
tcpkg source add -n "Beckhoff USA Community Stable" -s "https://packages.beckhoff-usa-community.com/stable/v3/index.json"
```

Agree to the disclaimer when prompted.

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
