﻿using System;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.Aggregates.Releases.ReadModel;
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
    public sealed class ProcessOrderOnTerminationToOnApprovedHandler : RequestHandler<ProcessOrderOnTerminationToOnApprovedRequest, EmptyResponse>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IReleaseReadModel _releaseRepository;
        private readonly IOrderRepository _orderRepository;

        public ProcessOrderOnTerminationToOnApprovedHandler(
            ISubRequestProcessor subRequestProcessor, 
            IReleaseReadModel releaseRepository,
            IOrderRepository orderRepository)
        {
            _subRequestProcessor = subRequestProcessor;
            _releaseRepository = releaseRepository;
            _orderRepository = orderRepository;
        }

        protected override EmptyResponse Handle(ProcessOrderOnTerminationToOnApprovedRequest request)
        {
            var order = request.Order;

            if (order == null)
            {
                throw new ArgumentException("Order must be supplied");
            }

            // Проверить на принадлежность заказа к текущему выпуску.
            if (order.RejectionDate.HasValue)
            {
                var releaseExists = _releaseRepository.HasFinalReleaseAfterDate(order.DestOrganizationUnitId, order.RejectionDate.Value);
                if (releaseExists)
                {
                    throw new NotificationException(BLResources.ProcessOrder_OrderHasReleasesWhenRejectedToApproved);
                }
            }
            
            // Пересчетать значения фактических атрибутов
            var releaseCountFact = order.ReleaseCountPlan;
            var releaseNumbersDto = _orderRepository.CalculateReleaseNumbers(order.DestOrganizationUnitId, order.BeginDistributionDate, order.ReleaseCountPlan, releaseCountFact);
            var distributionDatesDto = _orderRepository.CalculateDistributionDates(order.BeginDistributionDate, order.ReleaseCountPlan, releaseCountFact);

            order.ReleaseCountFact = releaseCountFact;
            order.EndDistributionDateFact = distributionDatesDto.EndDistributionDateFact;
            order.EndReleaseNumberFact = releaseNumbersDto.EndReleaseNumberFact;

            order.IsTerminated  = false;
            order.RejectionDate = null;
            order.PayableFact = order.PayablePlan;
            order.TerminationReason = (int)OrderTerminationReason.None;
            order.Comment = null;

            _subRequestProcessor.HandleSubRequest(new CalculateReleaseWithdrawalsRequest { Order = request.Order }, Context);

            _orderRepository.Update(order);
            if (order.DealId.HasValue)
            {
                _subRequestProcessor.HandleSubRequest(new ActualizeDealProfitIndicatorsRequest { DealIds = new[] { order.DealId.Value } }, Context);
            }

            return Response.Empty;
        }
    }
}
