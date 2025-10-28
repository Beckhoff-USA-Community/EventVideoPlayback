using EventVideoPlaybackService;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;

/// <summary>
/// Entry point for the Event Video Playback Windows Service application.
/// This service processes images from TwinCAT PLC systems and converts them into video files
/// via the ADS (Automation Device Specification) protocol.
/// </summary>
internal class Program
{
    /// <summary>
    /// Main entry point for the Windows Service application.
    /// Configures the host builder with Windows Service support and event logging,
    /// then starts the WindowsBackgroundService to handle ADS communication and video processing.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    private static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        // Configure the application to run as a Windows Service
        builder.Services.AddWindowsService(options =>
        {
            options.ServiceName = "EventVideoPlayback";
        });

        // Register Windows Event Log provider on Windows platforms for centralized logging
        if (OperatingSystem.IsWindows())
        {
            LoggerProviderOptions.RegisterProviderOptions<
            EventLogSettings, EventLogLoggerProvider>(builder.Services);
        }

        // Register the background service that handles ADS communication and video processing
        builder.Services.AddHostedService<WindowsBackgroundService>();

        IHost host = builder.Build();
        host.Run();
    }
}