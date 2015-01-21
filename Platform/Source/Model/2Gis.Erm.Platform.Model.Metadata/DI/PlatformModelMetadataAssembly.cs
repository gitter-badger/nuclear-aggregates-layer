using DoubleGis.Erm.Platform.Model.Zones;

using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.Platform.Model.Metadata.DI
{
    public sealed class PlatformModelMetadataAssembly : IZoneAssembly<MetadataZone>,
                                                        IZoneAnchor<MetadataZone>,
                                                        IContainsType<IMetadataSource>
    {
    }
}
