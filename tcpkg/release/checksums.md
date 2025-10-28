# TwinCAT Package Checksums

**Generated:** 2025-10-28 11:36:01
**Release Folder:** C:\GitHub\EventVideoPlayback\tcpkg\.\release

This file contains SHA256 checksums for all built packages to ensure integrity and traceability.

## Package Checksums

| Package Name | Version | SHA256 Checksum |
|--------------|---------|-----------------|
| Beckhoff-USA-Community.XAE.Documentation.EventVideoPlayback | 2.0.0 | `BE83185963E3AAF77F99D4C310235C594956E407427647C6E4FE495BCC520654` |
| Beckhoff-USA-Community.XAE.EventVideoPlayback | 2.0.0 | `A3EB879860F6C23B74A7573ED034DE6200AFD68338BF651FD70A73EF891805C8` |
| Beckhoff-USA-Community.XAE.HMI.EventVisionControl | 1.1.3 | `8A6ACF8D1854A8F8A6B3992C3A15799872D82707DC2E756044FB174F1A499E1D` |
| Beckhoff-USA-Community.XAE.PLC.Lib.EventVideoPlayback | 1.1.1 | `B2B6E4A95E2FE67F190B92C418174FA19DB2D626A4D24AB52E1C034F3B6CC3B3` |
| Beckhoff-USA-Community.XAR.EventVideoPlayback | 2.0.0 | `6E83B796F621EB8F4E05B2C129D1A7155B7D454B6F6E1F4AD4CA2CA6F86676A7` |
| Beckhoff-USA-Community.XAR.Service.EventVideoPlayback | 2.0.0 | `8B198DB77D2A80D8559CC28D1880183AB5EE5919AEDA4D9202BC41A6EBEBFFDA` |

## Verification Instructions

To verify a package's integrity, use PowerShell:

```powershell
Get-FileHash -Path "path\to\package.nupkg" -Algorithm SHA256
```

Compare the output checksum with the corresponding checksum in the table above.

## Notes

- All checksums are calculated using SHA256 algorithm
- Checksums are regenerated on each build
- Any modification to the package will result in a different checksum
- Keep this file for audit and compliance purposes
