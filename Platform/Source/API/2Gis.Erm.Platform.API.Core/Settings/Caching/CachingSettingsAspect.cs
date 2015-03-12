using DoubleGis.Erm.Platform.Common.Caching;

using NuClear.Settings;
using NuClear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Settings.Caching
{
    public sealed class CachingSettingsAspect : ISettingsAspect, ICachingSettings
    {
        private readonly EnumSetting<CachingMode> _entryPointCacheType = ConfigFileSetting.Enum.Required<CachingMode>("CachingMode");
        private readonly StringSetting _distributedCacheName = ConfigFileSetting.String.Optional("DistributedCacheName", "default");

        public CachingMode CachingMode
        {
            get { return _entryPointCacheType.Value; }
        }

        public string DistributedCacheName
        {
            get { return _distributedCacheName.Value; }
        }
    }
}