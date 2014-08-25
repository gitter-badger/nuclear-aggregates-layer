using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.DI;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BLCore.UI.Metadata.DI
{
    public class BlCoreUiMetadataAssembly : IZoneAssembly<MetadataZone>,
                                            IZoneAnchor<MetadataZone>,
                                            IContainsType<IMetadataSource>
    {
    }
}