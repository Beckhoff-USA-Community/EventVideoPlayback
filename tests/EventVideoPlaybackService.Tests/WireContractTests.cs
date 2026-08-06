using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using TwinCAT.Ads;
using Xunit;

namespace EventVideoPlaybackService.Tests
{
    /// <summary>
    /// Locks the ADS write wire contract the fielded PLC library depends on: heartbeat semantics,
    /// the "any index group other than 10 is a video request" rule, index-offset being ignored,
    /// and the response codes for valid, malformed, and incomplete payloads.
    /// </summary>
    public sealed class WireContractTests
    {
        private const uint HeartbeatIndexGroup = 10;
        private const uint VideoIndexGroup = 1;

        private static byte[] Bytes(string s) => Encoding.UTF8.GetBytes(s);

        private static byte[] ValidRequest() =>
            Bytes("{\"ImagePathSource\":\"C:\\\\evp-tests-nonexistent\\\\images\",\"VideoFilename\":\"C:\\\\evp-tests-nonexistent\\\\out.mp4\",\"VideoFPS\":10}");

        [Fact]
        public async Task Heartbeat_ReturnsSuccess_IgnoringPayload()
        {
            using var server = new TestableServer(NullLogger.Instance);

            var result = await server.WriteAsync(HeartbeatIndexGroup, indexOffset: 10, new byte[] { 1 });

            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task ValidRequest_ReturnsSuccess()
        {
            using var server = new TestableServer(NullLogger.Instance);

            var result = await server.WriteAsync(VideoIndexGroup, indexOffset: 1, ValidRequest());

            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task NonHeartbeatIndexGroup_IsTreatedAsVideoRequest()
        {
            using var server = new TestableServer(NullLogger.Instance);

            // Any index group that is not 10 must be handled as a video request, not rejected.
            var result = await server.WriteAsync(indexGroup: 42, indexOffset: 0, ValidRequest());

            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task IndexOffset_IsIgnored()
        {
            using var server = new TestableServer(NullLogger.Instance);

            var withZero = await server.WriteAsync(VideoIndexGroup, indexOffset: 0, ValidRequest());
            var withOther = await server.WriteAsync(VideoIndexGroup, indexOffset: 999, ValidRequest());

            Assert.Equal(withZero.ErrorCode, withOther.ErrorCode);
            Assert.True(withZero.Succeeded);
        }

        [Fact]
        public async Task MissingVideoFilename_ReturnsClientInvalidParameter()
        {
            using var server = new TestableServer(NullLogger.Instance);

            var result = await server.WriteAsync(VideoIndexGroup, 1,
                Bytes("{\"ImagePathSource\":\"C:\\\\x\",\"VideoFPS\":10}"));

            Assert.Equal(AdsErrorCode.ClientInvalidParameter, result.ErrorCode);
        }

        [Fact]
        public async Task ZeroVideoFps_ReturnsClientInvalidParameter()
        {
            using var server = new TestableServer(NullLogger.Instance);

            var result = await server.WriteAsync(VideoIndexGroup, 1,
                Bytes("{\"ImagePathSource\":\"C:\\\\x\",\"VideoFilename\":\"C:\\\\x\\\\out.mp4\",\"VideoFPS\":0}"));

            Assert.Equal(AdsErrorCode.ClientInvalidParameter, result.ErrorCode);
        }

        [Fact]
        public async Task NullField_ReturnsClientInvalidParameter()
        {
            using var server = new TestableServer(NullLogger.Instance);

            // A JSON null for ImagePathSource reads back as null and must be treated as missing.
            var result = await server.WriteAsync(VideoIndexGroup, 1,
                Bytes("{\"ImagePathSource\":null,\"VideoFilename\":\"C:\\\\x\\\\out.mp4\",\"VideoFPS\":10}"));

            Assert.Equal(AdsErrorCode.ClientInvalidParameter, result.ErrorCode);
        }

        [Fact]
        public async Task MalformedJson_ReturnsDeviceInvalidData_NotAThrow()
        {
            using var server = new TestableServer(NullLogger.Instance);

            var result = await server.WriteAsync(VideoIndexGroup, 1, Bytes("this is not json{"));

            Assert.Equal(AdsErrorCode.DeviceInvalidData, result.ErrorCode);
        }

        [Fact]
        public async Task TruncatedJson_ReturnsDeviceInvalidData()
        {
            using var server = new TestableServer(NullLogger.Instance);

            var result = await server.WriteAsync(VideoIndexGroup, 1,
                Bytes("{\"ImagePathSource\":\"C:\\\\x\",\"VideoFilename\":"));

            Assert.Equal(AdsErrorCode.DeviceInvalidData, result.ErrorCode);
        }

        [Fact]
        public async Task WrongTypeVideoFps_ReturnsDeviceInvalidData()
        {
            using var server = new TestableServer(NullLogger.Instance);

            // GetUInt16 on a JSON string throws, which must be caught and mapped to DeviceInvalidData.
            var result = await server.WriteAsync(VideoIndexGroup, 1,
                Bytes("{\"ImagePathSource\":\"C:\\\\x\",\"VideoFilename\":\"C:\\\\x\\\\out.mp4\",\"VideoFPS\":\"ten\"}"));

            Assert.Equal(AdsErrorCode.DeviceInvalidData, result.ErrorCode);
        }
    }
}
