using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Bills
{
    // TODO:подозреваю, что это не должно быть хэндлером
    public sealed class DistributeBillPaymentsHandler : RequestHandler<DistributeBillPaymentsRequest, DistributeBillPaymentsResponse>
    {
        private readonly IPaymentsDistributor _paymentsDistributor;

        public DistributeBillPaymentsHandler(IPaymentsDistributor paymentsDistributor)
        {
            _paymentsDistributor = paymentsDistributor;
        }

        protected override DistributeBillPaymentsResponse Handle(DistributeBillPaymentsRequest request)
        {
            var payments = new List<DistributeBillPaymentsResponse.BillPayment>();
            var response = new DistributeBillPaymentsResponse
                               {
                                   Payments = payments
                               };
            if (request.PaymentAmount == 0 || request.PaymentAmount == 1 || request.PaymentType == BillPaymentType.Single)
            {
                payments.Add(new DistributeBillPaymentsResponse.BillPayment
                                 {
                                     PaymentValue = request.OrderPayablePlan,
                                     PaymentDatePlan = request.PaymentDatePlanEvaluator(1, request.SignUpDate, request.BeginDistributionDate),
                                     PaymentPeriodStart = request.BeginDistributionDate,
                                     PaymentPeriodEnd = request.EndDistributionDate,
                                 });
            }
            else
            {
                var regularPayments = _paymentsDistributor.DistributePayment(request.OrderReleaseCount, request.OrderPayablePlan);

                for (int i = 0; i < request.PaymentAmount; i++)
                {
                    var paymentPeriodStart = request.BeginDistributionDate.AddMonths(i);
                    payments.Add(new DistributeBillPaymentsResponse.BillPayment
                    {
                        PaymentDatePlan = request.PaymentDatePlanEvaluator(i + 1, request.SignUpDate, paymentPeriodStart),
                        PaymentPeriodStart = paymentPeriodStart,
                        PaymentPeriodEnd = paymentPeriodStart.AddMonths(1).AddSeconds(-1),
                        PaymentValue = regularPayments[i]
                    });
                }

                var paymentGroupCount = request.OrderReleaseCount / request.PaymentAmount - 1;
                for (int groupIndex = 0; groupIndex < paymentGroupCount; groupIndex++)
                {
                    for (int paymentIndex = 0; paymentIndex < payments.Count; paymentIndex++)
                    {
                        var additionalPaymentValue = regularPayments[request.PaymentAmount + paymentIndex + request.PaymentAmount * groupIndex];
                        UpdatePaymentWithShift(payments, paymentIndex, additionalPaymentValue);
                    }
                }

                var paymentRemainder = request.OrderReleaseCount % request.PaymentAmount;
                for (int paymentIndex = 0; paymentIndex < paymentRemainder; paymentIndex++)
                {
                    var additionalPaymentValue = regularPayments[request.PaymentAmount + request.PaymentAmount * paymentGroupCount + paymentIndex];
                    UpdatePaymentWithShift(payments, paymentIndex, additionalPaymentValue);
                }
            }
            return response;
        }

        private static void UpdatePaymentWithShift(
            IList<DistributeBillPaymentsResponse.BillPayment> payments, 
            int paymentIndex,
                                                   decimal additionalPaymentValue)
        {
            var payment = payments[paymentIndex];

            // добавление суммы среднего платежа и увеличение срока платежа на месяц
            payment.PaymentValue += additionalPaymentValue;
            payment.PaymentPeriodEnd = payment.PaymentPeriodEnd.GetFirstDateOfMonth().AddMonths(2).AddSeconds(-1);

            // смещение даты на месяц для последующих платежей
            for (int i = paymentIndex + 1; i < payments.Count; i++)
            {
                payments[i].PaymentPeriodStart = payments[i].PaymentPeriodStart.AddMonths(1);
                payments[i].PaymentPeriodEnd = payments[i].PaymentPeriodEnd.GetFirstDateOfMonth().AddMonths(2).AddSeconds(-1);
            }
        }
    }
}