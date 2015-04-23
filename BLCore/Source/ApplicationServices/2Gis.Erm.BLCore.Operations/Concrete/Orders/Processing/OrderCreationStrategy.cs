using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing
{
    public class OrderCreationStrategy : OrderProcessingStrategy
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEvaluateOrderNumberService _numberService;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        // TODO {all, 19.01.2015}: Есть смысл в этих стратегиях не использовать ReadModel, а передавать уже считанные данные.
        public OrderCreationStrategy(IUserContext userContext,
                                     IOrderRepository orderRepository,
                                     IUseCaseResumeContext<EditOrderRequest> resumeContext,
                                     IProjectService projectService,
                                     IOperationScope operationScope,
                                     IUserRepository userRepository,
                                     IOrderReadModel orderReadModel,
                                     IAccountRepository accountRepository,
                                     IEvaluateOrderNumberService numberService,
                                     ILegalPersonReadModel legalPersonReadModel)
            : base(userContext, orderRepository, resumeContext, projectService, operationScope, userRepository, orderReadModel)
        {
            _accountRepository = accountRepository;
            _numberService = numberService;
            _legalPersonReadModel = legalPersonReadModel;
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