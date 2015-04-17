using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Zones;
using DoubleGis.Erm.Platform.UI.Web.Mvc.DI;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.DI
{
    public sealed class BlFlexUIWebMvcAssembly : IZoneAssembly<WebMvcZone>,
                                                 IZoneAnchor<WebMvcZone>,
                                                 IContainsType<IController>,
                                                 IContainsType<IUIService>,
                                                 IContainsType<IEnumAdaptationService>,
                                                 IContainsType<IEntityUIService>,
                                                 IContainsType<IViewModel>,
                                                 IContainsType<IViewModelCustomization>,
                                                 IContainsType<IMetadataSource>
    {
    }
}