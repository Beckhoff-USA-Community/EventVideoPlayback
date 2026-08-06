# TwinCAT Package Checksums

**Generated:** 2026-08-06 13:12:29
**Release Folder:** C:\GitHub\EventVideoPlayback\packaging\.\release

This file contains SHA256 checksums for all built packages to ensure integrity and traceability.

## Package Checksums

| Package Name | Version | SHA256 Checksum |
|--------------|---------|-----------------|
| Beckhoff-USA-Community.EventVideoPlayback.XAE | 2.0.3 | `5CB6EE438CD4E35B79F1FB6BECC9B6F7F2439198E3D8EFD851888FC3B4CB1766` |
| Beckhoff-USA-Community.EventVideoPlayback.XAR | 2.0.3 | `10D9AA08361457AF9E5FFDB9226EF715C2F2A7FEF153507C215F62E7147E6B49` |
| Beckhoff-USA-Community.XAE.PLC.Lib.EventVideoPlayback | 1.1.1 | `08B301A50E3917DFE63EB69D623943F65AB5926547D6B079B96771D90984CAC7` |

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
