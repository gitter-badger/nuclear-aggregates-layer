using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.OrderProcessing;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditOrderHandler : RequestHandler<EditOrderRequest, EmptyResponse>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IReleaseReadModel _releaseRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public EditOrderHandler(
            ISubRequestProcessor subRequestProcessor,
            IOrderProcessingService orderProcessingService,
            IReleaseReadModel releaseRepository,
            IOperationScopeFactory scopeFactory)
        {
            _subRequestProcessor = subRequestProcessor;
            _orderProcessingService = orderProcessingService;
            _releaseRepository = releaseRepository;
            _scopeFactory = scopeFactory;
        }

        protected override EmptyResponse Handle(EditOrderRequest request)
        {
            var order = request.Entity;
            if (!order.IsActive)
            {
                throw new ArgumentException("Cannot save inactive Order");
            }

            if (order.WorkflowStepId == (int)OrderState.Approved || order.WorkflowStepId == (int)OrderState.OnTermination)
            {
                var isReleaseInProgress = _releaseRepository.HasFinalReleaseInProgress(
                    order.DestOrganizationUnitId,
                    new TimePeriod(order.BeginDistributionDate, order.EndDistributionDateFact));

                if (isReleaseInProgress)
                {
                    throw new NotificationException(BLResources.OrderChangesDeniedByRelease);
                }
            }

            using (var operationScope = _scopeFactory.CreateOrUpdateOperationFor(order))
            {
                var strategies =
                    _orderProcessingService.EvaluateProcessingStrategies(new UseCaseResumeContext<EditOrderRequest>(_subRequestProcessor, Context, request), operationScope);

                _orderProcessingService.ExecuteProcessingStrategies(strategies, order);

                operationScope.Complete();
            }

            return Response.Empty;
        }
    }
}
