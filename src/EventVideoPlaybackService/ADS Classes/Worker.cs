using EventVideoPlaybackService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TwinCAT.Ads;
using TwinCAT.Ads.Server;

namespace EventVideoPlaybackService
{
    /// <summary>
    /// Configuration data for the EventVideoPlaybackService.
    /// This class defines all configurable parameters that control the ADS server behavior,
    /// video codec settings, and file management policies.
    /// </summary>
    /// <remarks>
    /// Configuration is loaded from EventVideoPlaybackService.config.json located next to the
    /// service executable. Any missing or invalid value falls back to a per-field default so a
    /// partial file cannot leave the service running on zeroed settings.
    /// </remarks>
    class ConfigData
    {
        /// <summary>
        /// Gets or sets the FourCC code for the video codec used for video encoding.
        /// </summary>
        /// <value>
        /// A 4-character code identifying the video codec. Common values:
        /// <list type="bullet">
        /// <item><description>"avc1" - H.264/AVC (default, recommended for broad compatibility)</description></item>
        /// <item><description>"mp4v" - MPEG-4 Part 2</description></item>
        /// <item><description>"h264" - H.264 alternative identifier</description></item>
        /// </list>
        /// </value>
        public string CodecFourCC { get; set; } = "";

        /// <summary>
        /// Gets or sets the number of days to retain video files before automatic deletion.
        /// </summary>
        /// <value>
        /// The retention period in days. Videos older than this are deleted during maintenance cycles.
        /// </value>
        public double VideoDeleteTime { get; set; }

        /// <summary>
        /// Gets or sets the ADS port number for the server to listen on.
        /// </summary>
        /// <value>
        /// The ADS port number. Must be in one of the customer-reserved ranges:
        /// <list type="bullet">
        /// <item><description>25000-25999 (CUSTOMER_FIRST to CUSTOMER_LAST)</description></item>
        /// <item><description>26000-26999 (CUSTOMERPRIVATE_FIRST to CUSTOMERPRIVATE_LAST)</description></item>
        /// </list>
        /// Must match the PLC library parameter ADS_PORT_FOR_IMAGE_TO_VIDEO (26129 by default).
        /// </value>
        /// <remarks>
        /// See <see href="https://infosys.beckhoff.com/content/1033/tc3_ads.net/9408352011.html?id=1801810347107555608">Beckhoff ADS Port Documentation</see> for port range restrictions.
        /// </remarks>
        public ushort AdsPort { get; set; }

        /// <summary>
        /// Gets or sets the maximum total size limit for video files in the storage folder.
        /// </summary>
        /// <value>
        /// The maximum folder size in megabytes (MB). When this limit is exceeded,
        /// oldest videos are deleted until total size is below the limit.
        /// </value>
        public ulong MaxFolderSize { get; set; }

        /// <summary>
        /// Gets or sets the directory that video requests are confined to.
        /// </summary>
        /// <value>
        /// When set to a non-empty path, the service rejects any request whose image source or
        /// video output path resolves outside this directory (and rejects UNC paths). When empty
        /// (the default) no confinement is applied and the service accepts any local path the PLC
        /// sends. This is a defensive limit on the recursive image-directory delete the service
        /// performs; set it to the video output root the PLC library writes to.
        /// </value>
        public string VideoRootDirectory { get; set; } = "";
    }

    /// <summary>
    /// Hosted background service that loads configuration, starts the ADS image-to-video server,
    /// and keeps it running until the host is stopped.
    /// </summary>
    public sealed class WindowsBackgroundService(ILogger<WindowsBackgroundService> Logger) : BackgroundService
    {
        // Canonical default set. Every fallback path resolves to these values so the three
        // divergent default sets that used to exist cannot reappear.
        internal const string DefaultCodecFourCC = "avc1";
        internal const double DefaultVideoDeleteTime = 1;    // days; matches the shipped config file
        internal const ushort DefaultAdsPort = 26129;        // must match the PLC GVL ADS_PORT_FOR_IMAGE_TO_VIDEO
        internal const ulong DefaultMaxFolderSize = 250;     // MB
        internal const string ConfigFileName = "EventVideoPlaybackService.config.json";

        private static readonly JsonSerializerOptions ConfigJsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Loads the service configuration from the file located next to the service executable,
        /// applying the canonical per-field default for any value that is missing or out of range.
        /// </summary>
        /// <param name="baseDirectory">The directory to resolve the config file against (the executable directory).</param>
        /// <param name="logger">Logger used to report fallback to defaults.</param>
        /// <returns>A fully populated <see cref="ConfigData"/> with no zeroed or empty required fields.</returns>
        internal static ConfigData LoadConfig(string baseDirectory, ILogger logger)
        {
            string configPath = Path.Combine(baseDirectory, ConfigFileName);
            ConfigData config;

            try
            {
                string json = File.ReadAllText(configPath);
                config = JsonSerializer.Deserialize<ConfigData>(json, ConfigJsonOptions) ?? new ConfigData();
            }
            catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException)
            {
                logger.LogWarning("Configuration file not found at {ConfigPath}; using defaults.", configPath);
                config = new ConfigData();
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Configuration file at {ConfigPath} is malformed; using defaults.", configPath);
                config = new ConfigData();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // A locked or permission blocked file must not stop the service: the escape
                // used to reach the generic handler in ExecuteAsync, which exits the process,
                // and the service manager then restarted it into the same locked file forever.
                logger.LogError(ex, "Configuration file at {ConfigPath} could not be read; using defaults.", configPath);
                config = new ConfigData();
            }

            // Per-field validation. A partial or mis-cased file used to leave AdsPort/MaxFolderSize/
            // VideoDeleteTime at zero, which routes to a dynamic port and deletes every video; apply
            // the canonical default to any field that is missing or non-positive instead.
            if (string.IsNullOrWhiteSpace(config.CodecFourCC)) config.CodecFourCC = DefaultCodecFourCC;
            if (config.VideoDeleteTime <= 0) config.VideoDeleteTime = DefaultVideoDeleteTime;
            if (config.AdsPort == 0) config.AdsPort = DefaultAdsPort;
            if (config.MaxFolderSize == 0) config.MaxFolderSize = DefaultMaxFolderSize;
            config.VideoRootDirectory ??= "";

            return config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            AdsImageToVideoServer? adsServer = null;
            try
            {
                // User server ports must be in
                // AmsPortRange.CUSTOMER_FIRST (25000) <= PORT <= AmsPort.CUSTOMER_LAST (25999)
                // or
                // AmsPortRange.CUSTOMERPRIVATE_FIRST (26000) <= PORT <= AmsPort.CUSTOMERPRIVATE_LAST (26999)
                // so they do not conflict with Beckhoff prereserved servers. This presumes a
                // TwinCAT router is already running; if it is not, ConnectServerAndWaitAsync faults
                // and the process exits non-zero so the Windows service manager restarts it.

                // Config is resolved against the executable directory, not the current working
                // directory (a Windows service starts with CWD = System32).
                ConfigData config = LoadConfig(AppContext.BaseDirectory, Logger);
                Logger.LogInformation(
                    "EventVideoPlayback service starting. Codec={Codec}, AdsPort={Port}, RetentionDays={Retention}, MaxFolderSizeMB={MaxSize}, VideoRootDirectory={Root}",
                    config.CodecFourCC, config.AdsPort, config.VideoDeleteTime, config.MaxFolderSize,
                    string.IsNullOrEmpty(config.VideoRootDirectory) ? "(unrestricted)" : config.VideoRootDirectory);

                // Initialize and start the ADS server with the loaded configuration.
                adsServer = new AdsImageToVideoServer(config.AdsPort, "AdsImageToVideoAdsServer", Logger, config.VideoDeleteTime, config.CodecFourCC, config.MaxFolderSize, config.VideoRootDirectory);

                // Await the server for the service lifetime. This yields, so host startup completes.
                // ConnectServerAndWaitAsync does not throw on failure: it returns an error code when
                // the TwinCAT router is unavailable at startup, and returns when the connection is
                // later torn down (for example a router restart). Capture the result rather than
                // discarding it.
                AdsErrorCode connectResult = AdsErrorCode.NoError;
                try
                {
                    connectResult = await adsServer.ConnectServerAndWaitAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected when the host is stopping.
                }

                // If the host did not ask us to stop, the server ended unexpectedly (no router, or a
                // dropped connection). Throw so the outer handler exits non-zero and the Windows
                // service manager applies its restart policy, instead of the process lingering with a
                // disposed, non-functional server (a zombie).
                if (!stoppingToken.IsCancellationRequested)
                {
                    throw new InvalidOperationException($"ADS server connection ended unexpectedly (result={connectResult}).");
                }

                Logger.LogInformation("ADS server stopped; shutting down.");
                adsServer.Disconnect();
                adsServer.Dispose();
            }
            catch (OperationCanceledException)
            {
                // When the stopping token is canceled (for example from services.msc), this is
                // expected and must not exit with a non-zero code.
                adsServer?.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "EventVideoPlayback service terminated unexpectedly.");
                adsServer?.Dispose();

                // Terminate with a non-zero exit code so the Windows service manager can apply its
                // configured recovery options (restart) instead of leaving a zombie service.
                Environment.Exit(1);
            }
        }
    }
}
