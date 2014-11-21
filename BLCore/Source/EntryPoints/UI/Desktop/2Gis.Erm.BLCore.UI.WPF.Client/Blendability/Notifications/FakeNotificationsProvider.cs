using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Blendability.Notifications
{
    public static class FakeNotificationsProvider
    {
        public static INotification[] GetContextualNotifications(Guid sourceId)
        {
            return new INotification[]
                {
                    new ContextualNotification(sourceId, NotificationLevel.Error, "Contextual Error description"),
                    new ContextualNotification(sourceId, NotificationLevel.Warning, "Contextual Warning description"),
                    new ContextualNotification(sourceId, NotificationLevel.Info, "Contextual Info description")
                };
        }

        public static INotification[] GetSystemNotifications(Guid sourceId)
        {
            return new INotification[]
                {
                    new SystemNotification(sourceId, NotificationLevel.Error, "System Error description"),
                    new SystemNotification(sourceId, NotificationLevel.Warning, "System Warning description"),
                    new SystemNotification(sourceId, NotificationLevel.Info, "System Info description")
                };
        }
    }
}