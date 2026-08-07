# Service Configuration

The EventVideoPlayback service is a Windows service that encodes the images the PLC library saves to disk into an MP4 video, on command from the PLC over ADS (Automation Device Specification, the protocol TwinCAT components use to talk to each other). It also keeps the video folders in check by deleting old videos based on age and total folder size.

## Configuration file

The service reads its settings from one JSON file in the service install directory:

```text
C:\Program Files\Beckhoff USA Community\EventVideoPlayback\Service\EventVideoPlaybackService.config.json
```

Open it with Notepad or any editor. The shipped file looks like this:

```json
{
  "CodecFourCC": "avc1",
  "VideoDeleteTime": 1,
  "AdsPort": 26129,
  "MaxFolderSize": 250,
  "VideoRootDirectory": ""
}
```

!!! note
    Since the 2.1.0 release the configuration file is read from the service install directory. Earlier versions resolved it against the process working directory, which for a Windows service is the System32 directory, so field installs silently ran on default values. If you worked around that on a 2.0.x install, move your changes into the file above after upgrading.

The file is read once at startup. Restart the service to apply changes:

1. Open the Windows Services window (run `services.msc`).
2. Right click the **EventVideoPlayback** service.
3. Choose **Stop**, then **Start**. A reboot also works.

## Settings

### CodecFourCC

* Type: string
* Default: `avc1`

The FourCC code (a four character identifier) of the video codec the service uses for encoding. The videos play in a web browser through the HMI control, so the codec must be one that browsers can decode. A codec the browser cannot decode still produces a file, but it plays as a black box in the HMI. Stick with these options:

| Codec | FourCC | Description | Web Compatible | Recommended |
|-------|--------|-------------|----------------|-------------|
| H.264 | `avc1` | Most common, best compatibility | ✓ | **Yes** |
| H.264 | `avc3` | Alternative H.264 variant | ✓ | Yes |
| H.265/HEVC | `hev1` | Better compression, newer | ✓ | Maybe |
| H.265/HEVC | `hvc1` | Alternative HEVC variant | ✓ | Maybe |
| MPEG-4 | `mp4v` | Older standard | ✓ | No |
| VP9 | `vp09` | Google's codec | ✓ | No |
| AV1 | `av01` | Newest, best compression | ⚠ | No |

!!! tip
    Use `avc1` (H.264) unless you have a specific reason not to. It has the widest support across browsers and HMI platforms.

The full list of codes the encoder backend reports is in the [appendix](#appendix).

### VideoDeleteTime

* Type: floating point number
* Default: `1`

How many days a video stays on disk before automatic cleanup. Videos older than this are deleted the next time maintenance runs. Fractions work: `0.5` keeps videos for half a day (12 hours). See [Video maintenance](#video-maintenance) for when the check actually runs.

### AdsPort

* Type: integer
* Default: `26129`

The ADS port the service hosts its server on.

!!! danger
    Do not change this value. The PLC library connects to this port through its `ADS_PORT_FOR_IMAGE_TO_VIDEO` parameter, and both sides default to 26129. If the two values disagree, every video request from the PLC fails. If another ADS server on the target already occupies 26129, change both sides together and keep the port inside the customer ranges 25000 to 25999 or 26000 to 26999, which Beckhoff reserves for user servers. See the [PLC Library](./plc-library.md) reference for the library parameter.

### MaxFolderSize

* Type: integer
* Default: `250`

The size cap for a video folder in MB. After each encode the service sums the videos in the folder the new video landed in and deletes the oldest one when the total is over the cap. The cap applies per camera folder, not to the disk as a whole, so budget `MaxFolderSize` times the number of cameras when sizing storage. See [Video maintenance](#video-maintenance) for the exact behavior.

### VideoRootDirectory

* Type: string
* Default: empty (check disabled)

Added in the 2.1.0 release. When empty, the service accepts any path the PLC sends, including network paths. When set to a path, both the image source directory and the video output file of every request must resolve to that directory or below it. Requests with a path outside the root, UNC paths (network paths of the form `\\server\share`), and paths that fail to resolve are rejected with an ADS error and a log entry.

The reason this setting exists: after a successful encode the service deletes the image source directory it was commanded with, recursively. A misconfigured path in the PLC would make the service delete a directory it should never touch. Confining requests to the video root removes that risk.

Set it to the `VideoOutputDirectory` you configure on your `FB_ImageToVideo` instances. The library writes both the image dumps and the finished videos below that directory, so one root covers both paths. If your instances use different output directories, set the closest common parent.

## Validation

A missing, malformed, or partial configuration file never stops the service. Every content problem falls back to a default:

* File missing: a warning is written to the log and all defaults apply.
* File present but not valid JSON: an error is written to the log and all defaults apply.
* Field missing or invalid: the default for that field applies, the rest of the file is honored. A field counts as invalid when it is empty or whitespace (`CodecFourCC`), zero or negative (`VideoDeleteTime`), or zero (`AdsPort`, `MaxFolderSize`). A value of the wrong type, for example a quoted number or a negative number for `AdsPort` or `MaxFolderSize`, counts as a malformed file and drops every field to its default.

Key names are matched without regard to case, so `adsport` works the same as `AdsPort`.

The defaults are the same values as the shipped file, so deleting the file or removing a field returns you to shipped behavior. The 2.0.x releases carried different default values in different fallback paths, so an empty or partial file could put the server on the wrong port or zero out the retention limits. Since the 2.1.0 release there is one default set and every fallback resolves to it.

At startup the service writes the effective configuration to the Windows Application event log as a single line with the codec, port, retention time, folder size cap, and root directory. After changing the file, restart the service and check that line to confirm your values took effect.

## Video maintenance

Maintenance runs after every successful encode, on the folder the new video was written to. The PLC library gives each camera its own subfolder under `VideoOutputDirectory`, so each camera folder is maintained independently. The service scans the `*.mp4` files in that folder in two passes:

1. Age pass: every video whose file creation time is older than `VideoDeleteTime` days is deleted.
2. Size pass: the sizes of the remaining videos are summed. When the total exceeds `MaxFolderSize` MB, the single oldest remaining video is deleted.

The size pass removes only one file per run. That is deliberate: if the folder is still over the cap, the next encode triggers another pass, so the folder shrinks gradually instead of losing several videos at once.

Two consequences worth knowing:

* There is no timer. Maintenance runs only when a video is created, so a camera that stops triggering events keeps its last videos past their retention time until the next event fires.
* Only the folder of the newly created video is scanned. If you retire a camera or change its output directory, clean up the old folder by hand.

## Appendix

The list below is what the OpenCV FFMPEG backend reports for the MP4 container. Most entries are not web compatible, and the audio and subtitle tags (`mp4a`, `tx3g`, and similar) do not apply to this service. The [MDN video codec guide](https://developer.mozilla.org/en-US/docs/Web/Media/Formats/Video_codecs) covers what browsers can play.

??? note "Full FourCC code list"

    ```text
    OpenCV: FFMPEG: format mp4 / MP4 (MPEG-4 Part 14)
    fourcc tag 0x7634706d/'mp4v' codec_id 000C
    fourcc tag 0x31637661/'avc1' codec_id 001B
    fourcc tag 0x33637661/'avc3' codec_id 001B
    fourcc tag 0x31766568/'hev1' codec_id 00AD
    fourcc tag 0x31637668/'hvc1' codec_id 00AD
    fourcc tag 0x7634706d/'mp4v' codec_id 0002
    fourcc tag 0x7634706d/'mp4v' codec_id 0001
    fourcc tag 0x7634706d/'mp4v' codec_id 0007
    fourcc tag 0x7634706d/'mp4v' codec_id 003D
    fourcc tag 0x7634706d/'mp4v' codec_id 0058
    fourcc tag 0x312d6376/'vc-1' codec_id 0046
    fourcc tag 0x63617264/'drac' codec_id 0074
    fourcc tag 0x7634706d/'mp4v' codec_id 00A3
    fourcc tag 0x39307076/'vp09' codec_id 00A7
    fourcc tag 0x31307661/'av01' codec_id 801D
    fourcc tag 0x6134706d/'mp4a' codec_id 15002
    fourcc tag 0x63616c61/'alac' codec_id 15010
    fourcc tag 0x6134706d/'mp4a' codec_id 1502D
    fourcc tag 0x6134706d/'mp4a' codec_id 15001
    fourcc tag 0x6134706d/'mp4a' codec_id 15000
    fourcc tag 0x332d6361/'ac-3' codec_id 15003
    fourcc tag 0x332d6365/'ec-3' codec_id 15028
    fourcc tag 0x6134706d/'mp4a' codec_id 15004
    fourcc tag 0x61706c6d/'mlpa' codec_id 1502C
    fourcc tag 0x43614c66/'fLaC' codec_id 1500C
    fourcc tag 0x7375704f/'Opus' codec_id 1503C
    fourcc tag 0x6134706d/'mp4a' codec_id 15005
    fourcc tag 0x6134706d/'mp4a' codec_id 15018
    fourcc tag 0x6134706d/'mp4a' codec_id 15803
    fourcc tag 0x7334706d/'mp4s' codec_id 17000
    fourcc tag 0x67337874/'tx3g' codec_id 17005
    fourcc tag 0x646d7067/'gpmd' codec_id 18807
    fourcc tag 0x316d686d/'mhm1' codec_id 15817
    ```
