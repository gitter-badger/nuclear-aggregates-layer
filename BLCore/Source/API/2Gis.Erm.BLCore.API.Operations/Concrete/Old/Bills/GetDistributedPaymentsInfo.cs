using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills
{
    public sealed class DistributedPaymentsInfo
    {
        public DateTime PaymentDatePlan { get; set; }
        public DateTime PaymentPeriodStart { get; set; }
        public DateTime PaymentPeriodEnd { get; set; }
        public decimal PaymentValue { get; set; }
    }

    public sealed class GetDistributedPaymentsInfoRequest : Request
    {
        public long? OrderId { get; set; }
        public IEnumerable<DistributedPaymentsInfo> InitPaymentsInfos { get; set; }
    }

    public sealed class GetDistributedPaymentsInfoResponse : Response
    {
        public IEnumerable<DistributedPaymentsInfo> DistributedPaymentsInfos { get; set; }
    }
}