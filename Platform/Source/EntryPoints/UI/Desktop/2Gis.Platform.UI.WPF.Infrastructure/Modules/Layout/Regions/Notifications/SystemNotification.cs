using System;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications
{
    public sealed class SystemNotification : ISystemNotification
    {
        private readonly Guid _sourceId;
        private readonly NotificationLevel _notificationLevel;
        private readonly string _description;
        private readonly DateTime _timestampUtc = DateTime.UtcNow;

        public SystemNotification(Guid sourceId, NotificationLevel notificationLevel, string description)
        {
            _sourceId = sourceId;
            _notificationLevel = notificationLevel;
            _description = description;
            ExpiredTimeUtc = DateTime.UtcNow.AddMinutes(5);
        }

        public Guid SourceId
        {
            get
            {
                return _sourceId;
            }
        }

        public NotificationType NotificationType
        {
            get
            {
                return NotificationType.System;
            }
        }

        public NotificationLevel NotificationLevel
        {
            get
            {
                return _notificationLevel;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public DateTime TimestampUtc 
        {
            get
            {
                return _timestampUtc;
            }
        }

        public DateTime? ExpiredTimeUtc { get; set; }
    }
}