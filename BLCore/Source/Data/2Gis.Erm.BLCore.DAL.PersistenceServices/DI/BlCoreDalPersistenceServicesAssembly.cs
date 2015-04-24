using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.DI;
using DoubleGis.Erm.Platform.Model.Simplified;

using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.DI
{
    public sealed class BlCoreDalPersistenceServicesAssembly : IZoneAssembly<PlatformZone>,
                                                               IZoneAnchor<PlatformZone>,
                                                               IContainsType<IPersistenceService>,
                                                               IContainsType<ISimplifiedModelConsumer>
    {
    }
}