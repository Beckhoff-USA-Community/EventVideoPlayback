# EventVision.HMI Package

## Package Type
HMI NuGet Package Installer

## Prerequisites
Before building this package, ensure:
1. The HMI NuGet package exists at: `C:\GitHub\EventVideoPlayback\artifacts\hmi\EventVision.1.1.3.nupkg`
2. You have an icon file (icon.png) in this directory

## Building the Package

### Step 1: Copy the HMI NuGet Package
```bash
copy "C:\GitHub\EventVideoPlayback\artifacts\hmi\EventVision.1.1.3.nupkg" "C:\GitHub\EventVideoPlayback\tcpkg\EventVision.HMI\EventVision.1.1.3.nupkg"
```

### Step 2: Add an Icon (Optional)
Place an icon.png file in this directory, or copy from the reference:
```bash
copy "C:\Agent_Resources\V3\NewPackageTest\NewPackageTest\TF1000-Base.png" "C:\GitHub\EventVideoPlayback\tcpkg\EventVision.HMI\icon.png"
```

### Step 3: Pack the Package
Using relative path:
```bash
tcpkg pack EventVision.HMI.nuspec -o ../packages
```

Or using the full path:
```bash
tcpkg pack "C:\GitHub\EventVideoPlayback\tcpkg\packages\EventVision.HMI\EventVision.HMI.nuspec" -o "C:\GitHub\EventVideoPlayback\tcpkg\packages"
```

## Installation
Once packed, install the package with:
```bash
tcpkg install EventVision.HMI
```

This will:
- Copy the HMI NuGet package to `C:\ProgramData\Beckhoff\NuGetPackages\`
- Make it available for TwinCAT HMI projects
- Allow the HMI to reference the EventVision components

## Uninstallation
```bash
tcpkg uninstall EventVision.HMI
```

This will remove the NuGet package from the Beckhoff NuGet packages directory.

## Customization
Before building, update the `.nuspec` file with:
- Your company name in `<authors>`
- Your project URL in `<projectUrl>`
- Your copyright information
- The correct version number

## Dependencies
This package has the following dependencies:
- TE2000.HMIEngineering.XAE - TwinCAT HMI Engineering environment

This dependency will be automatically installed by tcpkg if not already present.
