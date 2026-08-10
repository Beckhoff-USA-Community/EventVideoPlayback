# Changelog

All components ship together as a release train. The train number is the version of the XAE and XAR workload packages. The PLC library, the HMI control, and the service each carry their own version and are listed when they change.

## 2.1.1 (2026-08-10)

The train ships PLC library 1.2.0, HMI control EventVision 1.2.1, and service 2.1.0.

### Fixed

* The EventVision control package published with 2.1.0 was missing its payload: the release pipeline's pack step silently dropped the inner control file, so the package failed to install. The control itself is unchanged; its package version moves to 1.2.1 and the pipeline now verifies the payload before anything publishes. The broken 1.2.0 control package and the 2.1.0 XAE workload are removed from the feed; install the 2.1.1 workload instead.

## 2.1.0 (2026-08-10)

The train ships PLC library 1.2.0, HMI control EventVision 1.2.0, and service 2.1.0. The packages are published on the GitHub Packages feed; the old community feed is retired.

Version 2.1.0 targets the current TE2000 line. EventVision 1.2.0 requires HMI Framework 14.5.1, Controls 14.7.1, and EventLogger 23.0.275 or newer; it does not work on the 14.3 package line, and 1.1.x does not work on the 14.6 line or newer. Version 14.16 is the tested Engineering and Server line. On current servers video playback also needs one server setting; see [Content Security Policy](./hmi-control.md#content-security-policy).

### Added

* `VideoRootDirectory` service setting that confines every path the PLC commands to one directory tree (off by default, see [Service Configuration](./service-configuration.md))
* The service logs its effective configuration to the Application event log at startup
* The video button, its states, and the dialog title are localized in English and German
* Documentation site rebuilt with reference pages for the [PLC library](./plc-library.md), the [HMI control](./hmi-control.md), and the [service configuration](./service-configuration.md)

### Changed

* The HMI control is rebuilt for the current TE2000 packages: new details popup integration, time zone conversion with daylight saving handling, and a capped file poll with clear button states
* The service runs on .NET 10 and ships self contained
* The service reads its configuration file from the install directory. Earlier versions resolved it against the process working directory, which for a Windows service meant the shipped configuration was silently ignored in the field
* One default set for all service configuration fallbacks. The 2.0.x releases carried three divergent fallback sets
* The project builds from a clean clone; the event class definition ships in the repository and is binary compatible with the fielded 1.1.1 library

### Fixed

* PLC: a failing image write gives up after a bounded number of attempts, latches an error code, and recovers through `Reset`. Before, a full disk froze capture silently with no alarm and no recovery
* PLC: the event timestamp is copied when the alarm fires, so a second event during a capture no longer changes the video filename mid capture
* PLC: `ErrorID` carries the real ADS error code instead of a fixed sentinel value, and `FramesPerSecond` and `ReductionFactor` validate their values
* Service: video frames are sorted before encoding, so videos play in capture order on every file system
* Service: the encode queue is bounded and a full queue answers the PLC with a busy error instead of accepting work it cannot finish
* Service: a failed encode deletes the partial video file and keeps the captured images
* Service: malformed command payloads are answered with an error instead of running the PLC into a timeout
* Service: a failed ADS connection at startup stops the service so the service manager restarts it, instead of leaving a running service that serves nothing
* HMI: the event grid row icons ship with the control, so they render on the current Controls packages

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
