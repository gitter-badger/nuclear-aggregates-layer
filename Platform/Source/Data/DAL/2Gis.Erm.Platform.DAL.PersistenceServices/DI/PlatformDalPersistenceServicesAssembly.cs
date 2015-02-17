using DoubleGis.Erm.Platform.Model.DI;
using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices.DI
{
    public sealed class PlatformDalPersistenceServicesAssembly : IZoneAssembly<PlatformZone>,
                                                                 IZoneAnchor<PlatformZone>,
                                                                 IContainsType<IPersistenceService>
    {
    }
}