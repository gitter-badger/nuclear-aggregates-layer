using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.DI;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BLQuerying.UI.Metadata.DI
{
    public sealed class BlQueryingUIMetadataAssembly : IZoneAssembly<MetadataZone>,
                                                       IZoneAnchor<MetadataZone>,
                                                       IContainsType<IMetadataSource>
    {
    }
}
