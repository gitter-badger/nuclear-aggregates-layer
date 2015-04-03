using DoubleGis.Erm.Platform.Model.Metadata.DI;
using NuClear.Assembling.Zones;

using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI
{
    public sealed class MetadataZonePartAssembly : IZoneAssembly<MetadataZone>,
                                                   IZoneAnchor<MetadataZone>,
                                                   IContainsType<IMetadataSource>
    {
    }
}