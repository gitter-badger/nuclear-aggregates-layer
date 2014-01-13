﻿using System;

using DoubleGis.Erm.BLCore.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.Aggregates.Withdrawals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.WorkflowProcessing;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.WorkflowProcessing
{
    public sealed class ProcessOrderOnApprovedToOnTerminationHandler : RequestHandler<ProcessOrderOnApprovedToOnTerminationRequest, EmptyResponse>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IAccountRepository _accountRepository;
        private readonly IWithdrawalInfoRepository _withdrawalInfoRepository;
        private readonly IOrderRepository _orderRepository;

        public ProcessOrderOnApprovedToOnTerminationHandler(
            ISubRequestProcessor subRequestProcessor, 
            IAccountRepository accountRepository,
            IWithdrawalInfoRepository withdrawalInfoRepository,
            IOrderRepository orderRepository)
        {
            _subRequestProcessor = subRequestProcessor;
            _accountRepository = accountRepository;
            _withdrawalInfoRepository = withdrawalInfoRepository;
            _orderRepository = orderRepository;
        }

        protected override EmptyResponse Handle(ProcessOrderOnApprovedToOnTerminationRequest request)
        {
            var order = request.Order;

            if (request.Order == null)
            {
                throw new ArgumentException("Order must be supplied");
            }

            if (order.TerminationReason == (int)OrderTerminationReason.None)
            {
                throw new NotificationException(BLResources.TerminationReasonIsEmpty);
            }

            if (string.IsNullOrWhiteSpace(order.Comment) &&
                (order.TerminationReason == (int)OrderTerminationReason.RejectionOther || order.TerminationReason == (int)OrderTerminationReason.TemporaryRejectionOther))
            {
                throw new NotificationException(BLResources.ProcessOrderRejected_TerminationReasonNeeds);
            }

            // Проверить был ли выпуск по заказу (Факт наличия блокировки), если Да - продолжаем. Если не было выпуска по заказу доступ на перевод запрещён! 
            var nonDeletedLocksCount = _accountRepository.GetNonDeletedLocksCount(order.Id);
            if (nonDeletedLocksCount == 0)
            {
                throw new NotificationException(BLResources.ProcessOrder_LocksNotExist);
            }

            // Проверить если последний выпуск по заказу (Кол-во блокировок = Кол-во выпусков (план)), если Нет - продолжаем. Если последний выпуск по заказу доступ на перевод запрещён! 
            if (nonDeletedLocksCount == order.ReleaseCountPlan)
            {
                throw new NotificationException(BLResources.ProcessOrder_LastRelease);
            }

            // Пересчитать значения фактических атрибутов
            var releaseCountFact = (short)nonDeletedLocksCount;
            var releaseNumbersDto = _orderRepository.CalculateReleaseNumbers(order.DestOrganizationUnitId, order.BeginDistributionDate, order.ReleaseCountPlan, releaseCountFact);
            var distributionDatesDto = _orderRepository.CalculateDistributionDates(order.BeginDistributionDate, order.ReleaseCountPlan, releaseCountFact);

            order.ReleaseCountFact = releaseCountFact;
            order.EndDistributionDateFact = distributionDatesDto.EndDistributionDateFact;
            order.EndReleaseNumberFact = releaseNumbersDto.EndReleaseNumberFact;

            order.IsTerminated = true;
            order.RejectionDate = DateTime.UtcNow;

            // 3- Списание в счет оплаты БЗ 
            var debitForOrderPaymentSum = _accountRepository.CalculateDebitForOrderPaymentSum(order.Id);
            order.PayableFact = debitForOrderPaymentSum ?? 0.0m;

            var activeLocksCount = _accountRepository.GetActiveLocksCount(order.Id);
            if (activeLocksCount > 0)
            {
                var inactiveLocksCount = _accountRepository.GetInactiveLocksCount(order.Id);
                var releasesSum = _withdrawalInfoRepository.TakeAmountToWithdrawForOrder(order.Id, inactiveLocksCount, 1);
                order.PayableFact += releasesSum ?? 0.0m;
            }

            _orderRepository.Update(order); // The following two handlers need order and its extension saved

            _subRequestProcessor.HandleSubRequest(new CalculateReleaseWithdrawalsRequest { Order = request.Order }, Context);
            if (order.DealId.HasValue)
            {
                _subRequestProcessor.HandleSubRequest(new ActualizeDealProfitIndicatorsRequest { DealIds = new[] { order.DealId.Value } }, Context);
            }

            return Response.Empty;
        }
    }
}
