using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.Platform.Model.Metadata.DI
{
    public sealed class PlatformModelMetadataAssembly : IZoneAssembly<MetadataZone>,
                                                        IZoneAnchor<MetadataZone>,
                                                        IContainsType<IMetadataSource>
    {
    }
}
