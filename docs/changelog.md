# Changelog

All components ship together as a release train. The train number is the version of the XAE and XAR workload packages. The PLC library, the HMI control, and the service each carry their own version and are listed when they change.

## 2.0.3 (2025-11-18)

### Changed

* Documentation site updates and rebuilt documentation PDFs
* Removed the beta feed disclaimer package from both workloads

No component changes. The train ships PLC library 1.1.1, HMI control 1.1.3, and service 2.0.0.

## 2.0.2 (2025-11-17)

### Changed

* Documentation package consolidated to three PDF documents
* XAR workload version pulled forward to match the XAE workload
* README corrections

No component changes.

## 2.0.1 (2025-11-11)

### Changed

* Documentation package rebuilt as PDF documents in place of markdown pages

Published for the XAE workload and the documentation package only; the XAR workload skipped this version. No component changes.

## 2.0.0 (2025-10-24)

Version 2.0.0 requires TwinCAT 4026 or newer. TwinCAT 4024 and older are not supported, and the legacy 4024 Tc_EventVideoPlayback project must be uninstalled before this version is installed.

The train ships PLC library 1.1.1, HMI control 1.1.3, and service 2.0.0.

### Added

* EventVideoPlayback PLC library for TwinCAT Vision integration
* EventVision HMI control that plays event videos from the event grid
* EventVideoPlayback Windows service that assembles captured images into videos
* TwinCAT Package Manager packaging for every component
* Documentation site and sample PLC and HMI projects
* OpenH264 codec integration for H.264 video encoding

### Fixed

* Service crash from infinite recursion when receiving confirmations from the PLC
* Race conditions and data corruption in notification management under concurrent operations
* PLC communication timeouts during long encodes; video creation runs in background tasks and ADS responses return immediately
* Error handling for file and video encoding operations, so disk errors, corrupt images, and codec issues no longer crash the service
* Crash when creating a video from a directory with no images
* Resource leaks: the ADS server, the video writer, and OpenCV image objects are now released reliably
