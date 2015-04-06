using DoubleGis.Erm.Platform.Common.Caching;

using NuClear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Settings.Caching
{
    public interface ICachingSettings : ISettings
    {
        CachingMode CachingMode { get; }
        string DistributedCacheName { get; }
    }
}