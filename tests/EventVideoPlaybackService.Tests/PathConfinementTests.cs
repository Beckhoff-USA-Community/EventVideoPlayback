using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using TwinCAT.Ads;
using Xunit;

namespace EventVideoPlaybackService.Tests
{
    /// <summary>
    /// Locks the optional VideoRootDirectory confinement: with it set, requests whose image source
    /// or video output resolves outside the root (or is a UNC path) are rejected before the service
    /// acts on them, since the image directory is deleted recursively. With it unset, any local
    /// path is accepted (preserving the fielded default behavior).
    /// </summary>
    public sealed class PathConfinementTests : IDisposable
    {
        private const uint VideoIndexGroup = 1;
        private readonly List<string> _tempDirs = new();

        private string NewRoot()
        {
            string dir = Path.Combine(Path.GetTempPath(), "evp-root-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            _tempDirs.Add(dir);
            return dir;
        }

        private static byte[] Request(string imagePathSource, string videoFilename, int fps = 10)
        {
            static string Esc(string p) => p.Replace("\\", "\\\\");
            return Encoding.UTF8.GetBytes(
                $"{{\"ImagePathSource\":\"{Esc(imagePathSource)}\",\"VideoFilename\":\"{Esc(videoFilename)}\",\"VideoFPS\":{fps}}}");
        }

        [Fact]
        public async Task ConfinementOff_AllowsAnyPath()
        {
            using var server = new TestableServer(NullLogger.Instance, videoRootDirectory: "");

            var result = await server.WriteAsync(VideoIndexGroup, 1,
                Request(@"C:\anywhere\images", @"C:\anywhere\out.mp4"));

            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task ConfinementOn_AllowsPathsUnderRoot()
        {
            string root = NewRoot();
            using var server = new TestableServer(NullLogger.Instance, videoRootDirectory: root);

            var result = await server.WriteAsync(VideoIndexGroup, 1,
                Request(Path.Combine(root, "Camera1", "images"), Path.Combine(root, "Camera1", "out.mp4")));

            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task ConfinementOn_RejectsImagePathOutsideRoot()
        {
            string root = NewRoot();
            using var server = new TestableServer(NullLogger.Instance, videoRootDirectory: root);

            var result = await server.WriteAsync(VideoIndexGroup, 1,
                Request(@"C:\elsewhere\images", Path.Combine(root, "out.mp4")));

            Assert.Equal(AdsErrorCode.ClientInvalidParameter, result.ErrorCode);
        }

        [Fact]
        public async Task ConfinementOn_RejectsVideoFileOutsideRoot()
        {
            string root = NewRoot();
            using var server = new TestableServer(NullLogger.Instance, videoRootDirectory: root);

            var result = await server.WriteAsync(VideoIndexGroup, 1,
                Request(Path.Combine(root, "images"), @"C:\elsewhere\out.mp4"));

            Assert.Equal(AdsErrorCode.ClientInvalidParameter, result.ErrorCode);
        }

        [Fact]
        public async Task ConfinementOn_RejectsUncPath()
        {
            string root = NewRoot();
            using var server = new TestableServer(NullLogger.Instance, videoRootDirectory: root);

            var result = await server.WriteAsync(VideoIndexGroup, 1,
                Request(@"\\server\share\images", @"\\server\share\out.mp4"));

            Assert.Equal(AdsErrorCode.ClientInvalidParameter, result.ErrorCode);
        }

        [Fact]
        public async Task ConfinementOn_RejectsRootPrefixSibling()
        {
            // A directory whose name merely starts with the root string must not be treated as inside.
            string root = NewRoot();
            using var server = new TestableServer(NullLogger.Instance, videoRootDirectory: root);

            var result = await server.WriteAsync(VideoIndexGroup, 1,
                Request(root + "-sibling\\images", Path.Combine(root, "out.mp4")));

            Assert.Equal(AdsErrorCode.ClientInvalidParameter, result.ErrorCode);
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
