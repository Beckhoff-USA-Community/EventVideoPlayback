# TwinCAT Package Checksums

**Generated:** 2025-11-18 14:35:28
**Release Folder:** C:\GitHub\EventVideoPlayback\tcpkg\.\release

This file contains SHA256 checksums for all built packages to ensure integrity and traceability.

## Package Checksums

| Package Name | Version | SHA256 Checksum |
|--------------|---------|-----------------|
| Beckhoff-USA-Community.EventVideoPlayback.XAE | 2.0.3 | `A11D6FD7200D4348927156F6696F167FBB754801CB6B9AA03FCFDB0F199CE700` |
| Beckhoff-USA-Community.EventVideoPlayback.XAR | 2.0.3 | `51C9C7C163452E7BB8D83D744874319E33734BC5F3764897E49E36B05D1D17F7` |
| Beckhoff-USA-Community.XAE.Documentation.EventVideoPlayback | 2.0.3 | `6452850BBB777EA92735790EF37B50420C0A18F4E1A0F3C914F528A8D73389E4` |
| Beckhoff-USA-Community.XAE.HMI.EventVisionControl | 1.1.3 | `3BD210416439B9AA7E39D15981F2EF3546E51FDF49B758ED4FFF16007C9E9F43` |
| Beckhoff-USA-Community.XAE.PLC.Lib.EventVideoPlayback | 1.1.1 | `192E7A6C33FEB397D2BB8BB6E36BB72B8AD212A40D8EB4F135E53E356C5D45A7` |
| Beckhoff-USA-Community.XAR.Service.EventVideoPlayback | 2.0.0 | `447A0012C5E2C83ABAC3EF37C346FCBED0C8CD294B67C03D996FED11C24EB4AC` |

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
