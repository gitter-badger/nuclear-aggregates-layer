using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing.Workflow;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing
{
    public class OrderWorkflowProcessingStrategy : OrderProcessingStrategy
    {
        private readonly ICommonLog _logger;
        private readonly IReleaseReadModel _releaseRepository;

        public OrderWorkflowProcessingStrategy(IUserContext userContext,
                                               IOrderRepository orderRepository,
                                               IUseCaseResumeContext<EditOrderRequest> resumeContext,
                                               IProjectService projectService,
                                               IOperationScope operationScope,
                                               IUserRepository userRepository,
                                               IOrderReadModel orderReadModel,
                                               ICommonLog logger,
                                               IReleaseReadModel releaseRepository)
            : base(userContext, orderRepository, resumeContext, projectService, operationScope, userRepository, orderReadModel)
        {
            _logger = logger;
            _releaseRepository = releaseRepository;
        }

        public override void FinishProcessing(Order order)
        {
            var originalOrderState = ResumeContext.Request.OriginalOrderState;
            if (!originalOrderState.HasValue)
            {
                throw new ArgumentException(BLResources.OriginalOrderStateNotSetDuringWorkflowProcessing);
            }

            var proposedOrderState = order.WorkflowStepId;
            var previousOrderState = order.WorkflowStepId;
            _logger.InfoFormat("Логика смены состояния заказа. Переход из [{0}] в [{1}].", previousOrderState, proposedOrderState);

            var orderStateBehaviour = new OrderStateBehaviourFactory(ResumeContext).GetOrderStateBehaviour(originalOrderState.Value, order);
            orderStateBehaviour.ChangeStateTo(proposedOrderState);
            OrderRepository.SetOrderState(order, proposedOrderState);
            OperationScope.Updated<Order>(order.Id);

            _logger.Debug("Логика смены состояния заказа - завершено");
        }

        protected override void ValidateOrderStateInternal(Order order, long currentUserCode)
        {
            if (order.WorkflowStepId == OrderState.Approved || order.WorkflowStepId == OrderState.OnTermination)
            {
                var isReleaseInProgress = _releaseRepository.HasFinalReleaseInProgress(
                    order.DestOrganizationUnitId,
                    new TimePeriod(order.BeginDistributionDate, order.EndDistributionDateFact));

                if (isReleaseInProgress)
                {
                    throw new ArgumentException(BLResources.OrderChangesDeniedByRelease);
                }
            }
        }
    }
}