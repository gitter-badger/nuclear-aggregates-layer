using DoubleGis.Erm.Platform.Model.Metadata.DI;

using NuClear.Assembling.Zones;
using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.DI
{
    public sealed class BlUIWebMetadataAssembly : IZoneAssembly<MetadataZone>,
                                                  IZoneAnchor<MetadataZone>,
                                                  IContainsType<IMetadataSource>
    {
    }
}