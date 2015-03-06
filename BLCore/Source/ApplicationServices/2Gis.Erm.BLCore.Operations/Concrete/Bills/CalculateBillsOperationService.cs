using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Bills
{
    public sealed class CalculateBillsOperationService : ICalculateBillsOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IPaymentsDistributor _paymentsDistributor;
        private readonly PaymentDatePlanEvaluator _paymentDatePlanEvaluator;

        public CalculateBillsOperationService(IOrderReadModel orderReadModel, IPaymentsDistributor paymentsDistributor)
        {
            _orderReadModel = orderReadModel;
            _paymentsDistributor = paymentsDistributor;
            _paymentDatePlanEvaluator = new PaymentDatePlanEvaluator();
        }

        public PaymentsInfo GetPayments(long? orderId, int? paymentAmount, BillPaymentType paymentType)
        {
            if (!orderId.HasValue)
            {
                throw new NotificationException(BLResources.CantGetOrderIdentifier);
            }

            var orderInfo = _orderReadModel.GetOrderInfoForInitPayments(orderId.Value);

            if (!orderInfo.IsOnRegistration)
            {
                throw new NotificationException(BLResources.CantEditBillsWhenOrderIsNotOnRegistration);
            }

            paymentAmount = paymentAmount ?? orderInfo.ReleaseCountPlan;
            var singlePayment = paymentAmount == 0 || paymentAmount == 1 || paymentType == BillPaymentType.Single;

            return new PaymentsInfo
                       {
                           OrderSum = orderInfo.PayablePlan,
                           OrderReleaseCount = orderInfo.ReleaseCountPlan,
                           Payments = singlePayment
                                          ? SinglePayment(orderInfo.PayablePlan, orderInfo.SignupDate, orderInfo.BeginDistributionDate, orderInfo.ReleaseCountPlan)
                                          : MultiplePayments(orderInfo.PayablePlan, orderInfo.SignupDate, orderInfo.BeginDistributionDate, orderInfo.ReleaseCountPlan, paymentAmount.Value),
                           BillsCount = orderInfo.BillsCount
                       };
        }

        private IEnumerable<BillPayment> SinglePayment(decimal payablePlan, DateTime signUpDate, DateTime beginDustributionDate, int releaseCount)
        {
            var singlePayment = new BillPayment
                                    {
                                        PaymentValue = payablePlan,
                                        PaymentDatePlan = _paymentDatePlanEvaluator.Evaluate(1, signUpDate, beginDustributionDate),
                                        PaymentPeriodStart = beginDustributionDate,
                                        PaymentPeriodEnd = GetPaymentPerodEnd(beginDustributionDate, releaseCount),
                                    };
            return new[] { singlePayment };
        }

        private IEnumerable<BillPayment> MultiplePayments(decimal payablePlan, DateTime signUpDate, DateTime beginDustributionDate, int releaseCount, int billCount)
        {
            // платежи по месяцам. но счета не будут строго совпадать по числу с числом месяцев.
            var paymentsForEachMonth = _paymentsDistributor.DistributePayment(releaseCount, payablePlan);
            var billDurations = CalculateBillDurations(billCount, releaseCount);

            var billPeriodStart = beginDustributionDate;

            var payments = new List<BillPayment>();
            for (var i = 0; i < paymentsForEachMonth.Length;)
            {
                var billIndex = payments.Count;
                var bill = new BillPayment
                {
                    PaymentDatePlan = _paymentDatePlanEvaluator.Evaluate(i + 1, signUpDate, billPeriodStart),
                    PaymentPeriodStart = billPeriodStart,
                    PaymentPeriodEnd = GetPaymentPerodEnd(billPeriodStart, billDurations[billIndex]),
                    PaymentValue = paymentsForEachMonth.Skip(i).Take(billDurations[billIndex]).Sum()
                };

                payments.Add(bill);
                billPeriodStart = billPeriodStart.AddMonths(billDurations[billIndex]);
                i += billDurations[billIndex];
            }

            return payments;
        }

        private int[] CalculateBillDurations(int billCount, int monthCount)
        {
            var averageBillDuration = monthCount / billCount;
            var mapping = Enumerable.Repeat(averageBillDuration, billCount).ToArray();

            // равномерно покрыть, скорее всего, не получится - поэтому появятся более ёмкие платежи. ставим их в начале.
            var roundingError = monthCount - mapping.Sum(); // 0 <= roundingError < billCount
            for (var i = 0; i < roundingError; i++)
            {
                mapping[i] += 1;
            }

            return mapping;
        }

        private DateTime GetPaymentPerodEnd(DateTime paymentPeriodStart, int monthCount)
        {
            return paymentPeriodStart.AddMonths(monthCount).AddSeconds(-1);
        }
    }
}
