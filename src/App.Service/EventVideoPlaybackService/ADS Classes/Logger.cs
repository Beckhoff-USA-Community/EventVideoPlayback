using Microsoft.Extensions.Logging;
using System;
using TwinCAT.Ads;
using TwinCAT.Ads.Server;

namespace EventVideoPlaybackService
{
    /// <summary>
    /// Interface for logging ADS (Automation Device Specification) protocol communications.
    /// Provides methods to log both indications (incoming requests) and confirmations (responses)
    /// for all standard ADS services including read, write, notifications, and control operations.
    /// </summary>
    public interface IServerLogger
    {
        /// <summary>
        /// Logs a custom message string.
        /// </summary>
        /// <param name="str">The message to log.</param>
        void Log(string str);

        /// <summary>
        /// Logs an incoming write indication request.
        /// </summary>
        /// <param name="rAddr">The AMS address of the sender.</param>
        /// <param name="invokeId">The unique identifier for this ADS request.</param>
        /// <param name="indexGroup">The ADS index group identifying the data area.</param>
        /// <param name="indexOffset">The ADS index offset within the index group.</param>
        /// <param name="writeData">The data being written.</param>
        void LogWriteInd(AmsAddress rAddr, uint invokeId, uint indexGroup, uint indexOffset, ReadOnlyMemory<byte> writeData);

        /// <summary>
        /// Logs an incoming read indication request.
        /// </summary>
        /// <param name="rAddr">The AMS address of the sender.</param>
        /// <param name="invokeId">The unique identifier for this ADS request.</param>
        /// <param name="indexOffset">The ADS index offset to read from.</param>
        /// <param name="cbLength">The number of bytes requested to be read.</param>
        void LogReadInd(AmsAddress rAddr, uint invokeId, uint indexOffset, int cbLength);

        /// <summary>
        /// Logs an incoming read state indication request.
        /// </summary>
        /// <param name="rAddr">The AMS address of the sender.</param>
        /// <param name="invokeId">The unique identifier for this ADS request.</param>
        void LogReadStateInd(AmsAddress rAddr, uint invokeId);

        /// <summary>
        /// Logs an incoming write control indication request.
        /// </summary>
        /// <param name="rAddr">The AMS address of the sender.</param>
        /// <param name="invokeId">The unique identifier for this ADS request.</param>
        /// <param name="adsState">The ADS state to set (e.g., Run, Stop, Config).</param>
        /// <param name="deviceState">The device-specific state value.</param>
        /// <param name="data">Additional control data.</param>
        void LogWriteControlInd(AmsAddress rAddr, uint invokeId, AdsState adsState, ushort deviceState, ReadOnlyMemory<byte> data);

        /// <summary>
        /// Logs an incoming add device notification indication request.
        /// </summary>
        /// <param name="rAddr">The AMS address of the sender.</param>
        /// <param name="invokeId">The unique identifier for this ADS request.</param>
        /// <param name="indexGroup">The ADS index group to monitor.</param>
        /// <param name="indexOffset">The ADS index offset to monitor.</param>
        /// <param name="cbLength">The number of bytes to include in notifications.</param>
        /// <param name="notificationSettings">The notification settings (cycle time, transmission mode).</param>
        void LogAddDeviceNotificationInd(AmsAddress rAddr, uint invokeId, uint indexGroup, uint indexOffset, int cbLength, NotificationSettings notificationSettings);

        /// <summary>
        /// Logs an incoming delete device notification indication request.
        /// </summary>
        /// <param name="rAddr">The AMS address of the sender.</param>
        /// <param name="invokeId">The unique identifier for this ADS request.</param>
        /// <param name="hNotification">The notification handle to delete.</param>
        void LogDelDeviceNotificationInd(AmsAddress rAddr, uint invokeId, uint hNotification);

        /// <summary>
        /// Logs an incoming device notification indication (data change notification).
        /// </summary>
        /// <param name="rAddr">The AMS address of the sender.</param>
        /// <param name="invokeId">The unique identifier for this ADS request.</param>
        /// <param name="numStapHeaders">The number of notification stamp headers.</param>
        /// <param name="stampHeaders">The array of notification samples with timestamps.</param>
        void LogDeviceNotificationInd(AmsAddress rAddr, uint invokeId, uint numStapHeaders, NotificationSamplesStamp[] stampHeaders);

        /// <summary>
        /// Logs an incoming read/write indication request (combined read and write operation).
        /// </summary>
        /// <param name="rAddr">The AMS address of the sender.</param>
        /// <param name="invokeId">The unique identifier for this ADS request.</param>
        /// <param name="indexGroup">The ADS index group.</param>
        /// <param name="indexOffset">The ADS index offset.</param>
        /// <param name="cbReadLength">The number of bytes to read.</param>
        /// <param name="writeData">The data to write.</param>
        void LogReadWriteInd(AmsAddress rAddr, uint invokeId, uint indexGroup, uint indexOffset, int cbReadLength, ReadOnlyMemory<byte> writeData);

        /// <summary>
        /// Logs an outgoing read state confirmation (response).
        /// </summary>
        /// <param name="rAddr">The AMS address of the recipient.</param>
        /// <param name="invokeId">The unique identifier for this ADS response.</param>
        /// <param name="result">The result code of the operation.</param>
        /// <param name="adsState">The current ADS state.</param>
        /// <param name="deviceState">The current device state.</param>
        void LogReadStateCon(AmsAddress rAddr, uint invokeId, AdsErrorCode result, AdsState adsState, ushort deviceState);

        /// <summary>
        /// Logs an outgoing read confirmation (response).
        /// </summary>
        /// <param name="rAddr">The AMS address of the recipient.</param>
        /// <param name="invokeId">The unique identifier for this ADS response.</param>
        /// <param name="result">The result code of the operation.</param>
        /// <param name="readData">The data that was read.</param>
        void LogReadCon(AmsAddress rAddr, uint invokeId, AdsErrorCode result, ReadOnlyMemory<byte> readData);

        /// <summary>
        /// Logs an outgoing write confirmation (response).
        /// </summary>
        /// <param name="rAddr">The AMS address of the recipient.</param>
        /// <param name="invokeId">The unique identifier for this ADS response.</param>
        /// <param name="result">The result code of the operation.</param>
        void LogWriteCon(AmsAddress rAddr, uint invokeId, AdsErrorCode result);

        /// <summary>
        /// Logs an outgoing read device info confirmation (response) with device details.
        /// </summary>
        /// <param name="rAddr">The AMS address of the recipient.</param>
        /// <param name="invokeId">The unique identifier for this ADS response.</param>
        /// <param name="result">The result code of the operation.</param>
        /// <param name="name">The device name.</param>
        /// <param name="version">The device version information.</param>
        void LogReadDeviceInfoCon(AmsAddress rAddr, uint invokeId, AdsErrorCode result, string name, AdsVersion version);

        /// <summary>
        /// Logs an outgoing read device info confirmation (response) without device details.
        /// </summary>
        /// <param name="rAddr">The AMS address of the recipient.</param>
        /// <param name="invokeId">The unique identifier for this ADS response.</param>
        /// <param name="result">The result code of the operation.</param>
        void LogReadDeviceInfoCon(AmsAddress rAddr, uint invokeId, AdsErrorCode result);

        /// <summary>
        /// Logs an outgoing add device notification confirmation (response).
        /// </summary>
        /// <param name="rAddr">The AMS address of the recipient.</param>
        /// <param name="invokeId">The unique identifier for this ADS response.</param>
        /// <param name="result">The result code of the operation.</param>
        /// <param name="notificationHandle">The assigned notification handle.</param>
        void LogAddDeviceNotificationCon(AmsAddress rAddr, uint invokeId, AdsErrorCode result, uint notificationHandle);

        /// <summary>
        /// Logs an outgoing delete device notification confirmation (response).
        /// </summary>
        /// <param name="rAddr">The AMS address of the recipient.</param>
        /// <param name="invokeId">The unique identifier for this ADS response.</param>
        /// <param name="result">The result code of the operation.</param>
        void LogDelDeviceNotificationCon(AmsAddress rAddr, uint invokeId, AdsErrorCode result);

        /// <summary>
        /// Logs an outgoing read/write confirmation (response).
        /// </summary>
        /// <param name="rAddr">The AMS address of the recipient.</param>
        /// <param name="invokeId">The unique identifier for this ADS response.</param>
        /// <param name="result">The result code of the operation.</param>
        /// <param name="readData">The data that was read.</param>
        void LogReadWriteCon(AmsAddress rAddr, uint invokeId, AdsErrorCode result, ReadOnlyMemory<byte> readData);

        /// <summary>
        /// Logs an incoming read device info indication request.
        /// </summary>
        /// <param name="rAddr">The AMS address of the sender.</param>
        /// <param name="invokeId">The unique identifier for this ADS request.</param>
        void LogReadDeviceInfoInd(AmsAddress rAddr, uint invokeId);

        /// <summary>
        /// Gets the underlying ILogger instance used for logging.
        /// </summary>
        ILogger Logger { get; }
    }

    /// <summary>
    /// Base implementation of the IServerLogger interface that provides no-op (empty) implementations
    /// for all logging methods. Derived classes can override only the methods they need to log.
    /// </summary>
    public class ServerLoggerBase : IServerLogger
    {
        readonly ILogger _logger;

        /// <summary>
        /// Gets the underlying ILogger instance used for logging.
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
        }

        /// <summary>
        /// Initializes a new instance of the ServerLoggerBase class with the specified logger.
        /// </summary>
        /// <param name="logger">The ILogger instance to use for logging operations.</param>
        protected ServerLoggerBase(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets or sets the server notification handle. This property is a no-op in the base implementation.
        /// </summary>
        public virtual uint ServerNotificationHandle
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// Logs a custom message string. This method is a no-op in the base implementation.
        /// </summary>
        /// <param name="str">The message to log.</param>
        public virtual void Log(string str)
        {
        }

        public virtual void LogAddDeviceNotificationCon(AmsAddress rAddr, uint invokeId, AdsErrorCode result, uint notificationHandle)
        {
        }

        public virtual void LogAddDeviceNotificationInd(AmsAddress rAddr, uint invokeId, uint indexGroup, uint indexOffset, int cbLength, NotificationSettings settings)
        {
        }

        public virtual void LogDelDeviceNotificationCon(AmsAddress rAddr, uint invokeId, AdsErrorCode result)
        {
        }

        public virtual void LogDelDeviceNotificationInd(AmsAddress sender, uint invokeId, uint hNotification)
        {
        }

        public virtual void LogDeviceNotificationInd(AmsAddress sender, uint invokeId, uint numStapHeaders, NotificationSamplesStamp[] stampHeaders)
        {
        }

        public virtual void LogReadCon(AmsAddress sender, uint invokeId, AdsErrorCode result, ReadOnlyMemory<byte> readData)
        {
        }

        public virtual void LogReadDeviceInfoCon(AmsAddress sender, uint invokeId, AdsErrorCode result, string name, AdsVersion version)
        {
        }

        public virtual void LogReadDeviceInfoCon(AmsAddress sender, uint invokeId, AdsErrorCode result)
        {
        }

        public virtual void LogReadDeviceInfoInd(AmsAddress sender, uint invokeId)
        {
        }

        public virtual void LogReadInd(AmsAddress sender, uint invokeId, uint indexOffset, int cbLength)
        {
        }

        public virtual void LogReadStateCon(AmsAddress sender, uint invokeId, AdsErrorCode result, AdsState adsState, ushort deviceState)
        {
        }

        public virtual void LogReadStateInd(AmsAddress sender, uint invokeId)
        {
        }

        public virtual void LogReadWriteCon(AmsAddress sender, uint invokeId, AdsErrorCode result, ReadOnlyMemory<byte> readData)
        {
        }

        public virtual void LogReadWriteInd(AmsAddress sender, uint invokeId, uint indexGroup, uint indexOffset, int cbReadLength, ReadOnlyMemory<byte> writeData)
        {
        }

        public virtual void LogWriteCon(AmsAddress sender, uint invokeId, AdsErrorCode result)
        {
        }

        public virtual void LogWriteControlInd(AmsAddress sender, uint invokeId, AdsState adsState, ushort deviceState, ReadOnlyMemory<byte> data)
        {
        }

        public virtual void LogWriteInd(AmsAddress sender, uint invokeId, uint indexGroup, uint indexOffset, ReadOnlyMemory<byte> writeData)
        {
        }
    }

    /// <summary>
    /// Concrete implementation of IServerLogger that logs all ADS protocol communications
    /// at the Debug level. This class provides detailed logging for troubleshooting
    /// ADS communication between clients and the server.
    /// </summary>
    public class ServerLogger : ServerLoggerBase
    {
        /// <summary>
        /// Initializes a new instance of the ServerLogger class with the specified logger.
        /// </summary>
        /// <param name="logger">The ILogger instance to use for logging operations.</param>
        public ServerLogger(ILogger logger) : base(logger)
        {
        }

        public override void LogAddDeviceNotificationCon(AmsAddress rAddr, uint invokeId, AdsErrorCode result, uint notificationHandle)
        {
            Logger.LogDebug($"AddDeviceNotificationCon(Address: {rAddr}, InvokeId: {invokeId}, Result: {result}, Handle: {notificationHandle}");
        }

        public override void LogAddDeviceNotificationInd(AmsAddress sender, uint invokeId, uint indexGroup, uint indexOffset, int cbLength, NotificationSettings settings)
        {
            Logger.LogDebug($"AddDeviceNotificationInd(Address: {sender}, InvokeId: {invokeId}, IG: {indexGroup}, IO: {indexOffset}, Len: {cbLength})");
        }

        public override void LogDelDeviceNotificationCon(AmsAddress sender, uint invokeId, AdsErrorCode result)
        {
            Logger.LogDebug($"DelDeviceNotificationCon(Address: {sender}, InvokeId: {invokeId}, Result: {result})");
        }

        public override void LogDelDeviceNotificationInd(AmsAddress sender, uint invokeId, uint hNotification)
        {
            Logger.LogDebug($"DelDeviceNotificationInd(Address: {sender}, InvokeId: {invokeId}, Handle: {hNotification})");
        }

        public override void LogDeviceNotificationInd(AmsAddress sender, uint invokeId, uint numStapHeaders, NotificationSamplesStamp[] stampHeaders)
        {
            Logger.LogDebug($"DeviceNotificationInd(Address: {sender}, InvokeId: {invokeId}, Headers: {numStapHeaders})");
        }

        public override void LogReadCon(AmsAddress sender, uint invokeId, AdsErrorCode result, ReadOnlyMemory<byte> readData)
        {
            Logger.LogDebug($"ReadCon(Address: {sender}, InvokeId: {invokeId}, Result: {result}, cbLength: {readData.Length}");
        }

        public override void LogReadDeviceInfoCon(AmsAddress sender, uint invokeId, AdsErrorCode result, string name, AdsVersion version)
        {
            Logger.LogDebug($"ReadDeviceInfoCon(Address: {sender}, InvokeId: {invokeId}, Result: {result}, Name: {name}, Version: {version})");
        }

        public override void LogReadDeviceInfoCon(AmsAddress sender, uint invokeId, AdsErrorCode result)
        {
            Logger.LogDebug($"ReadDeviceInfoCon(Address: {sender}, InvokeId: {invokeId}, Result: {result})");
        }

        public override void LogReadDeviceInfoInd(AmsAddress sender, uint invokeId)
        {
            Logger.LogDebug($"ReadDeviceInfoInd(Address: {sender}, InvokeId: {invokeId})");
        }

        public override void LogReadInd(AmsAddress sender, uint invokeId, uint indexOffset, int cbLength)
        {
            Logger.LogDebug($"ReadInd(Address: {sender}, InvokeId: {invokeId}, IO: {indexOffset}, Length: {cbLength})");
        }

        public override void LogReadStateCon(AmsAddress sender, uint invokeId, AdsErrorCode result, AdsState adsState, ushort deviceState)
        {
            Logger.LogDebug($"ReadStateCon(Address: {sender}, InvokeId: {invokeId}, Result: {result}, State: {adsState}, DeviceState: {deviceState})");
        }

        public override void LogReadStateInd(AmsAddress sender, uint invokeId)
        {
            Logger.LogDebug($"ReadStateInd(Address: {sender}, InvokeId: {invokeId})");
        }

        public override void LogReadWriteCon(AmsAddress sender, uint invokeId, AdsErrorCode result, ReadOnlyMemory<byte> readData)
        {
            Logger.LogDebug($"ReadWriteConfirmation(Address: {sender}, InvokeId: {invokeId}, Result: {result}, Length: {readData.Length})");
        }

        public override void LogReadWriteInd(AmsAddress sender, uint invokeId, uint indexGroup, uint indexOffset, int cbReadLength, ReadOnlyMemory<byte> writeData)
        {
            Logger.LogDebug($"ReadWriteInd(Address: {sender}, InvokeId: {invokeId}, IG: {indexGroup}, IO: {indexOffset}, ReadLen: {cbReadLength}, WriteLen: {writeData.Length})");
        }

        public override void LogWriteCon(AmsAddress sender, uint invokeId, AdsErrorCode result)
        {
            Logger.LogDebug($"WriteCon(Address: {sender}, InvokeId: {invokeId}, Result: {result})");
        }

        public override void LogWriteControlInd(AmsAddress sender, uint invokeId, AdsState adsState, ushort deviceState, ReadOnlyMemory<byte> data)
        {
            Logger.LogDebug($"WriteControlInd(Address: {sender}, InvokeId: {invokeId}, AdsState: {adsState}, DeviceState: {deviceState}, Length: {data.Length})");
        }

        public override void LogWriteInd(AmsAddress sender, uint invokeId, uint indexGroup, uint indexOffset, ReadOnlyMemory<byte> writeData)
        {
            Logger.LogDebug($"WriteInd(Address: {sender}, InvokeId: {invokeId}, IG: {indexGroup}, IO: {indexOffset}, Length: {writeData.Length})");
        }
    }
}