using DoubleGis.Erm.BLCore.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
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

        public OrderCreationStrategy(IUserContext userContext,
                                     IOrderRepository orderRepository,
                                     IUseCaseResumeContext<EditOrderRequest> resumeContext,
                                     IProjectService projectService,
                                     IOperationScope operationScope,
                                     IUserRepository userRepository,
                                     IOrderReadModel orderReadModel,
                                     IAccountRepository accountRepository)
            : base(userContext, orderRepository, resumeContext, projectService, operationScope, userRepository, orderReadModel)
        {
            _accountRepository = accountRepository;
        }

        public override void FinishProcessing(Order order)
        {
            OrderRepository.Create(order);
            OperationScope.Added<Order>(order.Id);
        }

        protected override void ActualizeOrderNumber(Order order, long? reservedNumberDigit)
        {
            if (string.IsNullOrEmpty(order.Number))
            {
                var response = (GenerateOrderNumberResponse)ResumeContext.UseCaseResume(new GenerateOrderNumberRequest
                    {
                        Order = order,
                        ReservedNumber = reservedNumberDigit
                    });
                order.Number = response.Number;
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

            var request = new GenerateOrderNumberRequest { Order = order, ReservedNumber = reservedNumberDigit, IsRegionalNumber = true };
            var regResponse = (GenerateOrderNumberResponse)ResumeContext.UseCaseResume(request);
            order.RegionalNumber = regResponse.Number;
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