using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations
{
    public sealed class OrderActualizeAmountToWithdrawAggregateService : IOrderActualizeAmountToWithdrawAggregateService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public OrderActualizeAmountToWithdrawAggregateService(IRepository<Order> orderRepository, IOperationScopeFactory scopeFactory)
        {
            _orderRepository = orderRepository;
            _scopeFactory = scopeFactory;
        }

        public void Actualize(Order order, decimal amountToWithdraw)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<ActualizeOrderAmountToWithdrawIdentity>())
            {
                order.AmountToWithdraw = order.WorkflowStepId == OrderState.Archive ? 0m : amountToWithdraw;
                _orderRepository.Update(order);

                _orderRepository.Save();

                scope.Updated<Order>(order.Id)
                     .Complete();
            }
        }
    }
}