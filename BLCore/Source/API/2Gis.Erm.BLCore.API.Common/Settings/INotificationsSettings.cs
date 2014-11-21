using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.API.Common.Settings
{
    public interface INotificationsSettings : ISettings
    {
        bool EnableNotifications { get; }
    }
}
