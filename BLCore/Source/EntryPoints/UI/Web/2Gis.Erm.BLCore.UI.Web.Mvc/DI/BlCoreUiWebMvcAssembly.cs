using System.Web.Mvc;

using DoubleGis.Erm.Platform.Model.Simplified;
using NuClear.Assembling.Zones;
using DoubleGis.Erm.Platform.UI.Web.Mvc.DI;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.DI
{
    public sealed class BlCoreUiWebMvcAssembly : IZoneAssembly<WebMvcZone>,
                                                 IZoneAnchor<WebMvcZone>,
                                                 IContainsType<IController>,
                                                 IContainsType<IEnumAdaptationService>,
                                                 IContainsType<IUIService>,
                                                 IContainsType<ISimplifiedModelConsumer>,
                                                 IContainsType<IEntityUIService>,
                                                 IContainsType<IViewModel>
    {
    }
}