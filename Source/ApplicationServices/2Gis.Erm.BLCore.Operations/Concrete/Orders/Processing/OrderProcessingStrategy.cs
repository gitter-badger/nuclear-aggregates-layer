﻿using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.OrderProcessing;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing
{
    public abstract class OrderProcessingStrategy : IOrderProcessingStrategy
    {
        protected readonly IOperationScope OperationScope;
        private readonly IUserContext _userContext;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IProjectService _projectService;
        private readonly IUserRepository _userRepository;
        private readonly IUseCaseResumeContext<EditOrderRequest> _resumeContext;

        protected OrderProcessingStrategy(
            IUserContext userContext,
            IOrderRepository orderRepository,
            IUseCaseResumeContext<EditOrderRequest> resumeContext,
            IProjectService projectService,
            IOperationScope operationScope,
            IUserRepository userRepository,
            IOrderReadModel orderReadModel)
        {
            _userContext = userContext;
            _orderRepository = orderRepository;
            _resumeContext = resumeContext;
            _projectService = projectService;
            OperationScope = operationScope;
            _userRepository = userRepository;
            _orderReadModel = orderReadModel;
        }

        protected IOrderRepository OrderRepository
        {
            get
            {
                return _orderRepository;
            }
        }

        protected IUseCaseResumeContext<EditOrderRequest> ResumeContext
        {
            get
            {
                return _resumeContext;
            }
        }

        protected IOrderReadModel OrderReadModel
        {
            get { return _orderReadModel; }
        }

        public void Validate(Order order)
        {
            var currentUserCode = _userContext.Identity.Code;

            if (order.SignupDate >= order.BeginDistributionDate)
            {
                throw new ArgumentException(BLResources.OrderValidateSignupDateMustBeLessThanBeginDistributionDate);
            }

            if (((order.DiscountPercent.HasValue && order.DiscountPercent != 0m) || (order.DiscountSum.HasValue && order.DiscountSum != 0m)) 
                && order.DiscountReasonEnum == (int)OrderDiscountReason.None)
            {
                throw new ArgumentException(BLResources.OrderValidateDiscountReasonRequired);
            }

            if (order.DiscountSum.HasValue && (order.DiscountSum < 0))
            {
                throw new ArgumentException(BLResources.OrderValidateDiscountNegative);
            }

            if (order.DiscountPercent.HasValue && (order.DiscountPercent < 0 || order.DiscountPercent > 100))
            {
                throw new ArgumentException(BLResources.OrderValidateDiscountPercentInvalid);
            }

            var projects = _projectService.GetProjectsByOrganizationUnit(order.DestOrganizationUnitId);
            if (!projects.Any())
            {
                throw new ArgumentException(BLResources.OrderValidateDestOrganizationUnitHasNoProject);
            }

            var sourceOrganizationUnit = _userRepository.GetOrganizationUnit(order.SourceOrganizationUnitId);
            if (!sourceOrganizationUnit.IsActive || sourceOrganizationUnit.IsDeleted)
            {
                throw new ArgumentException(BLResources.SourceOrganizationUnitIsInactive);
            }

            var destOrganizationUnit = _userRepository.GetOrganizationUnit(order.DestOrganizationUnitId);
            if (!destOrganizationUnit.IsActive || destOrganizationUnit.IsDeleted)
            {
                throw new ArgumentException(BLResources.DestOrganizationUnitIsInactive);
            }

            ValidateOrderStateInternal(order, currentUserCode);
        }

        public abstract void FinishProcessing(Order order);

        public void Process(Order order)
        {
            OrderReadModel.UpdateOrderDistributionDates(order);
            OrderReadModel.UpdateOrderReleaseNumbers(order);

            UpdateFinancialInformation(order);
            var reservedNumberDigit = ResumeContext.Request.ReservedNumberDigit;
            ActualizeOrderNumber(order, reservedNumberDigit);
            UpdateDeal(order);
            DetermineOrderBudgetType(order);
            DetermineOrderPlatform(order);
            CreateAccount(order);
        }

        protected virtual void ActualizeOrderNumber(Order order, long? reservedNumberDigit)
        {
        }

        protected virtual void DetermineOrderPlatform(Order order)
        {
        }

        protected virtual void UpdateFinancialInformation(Order order)
        {
        }

        protected virtual void UpdateDeal(Order order)
        {
        }

        protected virtual void DetermineOrderBudgetType(Order order)
        {
        }

        protected virtual void CreateAccount(Order order)
        {
        }

        protected virtual void ValidateOrderStateInternal(Order order, long currentUserCode)
        {
        }
    }
}