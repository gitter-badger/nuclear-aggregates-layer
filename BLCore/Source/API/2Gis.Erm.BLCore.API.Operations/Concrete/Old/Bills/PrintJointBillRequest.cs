using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills
{
    public sealed class PrintJointBillRequest: Request
    {
        public long BillId { get; set; }
        public long[] RelatedOrdersId { get; set; }
    }
}
