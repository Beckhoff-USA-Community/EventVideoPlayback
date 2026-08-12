using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace EventVideoPlaybackService.Tests
{
    /// <summary>
    /// Locks the video maintenance behavior: expired videos are deleted, the folder size cap
    /// deletes the oldest other video, and the video the maintenance run was triggered for is
    /// never a deletion candidate. Found in review 2026-08-10: when the new video was the only
    /// file over the cap, maintenance deleted it seconds after the encode, after the source
    /// images were already gone, destroying the footage with no signal anywhere.
    /// </summary>
    public sealed class MaintenanceTests : IDisposable
    {
        private readonly List<string> _tempDirs = new();

        private string NewCameraFolder()
        {
            string dir = Path.Combine(Path.GetTempPath(), "evp-maint-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            _tempDirs.Add(dir);
            return dir;
        }

        private static string WriteVideo(string folder, string name, double sizeMb, int ageDays)
        {
            string path = Path.Combine(folder, name);
            File.WriteAllBytes(path, new byte[(long)(sizeMb * 1024 * 1024)]);
            File.SetCreationTime(path, DateTime.Now.AddDays(-ageDays));
            return path;
        }

        [Fact]
        public async Task NewVideo_SurvivesSizeCap_WhenItIsTheOnlyFile()
        {
            // The field scenario: one video larger than the whole cap, nothing else in the
            // folder. Before the fix, pass two nominated the new video as the oldest file
            // and deleted it.
            string folder = NewCameraFolder();
            string newVideo = WriteVideo(folder, "new.mp4", sizeMb: 2, ageDays: 0);
            using var server = new TestableServer(NullLogger.Instance, maxFolderSizeMb: 1);

            await server.PerformVideoMaintenanceAsync(newVideo);

            Assert.True(File.Exists(newVideo));
        }

        [Fact]
        public async Task NewVideo_SurvivesRetention_EvenWithTinyRetention()
        {
            string folder = NewCameraFolder();
            string newVideo = WriteVideo(folder, "new.mp4", sizeMb: 0.1, ageDays: 0);
            // Creation time in the past plus a fractional retention makes the new video
            // "expired" by the age arithmetic; it must still survive.
            File.SetCreationTime(newVideo, DateTime.Now.AddHours(-1));
            using var server = new TestableServer(NullLogger.Instance, videoDeleteTime: 0.01);

            await server.PerformVideoMaintenanceAsync(newVideo);

            Assert.True(File.Exists(newVideo));
        }

        [Fact]
        public async Task ExpiredVideos_AreDeleted_NewVideoStays()
        {
            string folder = NewCameraFolder();
            string oldVideo = WriteVideo(folder, "old.mp4", sizeMb: 0.1, ageDays: 10);
            string newVideo = WriteVideo(folder, "new.mp4", sizeMb: 0.1, ageDays: 0);
            using var server = new TestableServer(NullLogger.Instance, videoDeleteTime: 1);

            await server.PerformVideoMaintenanceAsync(newVideo);

            Assert.False(File.Exists(oldVideo));
            Assert.True(File.Exists(newVideo));
        }

        [Fact]
        public async Task SizeCap_DeletesTheOldestOtherVideo()
        {
            string folder = NewCameraFolder();
            string oldest = WriteVideo(folder, "oldest.mp4", sizeMb: 0.6, ageDays: 2);
            string middle = WriteVideo(folder, "middle.mp4", sizeMb: 0.6, ageDays: 1);
            string newVideo = WriteVideo(folder, "new.mp4", sizeMb: 0.6, ageDays: 0);
            using var server = new TestableServer(NullLogger.Instance, videoDeleteTime: 30, maxFolderSizeMb: 1);

            await server.PerformVideoMaintenanceAsync(newVideo);

            Assert.False(File.Exists(oldest));
            Assert.True(File.Exists(middle));
            Assert.True(File.Exists(newVideo));
        }

        [Fact]
        public async Task SizeCap_CatchesFractionalOverrun()
        {
            // 0.6 + 0.6 = 1.2 MB against a 1 MB cap. The old (ulong) cast truncated 1.2 to 1,
            // so a folder up to a megabyte over the cap was never cleaned.
            string folder = NewCameraFolder();
            string oldVideo = WriteVideo(folder, "old.mp4", sizeMb: 0.6, ageDays: 1);
            string newVideo = WriteVideo(folder, "new.mp4", sizeMb: 0.6, ageDays: 0);
            using var server = new TestableServer(NullLogger.Instance, videoDeleteTime: 30, maxFolderSizeMb: 1);

            await server.PerformVideoMaintenanceAsync(newVideo);

            Assert.False(File.Exists(oldVideo));
            Assert.True(File.Exists(newVideo));
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
