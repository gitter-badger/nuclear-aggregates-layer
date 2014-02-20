﻿using System;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing.Workflow;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

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

            var proposedOrderState = (OrderState)order.WorkflowStepId;

            #region Logging
            var previousOrderState = (OrderState)order.WorkflowStepId;
            _logger.InfoFormatEx("Логика смены состояния заказа. Переход из [{0}] в [{1}].", previousOrderState, proposedOrderState);
            #endregion

            var orderStateBehaviour = new OrderStateBehaviourFactory(ResumeContext).GetOrderStateBehaviour(originalOrderState.Value, order);
            orderStateBehaviour.ChangeStateTo(proposedOrderState);
            OrderRepository.SetOrderState(order, proposedOrderState);
            OperationScope.Updated<Order>(order.Id);

            #region Logging
            _logger.DebugEx("Логика смены состояния заказа - завершено");
            #endregion
        }

        protected override void ValidateOrderStateInternal(Order order, long currentUserCode)
        {
            if (order.WorkflowStepId == (int)OrderState.Approved || order.WorkflowStepId == (int)OrderState.OnTermination)
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