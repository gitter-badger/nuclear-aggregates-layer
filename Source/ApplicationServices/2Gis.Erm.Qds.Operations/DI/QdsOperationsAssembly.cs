using DoubleGis.Erm.BLQuerying.API.Operations.Listing.DI;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.Qds.Operations.DI
{
    public sealed class QdsOperationsAssembly : IZoneAssembly<QueryingZone>,
                                                IZoneAnchor<QueryingZone>,
                                                IContainsType<IOperation>
    {
    }
}