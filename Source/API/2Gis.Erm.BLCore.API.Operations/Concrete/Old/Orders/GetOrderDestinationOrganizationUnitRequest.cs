using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders
{
    public sealed class GetOrderDestinationOrganizationUnitRequest : Request
    {
        public long FirmId { get; set; }
    }

    public sealed class GetOrderDestinationOrganizationUnitResponse : Response
    {
        public long OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
    }
}