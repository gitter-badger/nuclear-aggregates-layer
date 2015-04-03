using System.Web.Mvc;

using NuClear.Assembling.Zones;
using DoubleGis.Erm.Platform.UI.Web.Mvc.DI;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.DI
{
    public sealed class BlUiWebMvcAssembly : IZoneAssembly<WebMvcZone>,
                                             IZoneAnchor<WebMvcZone>,
                                             IContainsType<IController>,
                                             IContainsType<IUIService>,
                                             IContainsType<IEntityUIService>,
                                             IContainsType<IViewModel>
    {
    }
}