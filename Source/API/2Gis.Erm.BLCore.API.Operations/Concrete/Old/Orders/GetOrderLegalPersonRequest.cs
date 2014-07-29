using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders
{
    public sealed class GetOrderLegalPersonRequest : Request
    {
        public long FirmClientId { get; set; }
    }

    public sealed class GetOrderLegalPersonResponse : Response
    {
        public long? LegalPersonId { get; set; }
        public string LegalPersonName { get; set; }
    }
}
