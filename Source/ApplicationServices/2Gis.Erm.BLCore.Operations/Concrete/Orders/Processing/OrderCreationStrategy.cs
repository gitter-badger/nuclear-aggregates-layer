using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing
{
    public class OrderCreationStrategy : OrderProcessingStrategy
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEvaluateOrderNumberService _numberService;

        public OrderCreationStrategy(IUserContext userContext,
                                     IOrderRepository orderRepository,
                                     IUseCaseResumeContext<EditOrderRequest> resumeContext,
                                     IProjectService projectService,
                                     IOperationScope operationScope,
                                     IUserRepository userRepository,
                                     IOrderReadModel orderReadModel,
                                     IAccountRepository accountRepository,
                                     IEvaluateOrderNumberService numberService)
            : base(userContext, orderRepository, resumeContext, projectService, operationScope, userRepository, orderReadModel)
        {
            _accountRepository = accountRepository;
            _numberService = numberService;
        }

        public override void FinishProcessing(Order order)
        {
            OrderRepository.Create(order);
            OperationScope.Added<Order>(order.Id);
        }

        protected override void ActualizeOrderNumber(Order order, long? reservedNumberDigit)
        {
            var syncCodes = OrderReadModel.GetOrderOrganizationUnitsSyncCodes(order.SourceOrganizationUnitId, order.DestOrganizationUnitId);

            if (string.IsNullOrEmpty(order.Number))
            {
                order.Number = _numberService.Evaluate(order.Number,
                                                       syncCodes[order.SourceOrganizationUnitId],
                                                       syncCodes[order.DestOrganizationUnitId],
                                                       reservedNumberDigit);
            }

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

        protected override void UpdateDeal(Order order)
        {
            if (!order.DealId.HasValue)
            {
                return;
            }

            ResumeContext.UseCaseResume(new CheckDealRequest { DealId = order.DealId.Value, CurrencyId = order.CurrencyId.Value });
            ResumeContext.UseCaseResume(new ActualizeDealProfitIndicatorsRequest { DealIds = new[] { order.DealId.Value } });
            OperationScope.Updated<Deal>(order.DealId.Value);
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
    }
}