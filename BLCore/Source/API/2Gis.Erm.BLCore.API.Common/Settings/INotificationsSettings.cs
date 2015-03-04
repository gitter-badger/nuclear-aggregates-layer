using Nuclear.Settings.API;

namespace DoubleGis.Erm.BLCore.API.Common.Settings
{
    public interface INotificationsSettings : ISettings
    {
        bool EnableNotifications { get; }
    }
}
