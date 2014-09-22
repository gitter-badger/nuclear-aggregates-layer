using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.Caching
{
    public interface ICachingSettings : ISettings
    {
        CachingMode CachingMode { get; }
        string DistributedCacheName { get; }
    }
}