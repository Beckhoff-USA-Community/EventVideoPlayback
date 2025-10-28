using System;
using System.Collections.Generic;
using System.Text;
using TwinCAT.Ads;

namespace EventVideoPlaybackService
{
    /// <summary>
    /// Represents a single notification request entry in the ADS server's notification table.
    /// Stores the parameters required to manage device notifications from ADS clients,
    /// including the client address, data location (index group/offset), and notification settings.
    /// </summary>
    internal class NotificationRequestEntry
    {
        private readonly AmsAddress _rAddr;
        private readonly uint _indexGroup;
        private readonly uint _indexOffset;
        private readonly int _cbLength;
        private readonly NotificationSettings _settings;

        /// <summary>
        /// Initializes a new notification request entry with the specified parameters.
        /// This entry is stored in the server's notification table and used to track
        /// active notification subscriptions from ADS clients.
        /// </summary>
        /// <param name="rAddr">The AMS address of the requesting client that will receive notifications.</param>
        /// <param name="indexGroup">The ADS index group identifying the data area to monitor.</param>
        /// <param name="indexOffset">The ADS index offset within the index group to monitor.</param>
        /// <param name="cbLength">The number of bytes to include in each notification.</param>
        /// <param name="settings">The notification settings specifying transmission mode, cycle time, and max delay.</param>
        internal NotificationRequestEntry(AmsAddress rAddr,
                                          uint indexGroup,
                                          uint indexOffset,
                                          int cbLength,
                                          NotificationSettings settings)
        {
            _rAddr = rAddr;
            _indexGroup = indexGroup;
            _indexOffset = indexOffset;
            _cbLength = cbLength;
            _settings = settings;
        }
    }
}
