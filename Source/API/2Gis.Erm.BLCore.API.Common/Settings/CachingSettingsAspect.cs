using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.API.Common.Settings
{
    public sealed class CachingSettingsAspect : ISettingsAspect, ICachingSettings
    {
        private readonly BoolSetting _enableCaching = ConfigFileSetting.Bool.Required("EnableCaching");

        public bool EnableCaching
        {
            get { return _enableCaching.Value; }
        }
    }
}