# TwinCAT Package Checksums

**Generated:** 2025-11-10 08:39:04
**Release Folder:** C:\GitHub\EventVideoPlayback\tcpkg\.\release

This file contains SHA256 checksums for all built packages to ensure integrity and traceability.

## Package Checksums

| Package Name | Version | SHA256 Checksum |
|--------------|---------|-----------------|
| Beckhoff-USA-Community.EventVideoPlayback.XAE | 2.0.0 | `E3D29512A7F60977E2038F2F97777E913339E81AEB72FFC17769C208C317B2DB` |
| Beckhoff-USA-Community.EventVideoPlayback.XAR | 2.0.0 | `301C5BD8BCB1805E71776B5C332ADF707F2EE58053A0C5492FD961BA2625C286` |
| Beckhoff-USA-Community.XAE.Documentation.EventVideoPlayback | 2.0.0 | `64131561E0EC94E69DF52FCBBF564E003E906561D75A69E640151EF5098A4EF9` |
| Beckhoff-USA-Community.XAE.HMI.EventVisionControl | 1.1.3 | `358440528A73FB627340766FE11300F00E598FFA52EE459851EAB5A32836368B` |
| Beckhoff-USA-Community.XAE.PLC.Lib.EventVideoPlayback | 1.1.1 | `24BFE0AED289F0CF80294C0EE43064ACB040C4B44D10C4D1E80C1BC01DBBE470` |
| Beckhoff-USA-Community.XAR.Service.EventVideoPlayback | 2.0.0 | `97296EF0B207DEFFA13625DDF8C147AC42A19FF9CC0C8F6014F9368F896876D5` |
| Beckhoff.Disclaimer.BetaFeed | 1.0.0 | `45991EBCC3FD10760C73A406C33EC63B2D3ECF9E50B2C8108465D6D9DC09793D` |

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
