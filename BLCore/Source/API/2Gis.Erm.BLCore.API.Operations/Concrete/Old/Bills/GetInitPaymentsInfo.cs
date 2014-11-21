using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills
{
    public class GetInitPaymentsInfoRequest : Request
    {
        public long? OrderId { get; set; }
        public BillPaymentType PaymentType { get; set; }
        public int? PaymentAmount { get; set; }
        public Func<int, DateTime, DateTime, DateTime> PaymentDatePlanEvaluator { get; set; }
    }

    public class GetInitPaymentsInfoResponse : Response
    {
        public object PaymentsInfo { get; set; }
    }
}