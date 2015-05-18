using DoubleGis.Erm.Platform.Model.Metadata.DI;

using NuClear.Assembling.Zones;
using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.BL.UI.Metadata.DI
{
    public sealed class BlUIMetadataAssembly : IZoneAssembly<MetadataZone>,
                                               IZoneAnchor<MetadataZone>,
                                               IContainsType<IMetadataSource>
    {
    }
}