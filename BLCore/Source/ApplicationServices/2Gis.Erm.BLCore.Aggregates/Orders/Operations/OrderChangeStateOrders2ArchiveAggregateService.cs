using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations
{
    public sealed class OrderChangeStateOrders2ArchiveAggregateService : IOrderChangeStateOrders2ArchiveAggregateService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public OrderChangeStateOrders2ArchiveAggregateService(
            IRepository<Order> orderRepository, 
            IOperationScopeFactory scopeFactory,
            ICommonLog logger)
        {
            _orderRepository = orderRepository;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public IEnumerable<ChangesDescriptor> Archiving(IEnumerable<Order> orders)
        {
            var changes = new List<ChangesDescriptor>();

            _logger.Info("Starting changing orders workflow step to Archive");
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Order>())
            {
                foreach (var order in orders)
                {
                    changes.Add(ChangesDescriptor.Create(order, x => x.WorkflowStepId, order.WorkflowStepId, OrderState.Archive));

                    order.WorkflowStepId = OrderState.Archive;
                    order.AmountToWithdraw = 0m;

                    _orderRepository.Update(order);
                    scope.Updated<Order>(order.Id);
                }

                _orderRepository.Save();
                scope.Complete();
            }

            _logger.InfoFormat("Finished changing orders workflow step to Archive. Processed order: {0}", changes.Count);
            return changes;
        }
    }
}