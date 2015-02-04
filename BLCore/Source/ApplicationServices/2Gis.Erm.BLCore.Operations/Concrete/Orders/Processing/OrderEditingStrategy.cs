using System;
using System.Linq;
using System.Security;
using System.Text;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing
{
    public class OrderEditingStrategy : OrderProcessingStrategy
    {
        private static readonly Action<bool, string, StringBuilder> CheckField = (mismatch, fieldName, message) =>
        {
            if (mismatch)
            {
                if (message.Length > 0)
                {
                    message.Append(BLResources.ListSeparator);
                }

                message.Append(fieldName);
            }
        };

        private readonly ICommonLog _logger;
        private readonly IReleaseReadModel _releaseRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IEvaluateOrderNumberService _numberService;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        // TODO {all, 19.01.2015}: Есть смысл в этих стратегиях не использовать ReadModel, а передавать уже считанные данные.
        public OrderEditingStrategy(IUserContext userContext,
                                    IOrderRepository orderRepository,
                                    IUseCaseResumeContext<EditOrderRequest> resumeContext,
                                    IProjectService projectService,
                                    IOperationScope operationScope,
                                    IUserRepository userRepository,
                                    IOrderReadModel orderReadModel,
                                    ICommonLog logger,
                                    IReleaseReadModel releaseRepository,
                                    IAccountRepository accountRepository,
                                    ISecurityServiceFunctionalAccess functionalAccessService,
                                    IEvaluateOrderNumberService numberService,
                                    ILegalPersonReadModel legalPersonReadModel)
            : base(userContext, orderRepository, resumeContext, projectService, operationScope, userRepository, orderReadModel)
        {
            _logger = logger;
            _releaseRepository = releaseRepository;
            _accountRepository = accountRepository;
            _functionalAccessService = functionalAccessService;
            _numberService = numberService;
            _legalPersonReadModel = legalPersonReadModel;
        }

        public override void FinishProcessing(Order order)
        {
            OrderRepository.Update(order);
            OperationScope.Updated<Order>(order.Id);
        }

        protected override void ValidateOrderStateInternal(Order order, long currentUserCode)
        {
            // todo {all, 2013-08-05}: эта самая проверка выполняется в хендлере. действительно есть необходимость её выпонять ещё раз?
            if (order.WorkflowStepId == OrderState.Approved || order.WorkflowStepId == OrderState.OnTermination)
            {
                var isReleaseInProgress = _releaseRepository.HasFinalReleaseInProgress(order.DestOrganizationUnitId,
                                                                                       new TimePeriod(order.BeginDistributionDate, order.EndDistributionDateFact));
                if (isReleaseInProgress)
                {
                    throw new ArgumentException(BLResources.OrderChangesDeniedByRelease);
                }
            }

            var orderStateValidationInfo = OrderReadModel.GetOrderStateValidationInfo(order.Id);
            if (orderStateValidationInfo == null)
            {
                return;
            }

            // Проверка залоченного на карточке поля на достижиомсть 
            if (order.HasDocumentsDebt != orderStateValidationInfo.HasDocumentsDebt)
            {
                var hasPrivilege = _functionalAccessService.HasOrderChangeDocumentsDebtAccess(order.SourceOrganizationUnitId, currentUserCode);
                var isReleaseInProgress = _releaseRepository.HasFinalReleaseInProgress(order.DestOrganizationUnitId,
                                                                             new TimePeriod(order.BeginDistributionDate, order.EndDistributionDateFact));
                if (hasPrivilege && !isReleaseInProgress)
                {
                    return;
                }

                _logger.WarnFormat(
                    "Попытка изменить флаг 'Работа с задолженностью по документам' в заказе [{0}] со значения [{1}] на [{2}]",
                    order.Id, 
                    orderStateValidationInfo.HasDocumentsDebt, 
                    order.HasDocumentsDebt);
                throw new SecurityException(BLResources.YouHasNoEntityAccessPrivilege);
            }

            if (!orderStateValidationInfo.AnyPositions)
            {
                return;
            }

            var message = new StringBuilder();
            CheckField(order.SourceOrganizationUnitId != orderStateValidationInfo.SourceOrganizationUnitId, MetadataResources.SourceOrganizationUnit, message);
            CheckField(order.DestOrganizationUnitId != orderStateValidationInfo.DestOrganizationUnitId, MetadataResources.DestinationOrganizationUnit, message);
            CheckField(order.LegalPersonId != orderStateValidationInfo.LegalPersonId, MetadataResources.LegalPerson, message);
            CheckField(order.BranchOfficeOrganizationUnitId != orderStateValidationInfo.BranchOfficeOrganizationUnitId,
                       MetadataResources.BranchOfficeOrganizationUnitName,
                       message);

            if (message.Length > 0)
            {
                throw new ArgumentException(BLResources.TheFollowingFieldsCannotBeChangedWhenOrderHasPositions + message);
            }
        }

        protected override void ActualizeOrderNumber(Order order, long? reservedNumberDigit)
        {
            if (!string.IsNullOrEmpty(order.Number))
            {
                var isTheSameDirection = OrderReadModel.IsOrderForOrganizationUnitsPairExist(order.Id,
                                                                                               order.SourceOrganizationUnitId,
                                                                                               order.DestOrganizationUnitId);
                if (isTheSameDirection)
                {
                    return;
                }
            }

            var syncCodes = OrderReadModel.GetOrderOrganizationUnitsSyncCodes(order.SourceOrganizationUnitId, order.DestOrganizationUnitId);
            order.Number = _numberService.Evaluate(order.Number,
                                                   syncCodes[order.SourceOrganizationUnitId],
                                                   syncCodes[order.DestOrganizationUnitId],
                                                   reservedNumberDigit);
            order.RegionalNumber = null;

            if (order.SourceOrganizationUnitId == order.DestOrganizationUnitId)
            {
                return;
            }

            var isOrganizationUnitsBothBranches = OrderReadModel.IsOrganizationUnitsBothBranches(order.SourceOrganizationUnitId, order.DestOrganizationUnitId);
            if (isOrganizationUnitsBothBranches)
            {
                return;
            }

            order.RegionalNumber = _numberService.EvaluateRegional(order.Number,
                                                                   syncCodes[order.SourceOrganizationUnitId],
                                                                   syncCodes[order.DestOrganizationUnitId],
                                                                   reservedNumberDigit);
        }

        protected override void UpdateFinancialInformation(Order order)
        {
            #region Logging

            _logger.InfoFormat("Обновление скидки заказа [{0}]", order.Id);

            #endregion

            ResumeContext.UseCaseResume(new UpdateOrderDiscountRequest { Order = order, DiscountInPercents = ResumeContext.Request.DiscountInPercents });

            #region Logging

            _logger.Debug("Обновление скидки заказа - завершено");

            #endregion

            #region Logging

            _logger.InfoFormat("Обновление остатков по заказу [{0}]", order.Id);

            #endregion

            ResumeContext.UseCaseResume(new ActualizeOrderReleaseWithdrawalsRequest { Order = order });

            #region Logging

            _logger.Debug("Обновление остатков по заказу - завершено");

            #endregion
        }

        protected override void UpdateDeal(Order order)
        {
            if (!order.DealId.HasValue)
            {
                return;
            }

            ResumeContext.UseCaseResume(new CheckDealRequest { DealId = order.DealId.Value, CurrencyId = order.CurrencyId.Value });
            OperationScope.Updated<Deal>(order.DealId.Value);
        }

        protected override void DetermineOrderPlatform(Order order)
        {
            order.PlatformId = OrderReadModel.EvaluateOrderPlatformId(order.Id);
        }

        protected override void CreateAccount(Order order)
        {
            if (!order.LegalPersonId.HasValue || !order.BranchOfficeOrganizationUnitId.HasValue)
            {
                return;
            }

            var account = _accountRepository.CreateAccount(order.LegalPersonId.Value, order.BranchOfficeOrganizationUnitId.Value);
            order.AccountId = account.Id;
            OperationScope.Added<Account>(order.AccountId.Value);
        }

        protected override void UpdateDefaultProfile(Order order)
        {
            if (order.LegalPersonId.HasValue && order.LegalPersonProfileId == null)
            {
                var profiles = _legalPersonReadModel.GetLegalPersonProfileIds(order.LegalPersonId.Value).ToList();
                if (profiles.Count() == 1)
                {
                    order.LegalPersonProfileId = profiles.Single();
                }
            }
        }
    }
}