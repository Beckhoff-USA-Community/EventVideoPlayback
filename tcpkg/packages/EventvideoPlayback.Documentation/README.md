# EventVideoPlayback Documentation Package

This package contains comprehensive documentation for the EventVideoPlayback system.

## Package Information

- **Package ID:** `Beckhoff-USA-Community.XAE.Documentation.EventVideoPlayback`
- **Version:** 1.0.0
- **Authors:** Beckhoff Automation LLC
- **License:** Beckhoff Software License Agreement

## Contents

This documentation package includes:

- **Introduction.md** - Overview of the EventVideoPlayback system
- **Installation.md** - Installation instructions and setup guide
- **SystemRequirements.md** - System requirements and prerequisites
- **FirstProgram.md** - Getting started guide for creating your first program
- **AddToExist.md** - Guide for adding EventVideoPlayback to existing projects
- **ServiceDoc.md** - EventVideoPlayback service documentation
- **Licenses.md** - License information and attributions

## Installation

### Using TwinCAT Package Manager (tcpkg)

```powershell
tcpkg install Beckhoff-USA-Community.XAE.Documentation.EventVideoPlayback
```

### Manual Installation from Local Package

```powershell
tcpkg install .\EventVideoPlayback.Documentation.1.0.0.nupkg
```

## Installation Location

The documentation will be installed to:
```
C:\Program Files\Beckhoff USA Community\EventVideoPlayback\Documentation\
```

This location centralizes all EventVideoPlayback components under the Beckhoff USA Community directory.

## Building the Package

To build this package from source:

### Step 1: Navigate to Package Directory (Optional)
```powershell
cd C:\GitHub\EventVideoPlayback\tcpkg\packages\EventvideoPlayback.Documentation
```

### Step 2: Pack the Package
Using relative path:
```powershell
tcpkg pack EventVideoPlayback.Documentation.nuspec -o ../packages
```

Or using the full path:
```powershell
tcpkg pack "C:\GitHub\EventVideoPlayback\tcpkg\packages\EventvideoPlayback.Documentation\EventVideoPlayback.Documentation.nuspec" -o "C:\GitHub\EventVideoPlayback\tcpkg\packages"
```

This will create the package in the packages directory.

## Package Structure

```
EventvideoPlayback.Documentation/
├── EventVideoPlayback.Documentation.nuspec  # Package manifest
├── TF1000-Base.png                          # Package icon
├── README.md                                # This file
├── docs/                                    # Documentation source files
│   ├── Introduction.md
│   ├── Installation.md
│   ├── SystemRequirements.md
│   ├── FirstProgram.md
│   ├── AddToExist.md
│   ├── ServiceDoc.md
│   └── Licenses.md
└── tools/                                   # Installation scripts
    ├── chocolateyinstall.ps1
    ├── chocolateyuninstall.ps1
    ├── chocolateybeforemodify.ps1
    ├── LICENSE.txt
    └── VERIFICATION.txt
```

## Uninstallation

To remove the documentation:

```powershell
tcpkg uninstall Beckhoff-USA-Community.XAE.Documentation.EventVideoPlayback
```

This will:
- Remove the Documentation directory
- Remove the EventVideoPlayback parent directory if no other components remain installed
- Preserve the parent directory if other components (like Service) are still installed

## Related Packages

This documentation package is part of the EventVideoPlayback system. Related packages include:

- **Beckhoff-USA-Community.XAE.PLC.Lib.EventVideoPlayback** - PLC Library
- **Beckhoff-USA-Community.XAE.HMI.EventVisionControl** - HMI Control
- **Beckhoff-USA-Community.XAR.Service.EventVideoPlayback** - Background Service

## Support

For issues, questions, or contributions, please visit:
https://github.com/Beckhoff-USA-Community/EventVideoPlayback

## License

Copyright (c) Beckhoff Automation LLC

This package is licensed under the Beckhoff Software License Agreement.
Full license: https://www.beckhoff.com/media/downloads/general-terms-and-conditions/software_license_agreement_for_beckhoff_software_products.pdf/
