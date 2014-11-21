using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills
{
    public sealed class GetRelatedOrdersForPrintJointBillRequest : Request
    {
        public long OrderId { get; set; }
    }

    public sealed class GetRelatedOrdersForPrintJointBillResponse : Response
    {
        public RelatedOrderDescriptor[] Orders { get; set; }
    }
}
