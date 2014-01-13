using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills
{
    public class DistributeBillPaymentsRequest : Request
    {
        public BillPaymentType PaymentType { get; set; }
        public decimal OrderPayablePlan { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDate { get; set; }
        public DateTime SignUpDate { get; set; }
        public int OrderReleaseCount { get; set; }
        public int PaymentAmount { get; set; }
        public Func<int, DateTime, DateTime, DateTime> PaymentDatePlanEvaluator { get; set; }
    }

    public class DistributeBillPaymentsResponse : Response
    {
        public class BillPayment
        {
            public DateTime PaymentDatePlan { get; set; }
            public DateTime PaymentPeriodStart { get; set; }
            public DateTime PaymentPeriodEnd { get; set; }
            public decimal PaymentValue { get; set; }
        }

        public IEnumerable<BillPayment> Payments { get; set; }
    }
}