using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.Server;
using TwinCAT.Ads;
using OpenCvSharp;
using Microsoft.Extensions.Logging;
using EventVideoPlaybackService;
using System.IO;

namespace EventVideoPlaybackService
{
    /// <summary>
    /// ADS server implementation that receives image sequences from TwinCAT PLC clients and converts them into video files.
    /// This server accepts JSON-formatted requests containing image source paths and video parameters,
    /// processes the images using OpenCV, and manages video file lifecycle (creation, cleanup, size limits).
    /// </summary>
    public class AdsImageToVideoServer : AdsServer
    {
        private string? JsonString;
        private String? ImagePathSource;
        private String? VideoFilename;
        private UInt16 VideoFPS;
        private Int32 CodecValue;

        private AdsState _localAdsState = AdsState.Config;
        private ushort _localDeviceState = 0;
        private readonly ConcurrentDictionary<uint, NotificationRequestEntry> _notificationTable = new();
        private uint _currentNotificationHandle = 0;

        private IServerLogger _serverLogger;

        readonly private double KeepVideoTime;   // Video retention period in days
        readonly private string CodecFourCC;
        readonly private ulong maxFolderSize;  // Maximum folder size in MB (1024 MB = 1 GB)

        /// <summary>
        /// Initializes a new instance of the AdsImageToVideoServer with the specified configuration parameters.
        /// </summary>
        /// <param name="port">The ADS port number to use (must be in range 25000-25999 or 26000-26999).</param>
        /// <param name="portName">The name to register this ADS server with.</param>
        /// <param name="logger">The logger instance for logging operations.</param>
        /// <param name="videoDeleteTime">The number of days to keep video files before automatic deletion.</param>
        /// <param name="codecFourCC">The FourCC code for the video codec (e.g., "avc1" for H.264, "mp4v" for MPEG-4).</param>
        /// <param name="maxFileSize">The maximum total size of video files in the folder in megabytes.</param>
        public AdsImageToVideoServer(ushort port, string portName, ILogger logger, double videoDeleteTime, string codecFourCC,ulong maxFileSize) : base(port, portName, null)
        {
            _serverLogger = new ServerLogger(logger);
            this.CodecFourCC = codecFourCC;
            this.KeepVideoTime = videoDeleteTime;
            this.maxFolderSize = maxFileSize;
            _serverLogger.Logger.LogWarning(videoDeleteTime.ToString()+ " : "+ port);
        }

        /// <summary>
        /// Initializes a new instance of the AdsImageToVideoServer with a dynamic ADS port assigned by the router.
        /// This constructor is primarily for testing purposes with default/minimal configuration.
        /// </summary>
        /// <param name="portName">The name to register this ADS server with.</param>
        /// <param name="logger">The logger instance for logging operations.</param>
        public AdsImageToVideoServer(string portName, ILogger logger) : base(0, portName, null)
        {
            _serverLogger = new ServerLogger(logger);
            this.CodecFourCC = "";
            this.KeepVideoTime = 0;
            this.maxFolderSize = 0;
        }

        /// <summary>
        /// Called when the ADS server successfully connects to the TwinCAT router.
        /// Logs the connection event with the server type and address information.
        /// </summary>
        protected override void OnConnected()
        {
             _serverLogger.Logger.LogWarning($"Server '{this.GetType()}', Address: {base.ServerAddress} connected!");
        }

        /// <summary>
        /// Called when the ADS server connection state changes.
        /// Logs the new connection state for monitoring and troubleshooting purposes.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">Event arguments containing the new connection state.</param>
        protected override void OnServerConnectionStateChanged(object? sender, ServerConnectionStateChangedEventArgs e)
        {
            base.OnServerConnectionStateChanged(sender, e);

            _serverLogger.Logger.LogWarning("ConnectionState Changed " + e.State );
        }

        /// <summary>
        /// Handles incoming ADS write requests from clients.
        /// This method accepts JSON-formatted video creation requests containing image source path, video filename, and FPS.
        /// The video creation is queued as a background task to prevent blocking the ADS communication channel.
        /// </summary>
        /// <param name="sender">The AMS address of the client sending the write request.</param>
        /// <param name="invokeId">The unique identifier for this ADS request.</param>
        /// <param name="indexGroup">The ADS index group (10 = heartbeat/no-op, other values = video creation request).</param>
        /// <param name="indexOffset">The ADS index offset (not used in this implementation).</param>
        /// <param name="writeData">The JSON data containing ImagePathSource, VideoFilename, and VideoFPS.</param>
        /// <param name="cancel">Cancellation token for async operations.</param>
        /// <returns>A task containing the write operation result (success or error code).</returns>
        /// <example>
        /// Expected JSON format from PLC client:
        /// <code>
        /// {
        ///   "ImagePathSource": "C:\\Images\\Event_2024_01_15\\",
        ///   "VideoFilename": "C:\\Videos\\Event_2024_01_15.mp4",
        ///   "VideoFPS": 30
        /// }
        /// </code>
        /// </example>
        protected override Task<ResultWrite> OnWriteAsync(AmsAddress sender, uint invokeId, uint indexGroup, uint indexOffset, ReadOnlyMemory<byte> writeData, CancellationToken cancel)
        {
            ResultWrite result;
            Int32 dataSize = writeData.Length;

            // IndexGroup 10 is used as a heartbeat/keep-alive mechanism
            if (indexGroup == 10)
            {
                result = ResultWrite.CreateSuccess();
                return Task.FromResult(result);
            }
            else
            {
                // Parse the JSON data from the write request
                JsonString = System.Text.Encoding.UTF8.GetString(writeData.ToArray(), 0, dataSize);
                _serverLogger.Logger.LogWarning(JsonString);

                using var doc = JsonDocument.Parse(JsonString);
                var root = doc.RootElement;

                // Extract video creation parameters from JSON
                foreach (var item in root.EnumerateObject())
                {
                    switch (item.Name)
                    {
                        case "ImagePathSource":
                            ImagePathSource = item.Value.GetString();
                            break;
                        case "VideoFilename":
                            VideoFilename = item.Value.GetString();
                            break;
                        case "VideoFPS":
                            VideoFPS = item.Value.GetUInt16();
                            break;
                    }
                }

                // Validate that all required parameters are present
                if (ImagePathSource is not null && VideoFilename is not null && VideoFPS > 0)
                {
                    // Queue video creation in background to avoid blocking ADS communication
                    _ = Task.Run(() => CreateVideoFromImagesAsync(ImagePathSource, VideoFilename, VideoFPS));
                    result = ResultWrite.CreateSuccess();
                    return Task.FromResult(result);
                }
                else
                {
                    result = ResultWrite.CreateError(AdsErrorCode.ClientInvalidParameter);
                    return Task.FromResult(result);
                }
            }
        }

        /// <summary>
        /// Creates a video file from a sequence of PNG images using OpenCV.
        /// This method performs the following steps:
        /// 1. Validates the image source directory and loads all PNG files
        /// 2. Reads the first image to determine video dimensions
        /// 3. Creates a VideoWriter with the specified codec and frame rate
        /// 4. Processes each image and writes it to the video file
        /// 5. Deletes the source image directory after successful video creation
        /// 6. Performs video maintenance (cleanup old files and enforce size limits)
        /// </summary>
        /// <param name="imagePathSource">The directory path containing the source PNG images.</param>
        /// <param name="videoFilename">The full path for the output video file.</param>
        /// <param name="videoFPS">The frame rate (frames per second) for the output video.</param>
        /// <returns>A task representing the asynchronous video creation operation.</returns>
        private async Task CreateVideoFromImagesAsync(string imagePathSource, string videoFilename, ushort videoFPS)
        {
            try
            {
                // Validate the image source directory exists
                if (!Directory.Exists(imagePathSource))
                {
                    _serverLogger.Logger.LogError($"Image directory does not exist: {imagePathSource}");
                    return;
                }

                // Load all PNG files from the source directory
                string[] imageFiles = Directory.GetFiles(imagePathSource, "*.png");

                if (imageFiles.Length == 0)
                {
                    _serverLogger.Logger.LogError($"No PNG files found in directory: {imagePathSource}");
                    return;
                }

                // Read the first image to determine video dimensions (all images must be the same size)
                using Mat firstImage = Cv2.ImRead(imageFiles[0]);
                if (firstImage.Empty())
                {
                    _serverLogger.Logger.LogError($"Failed to read first image: {imageFiles[0]}");
                    return;
                }

                // Convert the FourCC string to codec value (e.g., "avc1" for H.264)
                CodecValue = FourCC.FromString(this.CodecFourCC);

                // Initialize the video writer with codec, frame rate, and dimensions
                using var videoWriter = new VideoWriter(videoFilename, CodecValue, videoFPS, firstImage.Size(), true);

                if (!videoWriter.IsOpened())
                {
                    _serverLogger.Logger.LogError($"Failed to open video writer for: {videoFilename}");
                    return;
                }

                // Process each image and write it as a frame to the video
                foreach (var imageFile in imageFiles)
                {
                    using Mat image = Cv2.ImRead(imageFile);
                    if (!image.Empty())
                    {
                        videoWriter.Write(image);
                    }
                }

                // VideoWriter is automatically disposed here, finalizing the video file

                // Clean up: delete the source image directory after successful video creation
                // This saves disk space by removing the original images once they're encoded into video
                try
                {
                    Directory.Delete(imagePathSource, true);
                }
                catch (Exception ex)
                {
                    // Expected exceptions: UnauthorizedAccessException (permissions), IOException (files in use), DirectoryNotFoundException
                    // Non-critical failure - log warning and continue (video was created successfully)
                    _serverLogger.Logger.LogWarning($"Failed to delete image directory {imagePathSource}: {ex.Message}");
                }

                // Perform video maintenance (cleanup old videos and enforce folder size limits)
                await PerformVideoMaintenanceAsync(videoFilename);
            }
            catch (Exception ex)
            {
                _serverLogger.Logger.LogError($"Video creation failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Performs maintenance on the video file directory by:
        /// 1. Deleting videos older than the configured retention period (KeepVideoTime)
        /// 2. Enforcing the maximum folder size limit by deleting the oldest video if necessary
        /// This ensures that storage space is managed automatically and old videos don't accumulate indefinitely.
        /// </summary>
        /// <param name="videoFilename">The path of the newly created video file (used to determine the directory to maintain).</param>
        /// <returns>A task representing the asynchronous maintenance operation.</returns>
        /// <remarks>
        /// This two-pass algorithm ensures time-based retention is prioritized over size limits.
        /// Pass 1: Delete all expired videos (older than KeepVideoTime days)
        /// Pass 2: If remaining videos exceed maxFolderSize, delete the oldest non-expired video
        ///
        /// Note: Only the oldest file is deleted in pass 2. If folder is still over the limit,
        /// the next video creation will trigger another maintenance cycle. This gradual approach
        /// prevents accidentally deleting multiple videos at once.
        /// </remarks>
        private async Task PerformVideoMaintenanceAsync(string videoFilename)
        {
            try
            {
                string? VideoFilePath = Path.GetDirectoryName(videoFilename);
                if (VideoFilePath == null) return;

                DateTime currentTime = DateTime.Now;
                string[] VideoFiles = Directory.GetFiles(VideoFilePath, "*.mp4");
                double size = 0;
                DateTime oldestFileTime = DateTime.Now;
                string oldestFileName = "";

                // First pass: Time-based cleanup and size calculation
                // Delete all videos that have exceeded the retention period
                // For remaining (non-expired) videos, track total size and identify the oldest
                foreach (var videoFile in VideoFiles)
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(videoFile);
                        DateTime creationTime = fileInfo.CreationTime;
                        TimeSpan difference = currentTime - creationTime;

                        // Delete videos that exceed the retention period
                        if (difference.TotalDays >= KeepVideoTime)
                        {
                            File.Delete(videoFile);
                        }
                        else
                        {
                            // Video is still within retention period - include in size calculation
                            // We only count non-expired videos because expired ones will be deleted above
                            size += (double)fileInfo.Length / (1024.0 * 1024.0);    // Convert bytes to MB

                            // Track the oldest non-expired video for potential size-based deletion
                            if (fileInfo.CreationTime < oldestFileTime)
                            {
                                oldestFileTime = fileInfo.CreationTime;
                                oldestFileName = fileInfo.FullName;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Expected exceptions: UnauthorizedAccessException, IOException (file locked), FileNotFoundException
                        // Non-critical failure - log and continue with other files
                        _serverLogger.Logger.LogWarning($"Failed to process video file {videoFile}: {ex.Message}");
                    }
                }

                // Second pass: Size-based cleanup (if necessary)
                // After removing expired videos, check if total size still exceeds limit
                // If yes, delete the oldest remaining video to bring size down
                if ((ulong)size > maxFolderSize && oldestFileName != "")
                {
                    try
                    {
                        File.Delete(oldestFileName);
                        _serverLogger.Logger.LogInformation($"Deleted oldest video {oldestFileName} to enforce size limit ({size:F2} MB > {maxFolderSize} MB)");
                    }
                    catch (Exception ex)
                    {
                        // Expected exceptions: UnauthorizedAccessException, IOException (file locked), FileNotFoundException
                        _serverLogger.Logger.LogWarning($"Failed to delete oldest video file {oldestFileName}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _serverLogger.Logger.LogError($"Video maintenance failed: {ex.Message}");
            }

            await Task.CompletedTask;
        }


        /// <summary>
        /// Handles incoming ADS read requests from clients.
        /// </summary>
        /// <param name="sender">The AMS address of the client sending the read request.</param>
        /// <param name="invokeId">The unique identifier for this ADS request.</param>
        /// <param name="indexGroup">The ADS index group specifying the data area to read from.</param>
        /// <param name="indexOffset">The ADS index offset within the specified group.</param>
        /// <param name="readLength">The number of bytes to read.</param>
        /// <param name="cancel">Cancellation token for async operations.</param>
        /// <returns>A task containing the read operation result. Currently returns "service not supported" error.</returns>
        /// <remarks>
        /// This server does not currently support read operations as it is designed primarily for write-based
        /// video creation requests. Future implementations could expose server status or statistics via read operations.
        /// </remarks>
        protected override Task<ResultReadBytes> OnReadAsync(AmsAddress sender, uint invokeId, uint indexGroup, uint indexOffset, int readLength, CancellationToken cancel)
        {
            ResultReadBytes result = ResultReadBytes.CreateError(AdsErrorCode.DeviceServiceNotSupported);
            return Task.FromResult(result);
        }

        /// <summary>
        /// Handles requests to read the current ADS device state.
        /// </summary>
        /// <param name="sender">The AMS address of the client requesting the device state.</param>
        /// <param name="invokeId">The unique identifier for this ADS request.</param>
        /// <param name="cancel">Cancellation token for async operations.</param>
        /// <returns>A task containing the current ADS state and device state values.</returns>
        /// <remarks>
        /// Returns the current state maintained in _localAdsState and _localDeviceState fields.
        /// These states can be modified via OnWriteControlAsync.
        /// </remarks>
        protected override Task<ResultReadDeviceState> OnReadDeviceStateAsync(AmsAddress sender, uint invokeId, CancellationToken cancel)
        {
            ResultReadDeviceState result = ResultReadDeviceState.CreateSuccess(new StateInfo(_localAdsState, _localDeviceState));
            return Task.FromResult(result);
        }

        /// <summary>
        /// Handles ADS write control requests to change the device state.
        /// </summary>
        /// <param name="sender">The AMS address of the client sending the control request.</param>
        /// <param name="invokeId">The unique identifier for this ADS request.</param>
        /// <param name="adsState">The requested ADS state (e.g., Config, Run, Stop).</param>
        /// <param name="deviceState">The requested device-specific state value.</param>
        /// <param name="data">Additional data associated with the control request (typically empty).</param>
        /// <param name="cancel">Cancellation token for async operations.</param>
        /// <returns>A task containing the result of the control operation (always success).</returns>
        /// <remarks>
        /// This method updates the internal state variables (_localAdsState and _localDeviceState)
        /// which can be queried by clients via OnReadDeviceStateAsync.
        /// </remarks>
        protected override Task<ResultAds> OnWriteControlAsync(AmsAddress sender, uint invokeId, AdsState adsState, ushort deviceState, ReadOnlyMemory<byte> data, CancellationToken cancel)
        {
            // Update the server's internal state to match the requested state
            _localAdsState = adsState;
            _localDeviceState = deviceState;

            ResultAds result = ResultAds.CreateSuccess();
            return Task.FromResult(result);
        }

        /// <summary>
        /// Handles requests to add a new device notification subscription.
        /// Creates a notification entry and assigns it a unique handle for future reference.
        /// </summary>
        /// <param name="sender">The AMS address of the client requesting the notification.</param>
        /// <param name="invokeId">The unique identifier for this ADS request.</param>
        /// <param name="indexGroup">The ADS index group to monitor for changes.</param>
        /// <param name="indexOffset">The ADS index offset within the specified group.</param>
        /// <param name="dataLength">The length of data to be sent in notifications.</param>
        /// <param name="receiver">The AMS address to receive notification callbacks.</param>
        /// <param name="settings">Notification settings including transmission mode and cycle time.</param>
        /// <param name="cancel">Cancellation token for async operations.</param>
        /// <returns>A task containing a unique notification handle for this subscription.</returns>
        /// <remarks>
        /// The notification handle is generated using thread-safe atomic increment (Interlocked.Increment)
        /// to ensure uniqueness across concurrent requests. The notification entry is stored in a
        /// ConcurrentDictionary for thread-safe access and can be removed via OnDeleteDeviceNotificationAsync.
        /// </remarks>
        protected override Task<ResultHandle> OnAddDeviceNotificationAsync(AmsAddress sender, uint invokeId, uint indexGroup, uint indexOffset, int dataLength, AmsAddress receiver, NotificationSettings settings, CancellationToken cancel)
        {
            // Create a new notification entry to track this subscription
            NotificationRequestEntry notEntry = new NotificationRequestEntry(receiver, indexGroup, indexOffset, dataLength, settings);

            // Generate a unique handle using thread-safe atomic increment
            uint handle = Interlocked.Increment(ref _currentNotificationHandle);

            // Store the notification entry in the thread-safe notification table
            _notificationTable.TryAdd(handle, notEntry);

            ResultHandle result = ResultHandle.CreateSuccess(handle);
            return Task.FromResult(result);
        }

        /// <summary>
        /// Handles requests to delete (unsubscribe from) an existing device notification.
        /// </summary>
        /// <param name="sender">The AMS address of the client requesting the deletion.</param>
        /// <param name="invokeId">The unique identifier for this ADS request.</param>
        /// <param name="hNotification">The notification handle to delete (returned from OnAddDeviceNotificationAsync).</param>
        /// <param name="cancel">Cancellation token for async operations.</param>
        /// <returns>A task containing the result - success if handle was found and deleted, error if handle was invalid.</returns>
        /// <remarks>
        /// This method uses ConcurrentDictionary.TryRemove for thread-safe removal of notification entries.
        /// If the handle is not found in the notification table, it returns DeviceNotifyHandleInvalid error code.
        /// </remarks>
        protected override Task<ResultAds> OnDeleteDeviceNotificationAsync(AmsAddress sender, uint invokeId, uint hNotification, CancellationToken cancel)
        {
            ResultAds result;

            // Attempt to remove the notification entry from the table (thread-safe operation)
            if (_notificationTable.TryRemove(hNotification, out _))
            {
                result = ResultAds.CreateSuccess();
            }
            else
            {
                // Notification handle not found in the table - return error
                result = ResultAds.CreateError(AdsErrorCode.DeviceNotifyHandleInvalid);
            }
            return Task.FromResult(result);
        }

        /// <summary>
        /// Handles incoming device notification data from clients.
        /// This method is called when a client sends notification samples to the server.
        /// </summary>
        /// <param name="sender">The AMS address of the client sending the notification.</param>
        /// <param name="stampHeaders">Array of notification samples containing timestamp and data.</param>
        /// <param name="cancel">Cancellation token for async operations.</param>
        /// <returns>A task containing the result of processing the notification (always success).</returns>
        /// <remarks>
        /// This implementation currently accepts notifications without processing them.
        /// Future implementations could handle notification data for bidirectional communication scenarios.
        /// </remarks>
        protected override Task<ResultAds> OnDeviceNotificationAsync(AmsAddress sender, NotificationSamplesStamp[] stampHeaders, CancellationToken cancel)
        {
            // Accept notification but no processing currently implemented
            return Task.FromResult(ResultAds.CreateSuccess());
        }

        /// <summary>
        /// Handles combined ADS read-write requests from clients.
        /// This operation atomically writes data and reads a response in a single transaction.
        /// </summary>
        /// <param name="sender">The AMS address of the client sending the request.</param>
        /// <param name="invokeId">The unique identifier for this ADS request.</param>
        /// <param name="indexGroup">The ADS index group for the operation.</param>
        /// <param name="indexOffset">The ADS index offset within the specified group.</param>
        /// <param name="readLength">The number of bytes to read in the response.</param>
        /// <param name="writeData">The data to write as part of the request.</param>
        /// <param name="cancel">Cancellation token for async operations.</param>
        /// <returns>A task containing the read-write operation result. Currently returns "service not supported" error.</returns>
        /// <remarks>
        /// This server does not currently support read-write operations. This advanced operation is typically
        /// used for atomic command-response patterns where the response depends on the written data.
        /// </remarks>
        protected override Task<ResultReadWriteBytes> OnReadWriteAsync(AmsAddress sender, uint invokeId, uint indexGroup, uint indexOffset, int readLength, ReadOnlyMemory<byte> writeData, CancellationToken cancel)
        {
            ResultReadWriteBytes result = ResultReadWriteBytes.CreateError(AdsErrorCode.DeviceServiceNotSupported);
            return Task.FromResult(result);
        }


        /* Overwritten indication methods of the AdsServer class for logging incoming request indications.
         * They are called upon incoming requests. These sample implemetations only add a log message
         * to the sample form.
         * 
         * In common, it is not necessary to overload the virtual Confirmation methods!
        */

#pragma warning disable CS0672 // Member overrides obsolete member
#pragma warning disable CS0618 // Type or member is obsolete
        protected override Task<AdsErrorCode> WriteIndicationAsync(AmsAddress sender, uint invokeId, uint indexGroup, uint indexOffset, ReadOnlyMemory<byte> writeData, CancellationToken cancel)
        {
            if (_serverLogger != null)
            {
                _serverLogger.LogWriteInd(sender, invokeId, indexGroup, indexOffset, writeData);
            }
            return base.WriteIndicationAsync(sender, invokeId, indexGroup, indexOffset, writeData, cancel);
        }

        protected override Task<AdsErrorCode> ReadIndicationAsync(AmsAddress rAddr, uint invokeId, uint indexGroup, uint indexOffset, int readLength, CancellationToken cancel)
        {
            if (_serverLogger != null)
            {
                _serverLogger.LogReadInd(rAddr, invokeId, indexOffset, readLength);
            }
            return base.ReadIndicationAsync(rAddr, invokeId, indexGroup, indexOffset, readLength, cancel);
        }

        protected override Task<AdsErrorCode> ReadDeviceStateIndicationAsync(AmsAddress rAddr, uint invokeId, CancellationToken cancel)
        {
            if (_serverLogger != null)
            {
                _serverLogger.LogReadStateInd(rAddr, invokeId);
            }
            return base.ReadDeviceInfoIndicationAsync(rAddr, invokeId, cancel);
        }

        protected override Task<AdsErrorCode> WriteControlIndicationAsync(AmsAddress rAddr, uint invokeId, AdsState adsState, ushort deviceState, ReadOnlyMemory<byte> data, CancellationToken cancel)
        {
            if (_serverLogger != null)
            {
                _serverLogger.LogWriteControlInd(rAddr, invokeId, adsState, deviceState, data);
            }
            return base.WriteControlIndicationAsync(rAddr, invokeId, adsState, deviceState, data, cancel);
        }

        protected override Task<AdsErrorCode> AddDeviceNotificationIndicationAsync(AmsAddress rAddr, uint invokeId, uint indexGroup, uint indexOffset, int length, NotificationSettings settings, CancellationToken cancel)
        {
            if (_serverLogger != null)
            {
                _serverLogger.LogAddDeviceNotificationInd(rAddr, invokeId, indexGroup, indexOffset, length, settings);
            }
            return base.AddDeviceNotificationIndicationAsync(rAddr, invokeId, indexGroup, indexOffset, length, settings, cancel);
        }

        protected override Task<AdsErrorCode> DeleteDeviceNotificationIndicationAsync(AmsAddress rAddr, uint invokeId, uint hNotification, CancellationToken cancel)
        {
            if (_serverLogger != null)
            {
                _serverLogger.LogDelDeviceNotificationInd(rAddr, invokeId, hNotification);
            }
            return base.DeleteDeviceNotificationIndicationAsync(rAddr, invokeId, hNotification, cancel);
        }

        protected override Task<AdsErrorCode> DeviceNotificationIndicationAsync(AmsAddress address, uint invokeId, uint numStampHeaders, NotificationSamplesStamp[] stampHeaders, CancellationToken cancel)

        {
            if (_serverLogger != null)
            {
                _serverLogger.LogDeviceNotificationInd(address, invokeId, numStampHeaders, stampHeaders);
                _serverLogger.Log("Received Device Notification Request");
            }
            return base.DeviceNotificationIndicationAsync(address, invokeId, numStampHeaders, stampHeaders, cancel);
        }

        protected override Task<AdsErrorCode> ReadWriteIndicationAsync(AmsAddress rAddr, uint invokeId, uint indexGroup, uint indexOffset, int readLength, ReadOnlyMemory<byte> writeData, CancellationToken cancel)
        {
            if (_serverLogger != null)
            {
                _serverLogger.LogReadWriteInd(rAddr, invokeId, indexGroup, indexOffset, readLength, writeData);
            }
            return base.ReadWriteIndicationAsync(rAddr, invokeId, indexGroup, indexOffset, readLength, writeData, cancel);
        }
#pragma warning restore CS0672 // Member overrides obsolete member
#pragma warning restore CS0618 // Type or member is obsolete

        /* Overwritten confirmation methods of the AdsServer class for the requests your ADS server
         * sends. They are called upon incoming responses. These sample implemetations only add a log message
         * to the sample form.
         * 
         * In common, it is not necessary to overload the virtual Confirmation methods!
         */
        protected override Task<AdsErrorCode> OnReadDeviceStateConfirmationAsync(AmsAddress rAddr, uint invokeId, AdsErrorCode result, AdsState adsState, ushort deviceState, CancellationToken cancel)

        {
            if (_serverLogger != null)
            {
                _serverLogger.LogReadStateCon(rAddr, invokeId, result, adsState, deviceState);
                _serverLogger.Log("Received Read State Confirmation");
            }

            return base.OnReadDeviceStateConfirmationAsync(rAddr, invokeId, result, adsState, deviceState, cancel);
        }

        protected override Task<AdsErrorCode> OnReadConfirmationAsync(AmsAddress sender, uint invokeId, AdsErrorCode result, ReadOnlyMemory<byte> readData, CancellationToken cancel)

        {
            if (_serverLogger != null)
            {
                _serverLogger.LogReadCon(sender, invokeId, result, readData);
                _serverLogger.Log("Received Read Confirmation");
            }

            return base.OnReadConfirmationAsync(sender, invokeId, result, readData, cancel);
        }

        protected override Task<AdsErrorCode> OnWriteConfirmationAsync(AmsAddress sender, uint invokeId, AdsErrorCode result, CancellationToken cancel)
        {
            if (_serverLogger != null)
            {
                _serverLogger.LogWriteCon(sender, invokeId, result);
                _serverLogger.Log("Received Write Confirmation");
            }

            return base.OnWriteConfirmationAsync(sender, invokeId, result, cancel);
        }

        protected override Task<AdsErrorCode> OnReadDeviceInfoConfirmationAsync(AmsAddress sender, uint invokeId, AdsErrorCode result, string name, AdsVersion version, CancellationToken cancel)
        {
            if (_serverLogger != null)
            {
                _serverLogger.LogReadDeviceInfoCon(sender, invokeId, result, name, version);
                _serverLogger.Log("Received Read Device Info Confirmation");
            }

            return base.OnReadDeviceInfoConfirmationAsync(sender, invokeId, result, name, version, cancel);
        }

        protected override Task<AdsErrorCode> OnWriteControlConfirmationAsync(AmsAddress rAddr, uint invokeId, AdsErrorCode result, CancellationToken cancel)

        {
            if (_serverLogger != null)
            {
                _serverLogger.LogReadDeviceInfoCon(rAddr, invokeId, result);
                _serverLogger.Log("Received Write Control Confirmation");
            }

            return base.OnWriteControlConfirmationAsync(rAddr, invokeId, result, cancel);
        }

        protected override Task<AdsErrorCode> OnAddDeviceNotificationConfirmationAsync(AmsAddress rAddr, uint invokeId, AdsErrorCode result, uint notificationHandle, CancellationToken cancel)

        {
            if (_serverLogger != null)
            {
                _serverLogger.LogAddDeviceNotificationCon(rAddr, invokeId, result, notificationHandle);
                _serverLogger.Log("Received Add Device Notification Confirmation. Notification handle: " + notificationHandle);
            }

            return base.OnAddDeviceNotificationConfirmationAsync(rAddr, invokeId, result, notificationHandle, cancel);
        }

        protected override Task<AdsErrorCode> OnDeleteDeviceNotificationConfirmationAsync(AmsAddress rAddr, uint invokeId, AdsErrorCode result, CancellationToken cancel)

        {
            if (_serverLogger != null)
            {
                _serverLogger.LogDelDeviceNotificationCon(rAddr, invokeId, result);
                _serverLogger.Log("Received Delete Device Notification Confirmation");
            }

            return base.OnDeleteDeviceNotificationConfirmationAsync(rAddr, invokeId, result, cancel);
        }

        protected override Task<AdsErrorCode> OnReadWriteConfirmationAsync(AmsAddress address, uint invokeId, AdsErrorCode result, ReadOnlyMemory<byte> readData, CancellationToken cancel)

        {
            if (_serverLogger != null)
            {
                _serverLogger.LogReadWriteCon(address, invokeId, result, readData);
                _serverLogger.Log("Received Read Write Confirmation");
            }
            return base.OnReadWriteConfirmationAsync(address, invokeId, result, readData, cancel);
        }

        /// <summary>
        /// Counter for generating unique invoke IDs for outgoing ADS requests.
        /// Automatically incremented with each Trigger* method call to ensure request uniqueness.
        /// </summary>
        /// <remarks>
        /// This field is not thread-safe. If multiple threads call Trigger methods concurrently,
        /// consider using Interlocked.Increment for atomic operations.
        /// </remarks>
        uint invokeId = 0;

        /// <summary>
        /// Triggers an ADS request to read device information from a target ADS device.
        /// </summary>
        /// <param name="target">The AMS address of the target device to query.</param>
        /// <param name="cancel">Cancellation token for async operations.</param>
        /// <returns>A task containing the error code result of the request operation.</returns>
        /// <remarks>
        /// This method automatically generates and increments the invoke ID for request tracking.
        /// The response will be received in OnReadDeviceInfoConfirmationAsync.
        /// </remarks>
        public Task<AdsErrorCode> TriggerReadDeviceInfoRequestAsync(AmsAddress target, CancellationToken cancel)
        {
            return base.ReadDeviceInfoRequestAsync(target, invokeId++, cancel);
        }

        /// <summary>
        /// Triggers an ADS read request to retrieve data from a target ADS device.
        /// </summary>
        /// <param name="target">The AMS address of the target device to read from.</param>
        /// <param name="indexGroup">The ADS index group specifying the data area to read.</param>
        /// <param name="indexOffset">The ADS index offset within the specified group.</param>
        /// <param name="readLength">The number of bytes to read.</param>
        /// <param name="cancel">Cancellation token for async operations.</param>
        /// <returns>A task containing the error code result of the request operation.</returns>
        /// <remarks>
        /// This method automatically generates and increments the invoke ID for request tracking.
        /// The response data will be received in OnReadConfirmationAsync.
        /// </remarks>
        public Task<AdsErrorCode> TriggerReadRequestAsync(AmsAddress target, uint indexGroup, uint indexOffset, int readLength, CancellationToken cancel)
        {
            return ReadRequestAsync(target, invokeId++, indexGroup, indexOffset, readLength, cancel);
        }

        /// <summary>
        /// Triggers an ADS write request to send data to a target ADS device.
        /// </summary>
        /// <param name="target">The AMS address of the target device to write to.</param>
        /// <param name="indexGroup">The ADS index group specifying the data area to write.</param>
        /// <param name="indexOffset">The ADS index offset within the specified group.</param>
        /// <param name="data">The data to write to the target device.</param>
        /// <param name="cancel">Cancellation token for async operations.</param>
        /// <returns>A task containing the error code result of the request operation.</returns>
        /// <remarks>
        /// This method automatically generates and increments the invoke ID for request tracking.
        /// The write confirmation will be received in OnWriteConfirmationAsync.
        /// </remarks>
        public Task<AdsErrorCode> TriggerWriteRequestAsync(AmsAddress target, uint indexGroup, uint indexOffset, ReadOnlyMemory<byte> data, CancellationToken cancel)
        {
            return WriteRequestAsync(target, invokeId++, indexGroup, indexOffset, data, cancel);
        }

        /// <summary>
        /// Triggers an ADS request to read the device state from a target ADS device.
        /// </summary>
        /// <param name="target">The AMS address of the target device to query.</param>
        /// <param name="cancel">Cancellation token for async operations.</param>
        /// <returns>A task containing the error code result of the request operation.</returns>
        /// <remarks>
        /// This method automatically generates and increments the invoke ID for request tracking.
        /// The state information will be received in OnReadDeviceStateConfirmationAsync.
        /// </remarks>
        public Task<AdsErrorCode> TriggerReadStateRequestAsync(AmsAddress target, CancellationToken cancel)
        {
            return ReadDeviceStateRequestAsync(target, invokeId++, cancel);
        }

        /// <summary>
        /// Triggers an ADS write control request to change the state of a target ADS device.
        /// </summary>
        /// <param name="target">The AMS address of the target device to control.</param>
        /// <param name="state">The requested ADS state (e.g., Config, Run, Stop).</param>
        /// <param name="deviceState">The requested device-specific state value.</param>
        /// <param name="data">Additional control data to send with the request.</param>
        /// <param name="cancel">Cancellation token for async operations.</param>
        /// <returns>A task containing the error code result of the request operation.</returns>
        /// <remarks>
        /// This method automatically generates and increments the invoke ID for request tracking.
        /// The control confirmation will be received in OnWriteControlConfirmationAsync.
        /// </remarks>
        public Task<AdsErrorCode> TriggerWriteControlRequestAsync(AmsAddress target, AdsState state, ushort deviceState, ReadOnlyMemory<byte> data, CancellationToken cancel)
        {
            return WriteControlRequestAsync(target, invokeId++, state, deviceState, data, cancel);
        }

        /// <summary>
        /// Triggers an ADS request to add a device notification subscription on a target ADS device.
        /// </summary>
        /// <param name="target">The AMS address of the target device to monitor.</param>
        /// <param name="indexGroup">The ADS index group to monitor for changes.</param>
        /// <param name="indexOffset">The ADS index offset within the specified group.</param>
        /// <param name="dataLength">The length of data to be sent in notifications.</param>
        /// <param name="settings">Notification settings including transmission mode and cycle time.</param>
        /// <param name="cancel">Cancellation token for async operations.</param>
        /// <returns>A task containing the error code result of the request operation.</returns>
        /// <remarks>
        /// This method automatically generates and increments the invoke ID for request tracking.
        /// The notification handle will be received in OnAddDeviceNotificationConfirmationAsync.
        /// Once subscribed, notification data will arrive via OnDeviceNotificationAsync.
        /// </remarks>
        public Task<AdsErrorCode> TriggerAddDeviceNotificationRequestAsync(AmsAddress target, uint indexGroup, uint indexOffset, int dataLength, NotificationSettings settings, CancellationToken cancel)
        {
            return AddDeviceNotificationRequestAsync(target, invokeId++, indexGroup, indexOffset, dataLength, settings, cancel);
        }

        /// <summary>
        /// Triggers an ADS request to delete (unsubscribe from) a device notification on a target ADS device.
        /// </summary>
        /// <param name="target">The AMS address of the target device.</param>
        /// <param name="handle">The notification handle to delete (received from TriggerAddDeviceNotificationRequestAsync).</param>
        /// <param name="cancel">Cancellation token for async operations.</param>
        /// <returns>A task containing the error code result of the request operation.</returns>
        /// <remarks>
        /// This method automatically generates and increments the invoke ID for request tracking.
        /// The deletion confirmation will be received in OnDeleteDeviceNotificationConfirmationAsync.
        /// </remarks>
        public Task<AdsErrorCode> TriggerDeleteDeviceNotificationRequestAsync(AmsAddress target, uint handle, CancellationToken cancel)
        {
            return DeleteDeviceNotificationRequestAsync(target, invokeId++, handle, cancel);
        }

        /// <summary>
        /// Triggers an ADS combined read-write request to a target ADS device.
        /// This operation atomically writes data and reads a response in a single transaction.
        /// </summary>
        /// <param name="target">The AMS address of the target device.</param>
        /// <param name="indexGroup">The ADS index group for the operation.</param>
        /// <param name="indexOffset">The ADS index offset within the specified group.</param>
        /// <param name="readLength">The number of bytes to read in the response.</param>
        /// <param name="data">The data to write as part of the request.</param>
        /// <param name="cancel">Cancellation token for async operations.</param>
        /// <returns>A task containing the error code result of the request operation.</returns>
        /// <remarks>
        /// This method automatically generates and increments the invoke ID for request tracking.
        /// The read-write result will be received in OnReadWriteConfirmationAsync.
        /// This operation is useful for atomic command-response patterns.
        /// </remarks>
        public Task<AdsErrorCode> TriggerReadWriteRequestAsync(AmsAddress target, uint indexGroup, uint indexOffset, int readLength, ReadOnlyMemory<byte> data, CancellationToken cancel)
        {
            return ReadWriteRequestAsync(target, invokeId++, indexGroup, indexOffset, readLength, data, cancel);
        }
    }
}
