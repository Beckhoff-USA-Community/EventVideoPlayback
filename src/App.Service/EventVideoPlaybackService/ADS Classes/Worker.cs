using EventVideoPlaybackService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TwinCAT.Ads.Server;

namespace EventVideoPlaybackService
{
    /// <summary>
    /// Configuration data for the EventVideoPlaybackService.
    /// This class defines all configurable parameters that control the ADS server behavior,
    /// video codec settings, and file management policies.
    /// </summary>
    /// <remarks>
    /// Configuration is typically loaded from EventVideoPlaybackService.config.json.
    /// If the configuration file is missing or invalid, default values are used as fallback.
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
        /// The retention period in days. Videos older than this will be deleted during maintenance cycles.
        /// Default fallback value is 5 or 10 days depending on error condition.
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
        /// Default fallback value is typically 26128 or 26129.
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
        /// Default fallback value is 250 MB.
        /// </value>
        public ulong MaxFolderSize { get; set; }
    }
    public sealed class WindowsBackgroundService(ILogger<WindowsBackgroundService> Logger) : BackgroundService
    {

        AdsImageToVideoServer? AdsServer;
        ConfigData config = new ConfigData();
        static DateTime lastTime = DateTime.MinValue; // Initialize to a default value

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // User Server Ports must be in between
                // AmsPortRange.CUSTOMER_FIRST (25000) <= PORT <= AmsPort.CUSTOMER_LAST (25999)
                // or
                // AmsPortRange.CUSTOMERPRIVATE_FIRST (26000) <= PORT <= AmsPort.CUSTOMERPRIVATE_LAST (26999)
                // to not conflict with Beckhoff prereserved servers!
                // see https://infosys.beckhoff.com/content/1033/tc3_ads.net/9408352011.html?id=1801810347107555608

                // This presumes that a TwinCAT Router is already running!

                // Load configuration from JSON file in the current working directory
                try
                {
                    string jsonFile = "EventVideoPlaybackService.config.json";
                    string jsonString = File.ReadAllText(jsonFile);
                    if (jsonString is not null)
                    {
                        config = JsonSerializer.Deserialize<ConfigData>(jsonString) ?? new ConfigData();
                    }

                    // Apply default values if configuration was empty or missing required properties
                    if (config.CodecFourCC == "")
                    {
                        config.CodecFourCC = "avc1";
                        config.VideoDeleteTime = 10;
                        config.AdsPort = 26128;
                        config.MaxFolderSize = 250;
                    }

                }
                catch (FileNotFoundException)
                {
                    // Configuration file not found - use fallback defaults with slightly different port to avoid conflicts
                    Logger.LogError("config file not found");
                    config.CodecFourCC = "avc1";
                    config.VideoDeleteTime = 5;
                    config.AdsPort = 26129;
                    config.MaxFolderSize = 250;
                }
                catch (JsonException e)
                {
                    // Configuration file is malformed - use fallback defaults
                    Logger.LogError("Error Parsing config file: " + e.Message);
                    config.CodecFourCC = "avc1";
                    config.VideoDeleteTime = 5;
                    config.AdsPort = 26129;
                    config.MaxFolderSize = 250;
                }


                // Initialize and start the ADS server with the loaded configuration
                AdsServer = new AdsImageToVideoServer(config.AdsPort, "AdsImageToVideoAdsServer", Logger, config.VideoDeleteTime, config.CodecFourCC,config.MaxFolderSize);
                Task[] serverTasks = new Task[1];
                serverTasks[0] = AdsServer.ConnectServerAndWaitAsync(stoppingToken);

                // Keep the service alive until cancellation is requested (e.g., service stop command)
                // The 1-second sleep interval prevents busy-waiting while allowing responsive shutdown
                while (!stoppingToken.IsCancellationRequested)
                {
                    Thread.Sleep(1000);
                }

                // Graceful shutdown: Wait for the ADS server connection task to complete
                Task shutdownTask = Task.Run(async () =>
                {
                    await Task.WhenAll(serverTasks);
                    Logger.LogWarning("All AdsServers closed down.!");
                });

                // Disconnect and dispose of the ADS server resources
                AdsServer.Disconnect();
                AdsServer?.Dispose();

                // Ensure all cleanup tasks complete before exiting
                await shutdownTask;


            }
            catch (OperationCanceledException)
            {
                Logger.LogWarning("OperationCanceledExeption");
                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{Message}", ex.Message);

                // Terminates this process and returns an exit code to the operating system.
                // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
                // performs one of two scenarios:
                // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
                // 2. When set to "StopHost": will cleanly stop the host, and log errors.
                //
                // In order for the Windows Service Management system to leverage configured
                // recovery options, we need to terminate the process with a non-zero exit code.
                Environment.Exit(1);
            }
        }
    }
}
