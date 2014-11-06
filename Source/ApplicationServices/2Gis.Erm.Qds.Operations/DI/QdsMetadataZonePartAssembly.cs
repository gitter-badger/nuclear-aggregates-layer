using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.DI;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.Qds.Operations.DI
{
    public sealed class QdsMetadataZonePartAssembly : IZoneAssembly<MetadataZone>,
                                                      IZoneAnchor<MetadataZone>,
                                                      IContainsType<IMetadataSource>
    {
    }
}