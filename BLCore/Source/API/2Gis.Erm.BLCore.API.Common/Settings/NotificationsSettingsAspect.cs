using NuClear.Settings;
using NuClear.Settings.API;

namespace DoubleGis.Erm.BLCore.API.Common.Settings
{
    public sealed class NotificationsSettingsAspect : ISettingsAspect, INotificationsSettings
    {
        private readonly BoolSetting _enableNotifications = ConfigFileSetting.Bool.Required("EnableNotifications");

        public bool EnableNotifications
        {
            get { return _enableNotifications.Value; }
        }
    }
}