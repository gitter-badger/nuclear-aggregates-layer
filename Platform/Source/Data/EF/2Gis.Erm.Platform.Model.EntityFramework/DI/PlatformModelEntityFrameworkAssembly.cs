using DoubleGis.Erm.Platform.Model.DI;

using NuClear.Assembling.Zones;
using NuClear.Storage.EntityFramework;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.DI
{
    public class PlatformModelEntityFrameworkAssembly : IZoneAssembly<PlatformZone>,
                                                        IZoneAnchor<PlatformZone>,
                                                        IContainsType<IEFDbModelConfiguration>,
                                                        IContainsType<IEFDbModelConvention>
    {
    }
}