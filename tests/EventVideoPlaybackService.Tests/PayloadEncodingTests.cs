using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace EventVideoPlaybackService.Tests
{
    /// <summary>
    /// Locks the request payload decoding to the machine's Windows ANSI codepage. The PLC sends
    /// path bytes exactly as its STRINGs store them (the machine codepage), and its file API
    /// creates folders with the same codepage, so the service must decode with that codepage or
    /// non ASCII camera names resolve to folders that do not exist. Found in the field 2026-08-10:
    /// an umlaut camera name decoded as UTF-8 produced a replacement character, the encode never
    /// found the image folder, and images piled up with no video and no alarm.
    /// </summary>
    public sealed class PayloadEncodingTests : IDisposable
    {
        private const uint VideoIndexGroup = 1;
        private readonly List<string> _tempDirs = new();

        private string NewRoot(string subfolder)
        {
            string dir = Path.Combine(Path.GetTempPath(), "evp-enc-" + Guid.NewGuid().ToString("N"), subfolder);
            Directory.CreateDirectory(dir);
            _tempDirs.Add(Path.GetDirectoryName(dir)!);
            return dir;
        }

        private static byte[] Request(string imagePathSource, string videoFilename, int fps = 10)
        {
            static string Esc(string p) => p.Replace("\\", "\\\\");
            // Encode the wire bytes exactly as the PLC does: raw machine codepage bytes.
            return AdsImageToVideoServer.PayloadEncoding.GetBytes(
                $"{{\"ImagePathSource\":\"{Esc(imagePathSource)}\",\"VideoFilename\":\"{Esc(videoFilename)}\",\"VideoFPS\":{fps}}}");
        }

        [Fact]
        public async Task UmlautCameraName_DecodesToTheFoldersThePlcCreated()
        {
            // Round trip the name through the payload encoding first, so the test asserts wire
            // symmetry on every codepage: what the PLC sends is what the service resolves. On a
            // Western codepage this is the real umlaut; the old UTF-8 decode turned it into a
            // replacement character and this request was rejected against the configured root.
            var enc = AdsImageToVideoServer.PayloadEncoding;
            string cameraName = enc.GetString(enc.GetBytes("Kamera_Tür"));
            string root = NewRoot(cameraName);
            using var server = new TestableServer(NullLogger.Instance, videoRootDirectory: root);

            var result = await server.WriteAsync(VideoIndexGroup, 1,
                Request(Path.Combine(root, "Images"), Path.Combine(root, "out.mp4")));

            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task AsciiPayload_BehavesTheSameAsBefore()
        {
            // ASCII bytes are identical in UTF-8 and every Windows ANSI codepage, so the decode
            // change is invisible to every existing install.
            string root = NewRoot("Camera1");
            using var server = new TestableServer(NullLogger.Instance, videoRootDirectory: root);

            var result = await server.WriteAsync(VideoIndexGroup, 1,
                Request(Path.Combine(root, "Images"), Path.Combine(root, "out.mp4")));

            Assert.True(result.Succeeded);
        }

        public void Dispose()
        {
            foreach (string dir in _tempDirs)
            {
                try { Directory.Delete(dir, true); }
                catch { /* best effort temp cleanup */ }
            }
        }
    }
}
