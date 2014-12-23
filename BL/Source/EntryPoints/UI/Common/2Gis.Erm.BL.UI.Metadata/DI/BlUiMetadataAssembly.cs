using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.DI;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BL.UI.Metadata.DI
{
    public sealed class BlUIMetadataAssembly : IZoneAssembly<MetadataZone>,
                                               IZoneAnchor<MetadataZone>,
                                               IContainsType<IMetadataSource>
    {
    }
}