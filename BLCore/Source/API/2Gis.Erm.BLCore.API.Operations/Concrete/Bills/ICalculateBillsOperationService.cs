using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Bill;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Bills
{
    public interface ICalculateBillsOperationService : IOperation<CalculateBillsIdentity>
    {
        PaymentsInfo GetPayments(long? orderId, int? paymentAmount, BillPaymentType paymentType);
    }

    public sealed class PaymentsInfo
    {
        public int BillsCount { get; set; }
        public decimal OrderSum { get; set; }
        public int OrderReleaseCount { get; set; }
        public IEnumerable<BillPayment> Payments { get; set; }
    }

    public sealed class BillPayment
    {
        public DateTime PaymentDatePlan { get; set; }
        public DateTime PaymentPeriodStart { get; set; }
        public DateTime PaymentPeriodEnd { get; set; }
        public decimal PaymentValue { get; set; }
    }
}
