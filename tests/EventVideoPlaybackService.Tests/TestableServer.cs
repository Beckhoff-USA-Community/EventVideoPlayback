using Microsoft.Extensions.Logging;
using TwinCAT.Ads;
using TwinCAT.Ads.Server;

namespace EventVideoPlaybackService.Tests
{
    /// <summary>
    /// Test double that exposes the protected OnWriteAsync handler so the ADS write path can be
    /// driven directly. The port-0 base constructor registers no ADS port and needs no running
    /// TwinCAT router, and the request never reaches an encode because tests use paths that do
    /// not exist on disk.
    /// </summary>
    internal sealed class TestableServer : AdsImageToVideoServer
    {
        public TestableServer(ILogger logger, string videoRootDirectory = "", double videoDeleteTime = 5, ulong maxFolderSizeMb = 250)
            : base(0, "EventVideoPlaybackTestServer", logger, videoDeleteTime, codecFourCC: "avc1", maxFileSize: maxFolderSizeMb, videoRootDirectory)
        {
        }

        /// <summary>
        /// Invokes the write handler exactly as the ADS server base would, returning the result
        /// the PLC client would receive.
        /// </summary>
        public Task<ResultWrite> WriteAsync(uint indexGroup, uint indexOffset, byte[] data)
            => OnWriteAsync(AmsAddress.Empty, 1u, indexGroup, indexOffset, data, CancellationToken.None);
    }
}
