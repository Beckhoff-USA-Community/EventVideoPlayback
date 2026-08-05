# EventVision.HMI Package

## Package Type
HMI NuGet Package Installer

## Prerequisites
Before building this package, ensure the HMI NuGet package built from
`src\HMI\TcHmiEventVideo` exists. The version number comes from
`EventVisionControl\Description.json` in that project.

## Building the Package

### Step 1: Stage the HMI NuGet Package
Copy the built nupkg into the `bin` folder of this package. The install script
globs for `EventVision.*.nupkg`, so no file needs to be renamed when the
version changes:

```powershell
Copy-Item "..\..\..\artifacts\hmi\EventVision.*.nupkg" ".\bin\"
```

### Step 2: Pack the Package
Using relative path:

```powershell
tcpkg pack EventVision.HMI.nuspec -o ../packages
```

Or run the repository wide dress rehearsal from the repo root:

```powershell
.\tcpkg\Build.ps1
```

## Installation
Once packed, install the package with:

```powershell
tcpkg install Beckhoff-USA-Community.XAE.HMI.EventVisionControl
```

This will:

* Copy the HMI NuGet package to `C:\ProgramData\Beckhoff\NuGetPackages\`
* Make it available for TwinCAT HMI projects
* Allow the HMI to reference the EventVision components

## Uninstallation

```powershell
tcpkg uninstall Beckhoff-USA-Community.XAE.HMI.EventVisionControl
```

This will remove the NuGet package from the Beckhoff NuGet packages directory.

## Versioning
The control version is defined once, in
`src\HMI\TcHmiEventVideo\TcEventVideoPlayback-TcHMIControl\EventVisionControl\Description.json`.
The nuspec in the HMI project and the nuspec in this folder must carry the same
version. Run `tcpkg\scripts\Sync-HmiVersion.ps1` after changing the version to
restamp both nuspecs. The install scripts glob the nupkg file name, so they
never need a version edit.

## Dependencies
This package has the following dependencies:

* TE2000.HMIEngineering.XAE, the TwinCAT HMI Engineering environment

This dependency will be automatically installed by tcpkg if not already present.
