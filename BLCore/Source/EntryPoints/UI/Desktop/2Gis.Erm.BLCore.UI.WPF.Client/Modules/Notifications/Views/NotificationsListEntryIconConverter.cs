using System;
using System.Globalization;
using System.Windows.Data;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Notifications.Views
{
    public class NotificationsListEntryIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var notificationLevel = (NotificationLevel)value;
            switch (notificationLevel)
            {
                case NotificationLevel.Error:
                    return ImageSourceContainer.ErrorIcon;
                case NotificationLevel.Warning:
                    return ImageSourceContainer.WarningIcon;
                case NotificationLevel.Info:
                    return ImageSourceContainer.MessageIcon;
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

