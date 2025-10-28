# TwinCAT Package Checksums

**Generated:** 2025-10-28 05:35:47
**Release Folder:** C:\GitHub\EventVideoPlayback\tcpkg\.\release

This file contains SHA256 checksums for all built packages to ensure integrity and traceability.

## Package Checksums

| Package Name | Version | SHA256 Checksum |
|--------------|---------|-----------------|
| Beckhoff-USA-Community.XAE.Documentation.EventVideoPlayback | 2.0.0 | `4B083128B51225CA05F6768E8DD5D55CA79F3B5CC858D842BB4B983D139FE6AB` |
| Beckhoff-USA-Community.XAE.EventVideoPlayback | 2.0.0 | `ACB3F0FCD9F7F7FEAC861D17BE0B8CD451CF86CE144B51CF0C8C5AABF5F96809` |
| Beckhoff-USA-Community.XAE.HMI.EventVisionControl | 1.1.3 | `3B6F1C5DBB973072EDBB095E61860570BF254AD703AA9B032EA11524C5421FE1` |
| Beckhoff-USA-Community.XAE.PLC.Lib.EventVideoPlayback | 1.1.1 | `C8B89C7F0EAE5C3FA0883F3B1AFAC54A83516D31D010CA408A60CB66BD187785` |
| Beckhoff-USA-Community.XAR.EventVideoPlayback | 2.0.0 | `5088CF0766D506234D01598C71FD996B373A1EBF31EBA0DC0820191071A9AF7E` |
| Beckhoff-USA-Community.XAR.Service.EventVideoPlayback | 2.0.0 | `BAC93046D063F2917E253BDD4E4EAF5E5CDB3C97A0D8A69A14BEA21DF470D93C` |

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
