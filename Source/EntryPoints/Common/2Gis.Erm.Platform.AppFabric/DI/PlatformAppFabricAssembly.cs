using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.Platform.AppFabric.DI
{
    public class PlatformAppFabricAssembly : IZoneAssembly<AppFabricZone>,
                                             IZoneAnchor<AppFabricZone>,
                                             IContainsType<ICacheAdapter>
    {
    }
}