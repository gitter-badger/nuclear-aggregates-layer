using NuClear.Settings;
using NuClear.Settings.API;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals.Settings
{
    public sealed class WarmClientProcessingSettingsAspect : ISettingsAspect, IWarmClientProcessingSettings
    {
        private readonly IntSetting _warmClientDaysCount = ConfigFileSetting.Int.Optional("WarmClientDaysCount", WarmClientProcessingConstants.WarmClientDaysCount);
        
        public int WarmClientDaysCount
        {
            get { return _warmClientDaysCount.Value; }
        }
    }
}