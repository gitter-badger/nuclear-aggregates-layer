using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.DI
{
    public sealed class BlQueryingApiOperationsListingAssembly : IZoneAssembly<QueryingZone>,
                                                                 IZoneAnchor<QueryingZone>,
                                                                 IContainsType<IOperation>
    {
    }
}