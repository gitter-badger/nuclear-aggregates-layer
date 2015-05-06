using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.DI;

using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.BL.UI.Metadata.DI
{
    public sealed class BlUIMetadataAssembly : IZoneAssembly<MetadataZone>,
                                               IZoneAnchor<MetadataZone>,
                                               IContainsType<IMetadataSource>
    {
    }
}