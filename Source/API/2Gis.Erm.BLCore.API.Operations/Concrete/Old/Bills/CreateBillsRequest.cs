using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills
{
    public sealed class CreateBillsRequest : Request
    {
        public long OrderId { get; set; }
        public CreateBillInfo[] CreateBillInfos { get; set; }
    }

    public sealed class CreateBillInfo
    {
        // temporal bill name (used before bill saving)
        public string PaymentNumber { get; set; }

        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDate { get; set; }
        public DateTime PaymentDatePlan { get; set; }
        public decimal PayablePlan { get; set; }
        public string BillNumber { get; set; }
    }
}