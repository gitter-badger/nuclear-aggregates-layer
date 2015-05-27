using DoubleGis.Erm.Platform.Model.Metadata.DI;

using NuClear.Assembling.Zones;
using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.Tests.Integration.InProc.DI.Zones.Parts
{
    public sealed class MetadataZonePartAssembly : IZoneAssembly<MetadataZone>,
                                                   IZoneAnchor<MetadataZone>,
                                                   IContainsType<IMetadataSource>
    {
    }
}