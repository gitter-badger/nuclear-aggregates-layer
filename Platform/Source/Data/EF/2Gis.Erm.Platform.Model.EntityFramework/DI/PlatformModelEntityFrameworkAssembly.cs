using DoubleGis.Erm.Platform.Model.DI;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.DI
{
    public class PlatformModelEntityFrameworkAssembly : IZoneAssembly<PlatformZone>,
                                                        IZoneAnchor<PlatformZone>,
                                                        IContainsType<IEfDbModelConfiguration>,
                                                        IContainsType<IEfDbModelConvention>
    {
    }
}