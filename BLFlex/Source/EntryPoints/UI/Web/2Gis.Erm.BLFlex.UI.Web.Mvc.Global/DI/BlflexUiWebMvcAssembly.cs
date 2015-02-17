using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using NuClear.Assembling.Zones;
using DoubleGis.Erm.Platform.UI.Web.Mvc.DI;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.DI
{
    public sealed class BlFlexUiWebMvcAssembly : IZoneAssembly<WebMvcZone>,
                                                 IZoneAnchor<WebMvcZone>,
                                                 IContainsType<IController>,
                                                 IContainsType<IUIService>,
                                                 IContainsType<IEnumAdaptationService>,
                                                 IContainsType<IEntityUIService>,
                                                 IContainsType<IViewModel>,
                                                 IContainsType<IViewModelCustomization>
    {
    }
}