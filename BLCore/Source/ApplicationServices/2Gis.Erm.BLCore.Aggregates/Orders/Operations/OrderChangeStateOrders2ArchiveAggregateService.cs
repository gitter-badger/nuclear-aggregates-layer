using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations
{
    public sealed class OrderChangeStateOrders2ArchiveAggregateService : IOrderChangeStateOrders2ArchiveAggregateService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ITracer _tracer;

        public OrderChangeStateOrders2ArchiveAggregateService(
            IRepository<Order> orderRepository, 
            IOperationScopeFactory scopeFactory,
            ITracer tracer)
        {
            _orderRepository = orderRepository;
            _scopeFactory = scopeFactory;
            _tracer = tracer;
        }

        public IEnumerable<ChangesDescriptor> Archiving(IEnumerable<Order> orders)
        {
            var changes = new List<ChangesDescriptor>();

            _tracer.Info("Starting changing orders workflow step to Archive");
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

            _tracer.InfoFormat("Finished changing orders workflow step to Archive. Processed order: {0}", changes.Count);
            return changes;
        }
    }
}