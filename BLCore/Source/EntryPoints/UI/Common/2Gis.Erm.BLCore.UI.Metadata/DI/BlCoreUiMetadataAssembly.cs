using DoubleGis.Erm.Platform.Model.Metadata.DI;
using NuClear.Assembling.Zones;

using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.BLCore.UI.Metadata.DI
{
    public class BlCoreUiMetadataAssembly : IZoneAssembly<MetadataZone>,
                                            IZoneAnchor<MetadataZone>,
                                            IContainsType<IMetadataSource>
    {
    }
}