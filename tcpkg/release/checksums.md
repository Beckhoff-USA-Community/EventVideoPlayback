# TwinCAT Package Checksums

**Generated:** 2025-11-11 09:30:01
**Release Folder:** C:\GitHub\EventVideoPlayback\tcpkg\.\release

This file contains SHA256 checksums for all built packages to ensure integrity and traceability.

## Package Checksums

| Package Name | Version | SHA256 Checksum |
|--------------|---------|-----------------|
| Beckhoff-USA-Community.EventVideoPlayback.XAE | 2.0.0 | `E3D29512A7F60977E2038F2F97777E913339E81AEB72FFC17769C208C317B2DB` |
| Beckhoff-USA-Community.EventVideoPlayback.XAE | 2.0.1 | `C66B5E77507B593D944217F3D9D1B30E148B6EA097F8ACDF73B24061581B831E` |
| Beckhoff-USA-Community.EventVideoPlayback.XAR | 2.0.0 | `B2D44674BB4F540BA8F9614D76DCC02D6793D8729F278B4828326202474A77FA` |
| Beckhoff-USA-Community.XAE.Documentation.EventVideoPlayback | 2.0.0 | `64131561E0EC94E69DF52FCBBF564E003E906561D75A69E640151EF5098A4EF9` |
| Beckhoff-USA-Community.XAE.Documentation.EventVideoPlayback | 2.0.1 | `603A47E4BBCBB64E24166D8C7412157EF538C95B640DCF5988FAB0E337D25EB4` |
| Beckhoff-USA-Community.XAE.HMI.EventVisionControl | 1.1.3 | `3BD210416439B9AA7E39D15981F2EF3546E51FDF49B758ED4FFF16007C9E9F43` |
| Beckhoff-USA-Community.XAE.PLC.Lib.EventVideoPlayback | 1.1.1 | `192E7A6C33FEB397D2BB8BB6E36BB72B8AD212A40D8EB4F135E53E356C5D45A7` |
| Beckhoff-USA-Community.XAR.Service.EventVideoPlayback | 2.0.0 | `447A0012C5E2C83ABAC3EF37C346FCBED0C8CD294B67C03D996FED11C24EB4AC` |
| Beckhoff.Disclaimer.BetaFeed | 1.0.0 | `401A48CD5C15F0F49F9C373C4AF8CC28E970DC720B8D616C03CF8C98D7262979` |

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
