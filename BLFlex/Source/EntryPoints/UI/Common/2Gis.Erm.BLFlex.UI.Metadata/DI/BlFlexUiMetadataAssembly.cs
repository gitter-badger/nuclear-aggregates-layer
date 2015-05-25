using DoubleGis.Erm.Platform.Model.Metadata.DI;

using NuClear.Assembling.Zones;
using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.BLFlex.UI.Metadata.DI
{
    public sealed class BlFlexUIMetadataAssembly : IZoneAssembly<MetadataZone>,
                                                   IZoneAnchor<MetadataZone>,
                                                   IContainsType<IMetadataSource>
    {
    }
}