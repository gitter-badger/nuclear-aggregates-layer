using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.DI;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.DI
{
    public sealed class BlUIWebMetadataAssembly : IZoneAssembly<MetadataZone>,
                                                  IZoneAnchor<MetadataZone>,
                                                  IContainsType<IMetadataSource>
    {
    }
}