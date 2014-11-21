using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders
{
    public sealed class GetOrdersWithDummyAdvertisementRequest : Request
    {
        public long OrganizationUnitId { get; set; }
        public long OwnerId { get; set; }
        public bool IncludeOwnerDescendants { get; set; }
    }
}
