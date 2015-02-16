using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders
{
    public class ActualizeOrderAmountToWithdrawOperationService : IActualizeOrderAmountToWithdrawOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOrderActualizeAmountToWithdrawAggregateService _actualizeAmountToWithdrawAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;

        public ActualizeOrderAmountToWithdrawOperationService(
            IOrderReadModel orderReadModel,
            IOrderActualizeAmountToWithdrawAggregateService actualizeAmountToWithdrawAggregateService,
            IOperationScopeFactory scopeFactory)
        {
            _orderReadModel = orderReadModel;
            _actualizeAmountToWithdrawAggregateService = actualizeAmountToWithdrawAggregateService;
            _scopeFactory = scopeFactory;
        }

        public void Actualize(long orderId)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<ActualizeOrderAmountToWithdrawIdentity>())
            {
                var orderAmountToWithdrawnInfo = _orderReadModel.GetOrderAmountToWithdrawInfo(orderId);
                _actualizeAmountToWithdrawAggregateService.Actualize(orderAmountToWithdrawnInfo.Order, orderAmountToWithdrawnInfo.AmountToWithdraw);
                scope.Complete();
            }
        }
    }
}
