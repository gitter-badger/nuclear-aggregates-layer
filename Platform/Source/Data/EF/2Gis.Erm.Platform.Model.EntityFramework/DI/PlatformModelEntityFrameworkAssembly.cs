using DoubleGis.Erm.Platform.Model.DI;
using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.DI
{
    public class PlatformModelEntityFrameworkAssembly : IZoneAssembly<PlatformZone>,
                                                        IZoneAnchor<PlatformZone>,
                                                        IContainsType<IEfDbModelConfiguration>,
                                                        IContainsType<IEfDbModelConvention>
    {
    }
}