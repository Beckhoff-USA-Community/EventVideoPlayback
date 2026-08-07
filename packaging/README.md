# TwinCAT Package Manager - EventVideoPlayback

This directory contains the automated build system for all TwinCAT packages in the EventVideoPlayback project.

## Directory Structure

```
packaging/
├── packages/              # Package source directories
│   ├── EventVideoPlayback.Library/
│   ├── EventVideoPlayback.Service/
│   ├── EventVision.HMI/
│   ├── EventVideoPlayback.XAE/
│   ├── EventVideoPlayback.XAR/
│   └── EventVideoPlayback.Documentation/
├── release/               # Built .nupkg files and checksums.md
├── scripts/               # Build scripts and documentation
│   ├── Build-TcPackages.ps1
│   ├── Build-All-Packages.bat
│   ├── BUILD_INSTRUCTIONS.md
│   └── QUICK_REFERENCE.txt
├── Build.ps1              # Convenience wrapper (run from here)
└── Build.bat              # Convenience wrapper (double-click)
```

## Quick Start

### Build TwinCAT Packages

#### Option 1: PowerShell (Recommended)
```powershell
.\Build.ps1 -CleanBuild
```

#### Option 2: Batch File
Double-click `Build.bat`

#### Option 3: From Scripts Directory
```powershell
cd scripts
.\Build-TcPackages.ps1 -CleanBuild
```

### Stage the documentation payload

The CI build stages the documentation package payload from the repository docs before packing. To reproduce that locally for a dress rehearsal:

```powershell
Copy-Item ..\README.md packages\EventVideoPlayback.Documentation\bin\README.md
Copy-Item ..\docs\*.md packages\EventVideoPlayback.Documentation\bin\
Copy-Item -Recurse ..\docs\assets packages\EventVideoPlayback.Documentation\bin\assets
```

## Adding New Packages

To add a new package to the build:

1. Create a new directory under `packages/`
2. Add your `.nuspec` file and source files
3. Run the build script. It will automatically discover and build your package.

**Example:**
```
packages/
└── MyNewPackage/
    ├── MyNewPackage.nuspec
    ├── bin/              # Files to install (optional)
    └── tools/            # Chocolatey scripts (optional)
```

## Output

All built packages are output to:
- **Location**: `release/`
- **Files**: All `.nupkg` files
- **Checksums**: `release/checksums.md` with SHA256 hashes

## Package Verification

Packages that contain a `bin/` directory automatically get a `VERIFICATION.md` file in their `tools/` directory with SHA256 checksums of all installation files.

## Documentation

For complete documentation, see:
- **BUILD_INSTRUCTIONS.md** - Comprehensive build guide
- **QUICK_REFERENCE.txt** - Quick command reference

## Requirements

### For Building TwinCAT Packages
- TwinCAT tcpkg tool (installed with TwinCAT XAE)
- PowerShell 5.1 or higher
- Windows 10/11

### For PDF Generation (Optional)
- Python 3.6+
- Python packages (automatically installed by script):
  - `markdown` - Markdown parser
  - `xhtml2pdf` - HTML to PDF converter (pure Python, no external dependencies)

## Support

For issues or questions, review the documentation in the `scripts/` directory or check the console output for specific error messages.
