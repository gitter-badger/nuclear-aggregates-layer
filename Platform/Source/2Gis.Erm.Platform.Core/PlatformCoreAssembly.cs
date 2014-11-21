using DoubleGis.Erm.Platform.Model.DI;
using DoubleGis.Erm.Platform.Model.Simplified;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.Platform.Core
{
    public sealed class PlatformCoreAssembly : IZoneAssembly<PlatformZone>,
                                               IZoneAnchor<PlatformZone>,
                                               IContainsType<ISimplifiedModelConsumer>
    {
    }
}