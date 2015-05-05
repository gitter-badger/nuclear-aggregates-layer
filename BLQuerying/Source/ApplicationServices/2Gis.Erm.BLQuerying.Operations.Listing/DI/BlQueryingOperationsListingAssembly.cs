using DoubleGis.Erm.BLQuerying.API.Operations.Listing.DI;
using DoubleGis.Erm.Platform.API.Core.Operations;

using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.DI
{
    public sealed class BlQueryingOperationsListingAssembly : IZoneAssembly<QueryingZone>,
                                                              IZoneAnchor<QueryingZone>,
                                                              IContainsType<IOperation>
    {
    }
}