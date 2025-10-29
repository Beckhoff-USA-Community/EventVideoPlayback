# EventVideoPlaybackService - Cross-Platform Quick Reference

## Quick Start

### Build on Windows
```bash
cd C:\GitHub\EventVideoPlayback\src\App.Service\EventVideoPlaybackService
dotnet build
```

### Run as Console App (Testing)
```bash
dotnet run --project EventVideoPlaybackService.csproj
```

### Publish for Windows
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### Publish for Linux
```bash
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
```

## What Changed

1. **Removed hardcoded RuntimeIdentifier** - Now builds for any platform
2. **Added Linux support** - Includes systemd service integration
3. **Platform-specific packages** - Conditionally includes Windows/Linux OpenCV runtimes
4. **Dual-mode operation** - Works as service or console app

## File Changes

- `EventVideoPlaybackService.csproj` - Removed RuntimeIdentifier, added conditional packages
- `Program.cs` - Added AddSystemd() for Linux support
- `CodecLibs.projitems` - Conditional codec library inclusion

## Next Steps for Linux Deployment

1. Obtain Linux H.264 codec: `libopenh264.so`
2. Place in: `CodecLibs\libopenh264.so`
3. Publish for linux-x64
4. Copy to Debian system
5. Create systemd service unit
6. Test and deploy

See `CROSS_PLATFORM_DEPLOYMENT.md` for detailed instructions.
