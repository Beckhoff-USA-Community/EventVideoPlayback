# TwinCAT Package Checksums

**Generated:** 2025-10-28 12:54:36
**Release Folder:** C:\GitHub\EventVideoPlayback\tcpkg\.\release

This file contains SHA256 checksums for all built packages to ensure integrity and traceability.

## Package Checksums

| Package Name | Version | SHA256 Checksum |
|--------------|---------|-----------------|
| Beckhoff-USA-Community.XAE.Documentation.EventVideoPlayback | 2.0.0 | `B3613FC56E6EBB407D520CA0B7A0E0E696CCEDA9031EDD1431C1A2B200C20974` |
| Beckhoff-USA-Community.XAE.EventVideoPlayback | 2.0.0 | `1519ED7182A18467741835B31481847728D174802082AFF77CCBD712671A0937` |
| Beckhoff-USA-Community.XAE.HMI.EventVisionControl | 1.1.3 | `CAA7EC75E838F87034E4C77E198140027B759751FE3AFF25111DA2717E6FC77B` |
| Beckhoff-USA-Community.XAE.PLC.Lib.EventVideoPlayback | 1.1.1 | `06B94CD1D5CA2D5459340EC299393DDE4CB9BB443D43675ADAC18FB0001870BE` |
| Beckhoff-USA-Community.XAR.EventVideoPlayback | 2.0.0 | `875E4F37FE1D15F39DE1EBFCDB62249956E9D997000D7BC42A3CA9C6C057F355` |
| Beckhoff-USA-Community.XAR.Service.EventVideoPlayback | 2.0.0 | `09315B42D8E298669A1542FE81C1C090B3B33FB731308C2B05F4401DE2C48B79` |

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
