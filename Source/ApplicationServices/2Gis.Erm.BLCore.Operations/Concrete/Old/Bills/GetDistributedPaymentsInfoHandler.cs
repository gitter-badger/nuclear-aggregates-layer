using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Bills
{
    public sealed class GetDistributedPaymentsInfoHandler : RequestHandler<GetDistributedPaymentsInfoRequest, GetDistributedPaymentsInfoResponse>
    {
        private readonly IOrderRepository _orderRepository;

        public GetDistributedPaymentsInfoHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        protected override GetDistributedPaymentsInfoResponse Handle(GetDistributedPaymentsInfoRequest request)
        {
            if (!request.OrderId.HasValue)
            {
                throw new NotificationException(BLResources.CantGetOrderIdentifier);
            }

            var orderInfo = _orderRepository.GetOrderUnsecure(request.OrderId.Value);
            if (orderInfo == null)
            {
                throw new NotificationException(BLResources.OrderInfoNotFound);
            }
            
            CheckDates(request.InitPaymentsInfos, orderInfo.BeginDistributionDate, orderInfo.EndDistributionDatePlan);

            // вычисляем платежи по выпускам (месячные)
            var orderPayablePlanDistributedPayments = PaymentsDistributor.DistributePayment(orderInfo.ReleaseCountPlan, orderInfo.PayablePlan);

            var distributedPaymentsInfos = new List<DistributedPaymentsInfo>(request.InitPaymentsInfos);
            var distributedPaymentIndex = 0;
            for (int paymentIndex = 0; paymentIndex < distributedPaymentsInfos.Count; paymentIndex++)
            {
                // проверяем, не вышли ли за пределы периода размещения
                if (distributedPaymentIndex >= orderPayablePlanDistributedPayments.Length)
                {
                    throw new NotificationException(BLResources.SelectedPaymentDatesAreOutOfOrderPeriodBound);
                }

                var distributedPaymentsInfo = distributedPaymentsInfos[paymentIndex];

                // обнуляем предыдущие суммы платежей
                distributedPaymentsInfo.PaymentValue = 0m;

                // вычисляем количество периодов размещения для платежа и складываем суммы месячных платежей
                var paymentsForPeriodCount = (distributedPaymentsInfo.PaymentPeriodEnd.Year - distributedPaymentsInfo.PaymentPeriodStart.Year)*12 +
                                             distributedPaymentsInfo.PaymentPeriodEnd.Month - distributedPaymentsInfo.PaymentPeriodStart.Month + 1;
                for (int periodIndex = 0; periodIndex < paymentsForPeriodCount; periodIndex++)
                {
                    distributedPaymentsInfo.PaymentValue += orderPayablePlanDistributedPayments[distributedPaymentIndex++];
                }

                // переносим даты следующих платежей для всех, кроме последнего
                if (paymentIndex != distributedPaymentsInfos.Count - 1)
                {
                    var nextDistributedPaymentsInfo = distributedPaymentsInfos[paymentIndex + 1];
                    nextDistributedPaymentsInfo.PaymentPeriodStart = distributedPaymentsInfo.PaymentPeriodEnd.AddMonths(1).GetFirstDateOfMonth();
                    if (nextDistributedPaymentsInfo.PaymentPeriodStart > nextDistributedPaymentsInfo.PaymentPeriodEnd)
                    {
                        nextDistributedPaymentsInfo.PaymentPeriodEnd = nextDistributedPaymentsInfo.PaymentPeriodStart.AddMonths(1).AddSeconds(-1);
                    }
                }
            }
            var response = new GetDistributedPaymentsInfoResponse
                               {
                                   DistributedPaymentsInfos = distributedPaymentsInfos
                               };
            return response;
        }

        private static void CheckDates(IEnumerable<DistributedPaymentsInfo> initPaymentsInfos, DateTime beginDistributionDate, DateTime endDistributionDate)
        {
            var firstPayment = initPaymentsInfos.First();
            var lastPayment = initPaymentsInfos.Last();
            if (firstPayment.PaymentPeriodStart != beginDistributionDate)
            {
                throw new NotificationException(BLResources.FirstPaymentPeriodStartMustBeEqualToOrderBeginDistributionDate);
            }

            if (initPaymentsInfos.Except(new[] { lastPayment }).Any(x => x.PaymentPeriodEnd == endDistributionDate))
            {
                throw new NotificationException(
                    BLResources.EndDistributionDateCanOnlyMatchEndDistributionDateOfLastPayment);
            }

            if (lastPayment.PaymentPeriodEnd != endDistributionDate)
            {
                throw new NotificationException(BLResources.LastPaymentPeriodEndMustBeEqualToOrderEndDistributionDate);
            }

            if (initPaymentsInfos.Any(x => x.PaymentPeriodStart >= x.PaymentPeriodEnd))
            {
                throw new NotificationException(BLResources.PaymentPeriodStartMustBeLessThanPaymentPeriodEnd);
            }
        }
    }
}