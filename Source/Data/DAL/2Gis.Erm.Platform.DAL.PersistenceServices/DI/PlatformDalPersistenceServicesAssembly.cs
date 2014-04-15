using DoubleGis.Erm.Platform.Model.DI;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices.DI
{
    public sealed class PlatformDalPersistenceServicesAssembly : IZoneAssembly<PlatformZone>,
                                                                 IZoneAnchor<PlatformZone>,
                                                                 IContainsType<IPersistenceService>
    {
    }
}