using DoubleGis.Erm.Platform.Model.Metadata.DI;
using NuClear.Assembling.Zones;

using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.BLQuerying.UI.Metadata.DI
{
    public sealed class BlQueryingUiMetadataAssembly : IZoneAssembly<MetadataZone>,
                                                       IZoneAnchor<MetadataZone>,
                                                       IContainsType<IMetadataSource>
    {
    }
}
