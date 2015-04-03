using DoubleGis.Erm.BLQuerying.API.Operations.Listing.DI;
using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Assembling.Zones;

using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.Qds.Operations.DI
{
    public sealed class QdsOperationsAssembly : IZoneAssembly<QueryingZone>,
                                                IZoneAnchor<QueryingZone>,
                                                IContainsType<IOperation>,
                                                IContainsType<IMetadataSource>
    {
    }
}