using DoubleGis.Erm.Platform.Model.Metadata.DI;

using NuClear.Assembling.Zones;
using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.DI
{
    public sealed class BlFlexUIWebMetadataAssembly : IZoneAssembly<MetadataZone>,
                                                      IZoneAnchor<MetadataZone>,
                                                      IContainsType<IMetadataSource>
    {
    }
}