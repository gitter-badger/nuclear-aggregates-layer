using System;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications
{
    public sealed class ContextualNotification : IContextualNotification
    {
        private readonly Guid _sourceId;
        private readonly NotificationLevel _notificationLevel;
        private readonly string _description;
        private readonly DateTime _timestampUtc = DateTime.UtcNow;

        public ContextualNotification(Guid sourceId, NotificationLevel notificationLevel, string description)
        {
            _sourceId = sourceId;
            _notificationLevel = notificationLevel;
            _description = description;
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
                return NotificationType.Contextual;
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

        public string PropertyName { get; set; }
    }
}