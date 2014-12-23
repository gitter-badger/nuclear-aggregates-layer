using DoubleGis.Erm.Platform.Model.Zones;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services.Enums;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.DI
{
    public sealed class PlatformUIWebMvcAssembly : IZoneAssembly<WebMvcZone>,
                                                   IZoneAnchor<WebMvcZone>,
                                                   IContainsType<IEnumAdaptationService>
    {
    }
}