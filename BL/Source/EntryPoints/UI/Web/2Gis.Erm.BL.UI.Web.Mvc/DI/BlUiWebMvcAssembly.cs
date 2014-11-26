using System.Web.Mvc;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Zones;
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
                                             IContainsType<IMetadataSource>,
                                             IContainsType<IViewModel>
    {
    }
}