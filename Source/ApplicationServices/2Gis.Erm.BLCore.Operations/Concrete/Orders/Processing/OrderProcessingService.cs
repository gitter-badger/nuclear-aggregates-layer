﻿using System;
using System.Collections.Generic;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.OrderProcessing;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing
{
    public class OrderProcessingService : IOrderProcessingService
    {
        private readonly IUserContext _userContext;
        private readonly ICommonLog _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IAccountRepository _accountRepository;
        private readonly IReleaseReadModel _releaseRepository;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserRepository _userRepository;
        private readonly IProjectService _projectService;
        private readonly IEvaluateOrderNumberService _numberService;

        public OrderProcessingService(
            IUserContext userContext,
            ICommonLog logger,
            IOrderRepository orderRepository,
            IAccountRepository accountRepository,
            IReleaseReadModel releaseRepository,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IProjectService projectService,
            IUserRepository userRepository,
            IOrderReadModel orderReadModel, 
            IEvaluateOrderNumberService numberService)
        {
            _userContext = userContext;
            _logger = logger;
            _orderRepository = orderRepository;
            _accountRepository = accountRepository;
            _releaseRepository = releaseRepository;
            _functionalAccessService = functionalAccessService;
            _projectService = projectService;
            _userRepository = userRepository;
            _orderReadModel = orderReadModel;
            _numberService = numberService;
        }

        public IOrderProcessingStrategy[] EvaluateProcessingStrategies(IUseCaseResumeContext<EditOrderRequest> resumeContext, IOperationScope operationScope)
        {
            var order = resumeContext.Request.Entity;
            var originalOrderState = resumeContext.Request.OriginalOrderState;
            var proposedOrderState = (OrderState)order.WorkflowStepId;

            if (!Enum.IsDefined(typeof(OrderState), proposedOrderState))
            {
                throw new ArgumentException(string.Format(BLResources.UnrecognizedOrderState, proposedOrderState));
            }

            var involvedStrategies = new List<IOrderProcessingStrategy>();
            if (order.IsNew())
            {
                involvedStrategies.Add(new OrderCreationStrategy(_userContext,
                                                                 _orderRepository,
                                                                 resumeContext,
                                                                 _projectService,
                                                                 operationScope,
                                                                 _userRepository,
                                                                 _orderReadModel,
                                                                 _accountRepository,
                                                                 _numberService));
                return involvedStrategies.ToArray();
            }

            if (originalOrderState == OrderState.OnRegistration || originalOrderState == proposedOrderState)
            {
                involvedStrategies.Add(new OrderEditingStrategy(_userContext,
                                                                _orderRepository,
                                                                resumeContext,
                                                                _projectService,
                                                                operationScope,
                                                                _userRepository,
                                                                _orderReadModel,
                                                                _logger,
                                                                _releaseRepository,
                                                                _accountRepository,
                                                                _functionalAccessService,
                                                                _numberService));
            }

            if (originalOrderState != proposedOrderState)
            {
                involvedStrategies.Add(new OrderWorkflowProcessingStrategy(_userContext,
                                                                           _orderRepository,
                                                                           resumeContext,
                                                                           _projectService,
                                                                           operationScope,
                                                                           _userRepository,
                                                                           _orderReadModel,
                                                                           _logger,
                                                                           _releaseRepository));
            }

            return involvedStrategies.ToArray();
        }

        public void ExecuteProcessingStrategies(IOrderProcessingStrategy[] strategies, Order order)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
                {
                    foreach (var strategy in strategies)
                    {
                        strategy.Validate(order);
                    }

                    foreach (var strategy in strategies)
                    {
                        strategy.Process(order);
                    }

                    foreach (var strategy in strategies)
                    {
                        strategy.FinishProcessing(order);
                    }

                    transaction.Complete();
                }
            }
            catch (ArgumentException ex)
            {
                throw new NotificationException(ex.Message, ex);
            }
        }
    }
}
