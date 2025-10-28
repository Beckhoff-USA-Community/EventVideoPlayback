# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed
- Repository prepared for public release
- Enhanced documentation with build instructions
- Added comprehensive .gitignore for TwinCAT/.NET projects

## [2.0.0] - 2025-10-24

### Breaking Changes
- **TwinCAT 4026+ Required**: This version only supports TwinCAT 4026 and newer. TwinCAT 4024 and older versions are no longer supported.

### Added
- EventVideoPlayback PLC library for TwinCAT Vision integration
- EventVision HMI Control for TwinCAT HMI video playback
- EventVideoPlaybackService Windows service for video assembly
- TwinCAT Package Manager build system (tcpkg)
- Comprehensive documentation and GitHub Pages site
- Sample PLC and HMI projects
- OpenH264 codec integration for H.264 video encoding
- Event Logger integration for alarm correlation

### Features
- Automatic video assembly from captured images
- Event-based video logging with timestamp correlation
- HMI playback control with seeking and timeline
- Configurable video encoding parameters
- Multiple video format support

### Fixed
- **Fixed service crashes during PLC notification handling**: Resolved infinite recursion bug that caused immediate stack overflow crashes when receiving confirmations from the PLC.
- **Resolved race conditions and data corruption**: Replaced non-thread-safe notification management with concurrent-safe implementation, eliminating crashes and data corruption under concurrent operations.
- **Fixed PLC communication timeouts**: Video creation now runs in background tasks, preventing ADS communication blocking and PLC timeouts during long video encoding operations.
- **Added comprehensive error handling**: All file I/O and video encoding operations now properly handle exceptions, preventing service crashes from disk errors, corrupt images, or codec issues.
- **Fixed crash when no images available**: Added validation to prevent crashes when attempting to create videos from empty directories.
- **Fixed memory and resource leaks**:
  - ADS server now properly disposed on shutdown
  - Video writer resources properly released even on errors
  - OpenCV Mat objects properly released during image processing

### Performance
- ADS responses now return immediately instead of waiting for video completion
- Service can now handle concurrent video creation requests
- Reduced memory usage through proper resource cleanup
- Improved thread pool efficiency with proper async operations

## Version History

For release notes of previous versions, see the [Releases page](https://github.com/Beckhoff-USA-Community/EventVideoPlayback/releases).
