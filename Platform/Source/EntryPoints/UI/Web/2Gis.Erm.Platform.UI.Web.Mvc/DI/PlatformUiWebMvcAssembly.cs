using DoubleGis.Erm.Platform.UI.Web.Mvc.Services.Enums;

using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.DI
{
    public sealed class PlatformUIWebMvcAssembly : IZoneAssembly<WebMvcZone>,
                                                   IZoneAnchor<WebMvcZone>,
                                                   IContainsType<IEnumAdaptationService>
    {
    }
}