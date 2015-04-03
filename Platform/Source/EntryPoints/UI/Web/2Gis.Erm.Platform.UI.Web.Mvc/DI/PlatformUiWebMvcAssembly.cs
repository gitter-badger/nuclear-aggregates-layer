using NuClear.Assembling.Zones;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services.Enums;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.DI
{
    public sealed class PlatformUiWebMvcAssembly : IZoneAssembly<WebMvcZone>,
                                                   IZoneAnchor<WebMvcZone>,
                                                   IContainsType<IEnumAdaptationService>
    {
    }
}