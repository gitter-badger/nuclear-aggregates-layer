using System;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications
{
    public interface ISystemNotification : INotification
    {
        DateTime? ExpiredTimeUtc { get; set; }
    }
}