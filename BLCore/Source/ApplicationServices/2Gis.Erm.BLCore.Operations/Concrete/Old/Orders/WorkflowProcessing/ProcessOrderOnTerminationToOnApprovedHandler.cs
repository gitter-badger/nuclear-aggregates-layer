using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.WorkflowProcessing;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Bargains;
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
        private readonly IOrderReadModel _orderReadModel;

        public ProcessOrderOnTerminationToOnApprovedHandler(
            ISubRequestProcessor subRequestProcessor, 
            IReleaseReadModel releaseRepository,
            IOrderRepository orderRepository, 
            IOrderReadModel orderReadModel)
        {
            _subRequestProcessor = subRequestProcessor;
            _releaseRepository = releaseRepository;
            _orderRepository = orderRepository;
            _orderReadModel = orderReadModel;
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
            var releaseNumbersDto = _orderReadModel.CalculateReleaseNumbers(order.DestOrganizationUnitId, order.BeginDistributionDate, order.ReleaseCountPlan, releaseCountFact);
            var distributionDatesDto = _orderReadModel.CalculateDistributionDates(order.BeginDistributionDate, order.ReleaseCountPlan, releaseCountFact);

            if (order.BargainId.HasValue)
            {
                var bargainDates = _orderReadModel.GetBargainEndAndCloseDates(order.BargainId.Value);
                if (bargainDates.BargainEndDate.HasValue && bargainDates.BargainEndDate.Value < distributionDatesDto.EndDistributionDateFact)
                {
                    throw new BargainEndDateIsLessThanOrderEndDistributionDateException(BLResources.BargainEndDateIsLessThanOrderEndDistributionDate);
                }

                if (bargainDates.BargainCloseDate.HasValue && bargainDates.BargainCloseDate.Value < distributionDatesDto.EndDistributionDateFact)
                {
                    throw new BargainCloseDateIsLessThanOrderEndDistributionDateException(BLResources.BargainCloseDateIsLessThanOrderEndDistributionDate);
                }
            }

            order.ReleaseCountFact = releaseCountFact;
            order.EndDistributionDateFact = distributionDatesDto.EndDistributionDateFact;
            order.EndReleaseNumberFact = releaseNumbersDto.EndReleaseNumberFact;

            order.IsTerminated  = false;
            order.RejectionDate = null;
            order.PayableFact = order.PayablePlan;
            order.TerminationReason = OrderTerminationReason.None;
            order.Comment = null;

            _subRequestProcessor.HandleSubRequest(new ActualizeOrderReleaseWithdrawalsRequest { Order = request.Order }, Context);

            _orderRepository.Update(order);

            return Response.Empty;
        }
    }
}
