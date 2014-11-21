using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills
{
    public sealed class DeleteBillsRequest : Request
    {
        public long OrderId { get; set; }
    }
}