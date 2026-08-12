using EventVideoPlaybackService;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace EventVideoPlaybackService.Tests
{
    /// <summary>
    /// Locks down configuration loading: the config resolves against a given directory, and every
    /// missing or invalid value falls back to the single canonical default set. These tests guard
    /// the fix for the three divergent default sets and the CWD-relative path.
    /// </summary>
    public sealed class ConfigLoadingTests : IDisposable
    {
        private const string ConfigFileName = "EventVideoPlaybackService.config.json";
        private readonly List<string> _tempDirs = new();

        private string NewDirWith(string? contents)
        {
            string dir = Path.Combine(Path.GetTempPath(), "evp-cfg-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            _tempDirs.Add(dir);
            if (contents is not null)
            {
                File.WriteAllText(Path.Combine(dir, ConfigFileName), contents);
            }
            return dir;
        }

        private static ConfigData Load(string dir) => WindowsBackgroundService.LoadConfig(dir, NullLogger.Instance);

        private static void AssertCanonicalDefaults(ConfigData config)
        {
            Assert.Equal(WindowsBackgroundService.DefaultCodecFourCC, config.CodecFourCC);
            Assert.Equal(WindowsBackgroundService.DefaultVideoDeleteTime, config.VideoDeleteTime);
            Assert.Equal(WindowsBackgroundService.DefaultAdsPort, config.AdsPort);
            Assert.Equal(WindowsBackgroundService.DefaultMaxFolderSize, config.MaxFolderSize);
            Assert.Equal(string.Empty, config.VideoRootDirectory);
        }

        [Fact]
        public void MissingFile_UsesCanonicalDefaults()
        {
            AssertCanonicalDefaults(Load(NewDirWith(null)));
        }

        [Fact]
        public void LockedFile_UsesCanonicalDefaults_InsteadOfThrowing()
        {
            // A file that exists but cannot be read (exclusive lock, ACL) used to escape
            // LoadConfig, reach the generic handler in ExecuteAsync, and exit the process,
            // so the service manager restarted it into the same locked file forever.
            string dir = NewDirWith("{\"AdsPort\":26129}");
            using var exclusiveLock = new FileStream(
                Path.Combine(dir, ConfigFileName), FileMode.Open, FileAccess.Read, FileShare.None);

            AssertCanonicalDefaults(Load(dir));
        }

        [Fact]
        public void EmptyFile_UsesCanonicalDefaults()
        {
            AssertCanonicalDefaults(Load(NewDirWith("")));
        }

        [Fact]
        public void EmptyJsonObject_UsesCanonicalDefaults()
        {
            AssertCanonicalDefaults(Load(NewDirWith("{}")));
        }

        [Fact]
        public void MalformedJson_UsesCanonicalDefaults()
        {
            AssertCanonicalDefaults(Load(NewDirWith("{ this is not valid json")));
        }

        [Fact]
        public void PartialConfig_AppliesPerFieldDefaults()
        {
            // Only the codec is set; the missing fields must NOT be left at zero (which would route
            // to a dynamic ADS port and delete every video), they must take the canonical default.
            ConfigData config = Load(NewDirWith("{\"CodecFourCC\":\"mp4v\"}"));

            Assert.Equal("mp4v", config.CodecFourCC);
            Assert.Equal(WindowsBackgroundService.DefaultVideoDeleteTime, config.VideoDeleteTime);
            Assert.Equal(WindowsBackgroundService.DefaultAdsPort, config.AdsPort);
            Assert.NotEqual((ushort)0, config.AdsPort);
            Assert.Equal(WindowsBackgroundService.DefaultMaxFolderSize, config.MaxFolderSize);
        }

        [Fact]
        public void MisCasedKeys_AreHonored()
        {
            ConfigData config = Load(NewDirWith("{\"codecfourcc\":\"h264\",\"adsport\":25001}"));

            Assert.Equal("h264", config.CodecFourCC);
            Assert.Equal((ushort)25001, config.AdsPort);
        }

        [Fact]
        public void FullValidConfig_HonoredVerbatim()
        {
            ConfigData config = Load(NewDirWith(
                "{\"CodecFourCC\":\"mp4v\",\"VideoDeleteTime\":7,\"AdsPort\":25500,\"MaxFolderSize\":1000,\"VideoRootDirectory\":\"D:/Videos\"}"));

            Assert.Equal("mp4v", config.CodecFourCC);
            Assert.Equal(7d, config.VideoDeleteTime);
            Assert.Equal((ushort)25500, config.AdsPort);
            Assert.Equal(1000ul, config.MaxFolderSize);
            Assert.Equal("D:/Videos", config.VideoRootDirectory);
        }

        [Fact]
        public void FractionalRetention_Survives()
        {
            // A sub-day retention is a legitimate value and must not be treated as "missing".
            ConfigData config = Load(NewDirWith("{\"CodecFourCC\":\"avc1\",\"VideoDeleteTime\":0.5}"));

            Assert.Equal(0.5d, config.VideoDeleteTime);
        }

        [Fact]
        public void ZeroAdsPort_DefaultsToCanonical()
        {
            ConfigData config = Load(NewDirWith("{\"CodecFourCC\":\"avc1\",\"AdsPort\":0}"));

            Assert.Equal(WindowsBackgroundService.DefaultAdsPort, config.AdsPort);
        }

        [Fact]
        public void AllFallbackPaths_ProduceIdenticalDefaults()
        {
            ConfigData missing = Load(NewDirWith(null));
            ConfigData empty = Load(NewDirWith("{}"));
            ConfigData malformed = Load(NewDirWith("not json"));

            foreach (ConfigData other in new[] { empty, malformed })
            {
                Assert.Equal(missing.CodecFourCC, other.CodecFourCC);
                Assert.Equal(missing.VideoDeleteTime, other.VideoDeleteTime);
                Assert.Equal(missing.AdsPort, other.AdsPort);
                Assert.Equal(missing.MaxFolderSize, other.MaxFolderSize);
                Assert.Equal(missing.VideoRootDirectory, other.VideoRootDirectory);
            }
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
