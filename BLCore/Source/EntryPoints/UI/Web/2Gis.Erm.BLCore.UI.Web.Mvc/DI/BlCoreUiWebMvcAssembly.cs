using System.Web.Mvc;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Simplified;
using DoubleGis.Erm.Platform.UI.Web.Mvc.DI;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.DI
{
    public sealed class BlCoreUIWebMvcAssembly : IZoneAssembly<WebMvcZone>,
                                                 IZoneAnchor<WebMvcZone>,
                                                 IContainsType<IController>,
                                                 IContainsType<IEnumAdaptationService>,
                                                 IContainsType<IUIService>,
                                                 IContainsType<ISimplifiedModelConsumer>,
                                                 IContainsType<IEntityUIService>,
                                                 IContainsType<IViewModel>,
                                                 IContainsType<IMetadataSource>
    {
    }
}