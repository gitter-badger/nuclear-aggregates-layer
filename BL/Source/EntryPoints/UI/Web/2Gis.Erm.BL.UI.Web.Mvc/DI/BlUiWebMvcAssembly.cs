using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using NuClear.Assembling.Zones;
using DoubleGis.Erm.Platform.UI.Web.Mvc.DI;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.DI
{
    public sealed class BlUIWebMvcAssembly : IZoneAssembly<WebMvcZone>,
                                             IZoneAnchor<WebMvcZone>,
                                             IContainsType<IController>,
                                             IContainsType<IUIService>,
                                             IContainsType<IEntityUIService>,
                                             IContainsType<IViewModel>,
                                             IContainsType<IViewModelCustomization>,
                                             IContainsType<IMetadataSource>
    {
    }
}