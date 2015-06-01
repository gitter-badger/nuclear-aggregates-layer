﻿using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations
{
    public sealed class OrderRestoreOrdersFromArchiveAggregateService : IOrderRestoreOrdersFromArchiveAggregateService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ITracer _tracer;

        public OrderRestoreOrdersFromArchiveAggregateService(
            IRepository<Order> orderRepository, 
            IOperationScopeFactory scopeFactory,
            ITracer tracer)
        {
            _orderRepository = orderRepository;
            _scopeFactory = scopeFactory;
            _tracer = tracer;
        }

        public IEnumerable<ChangesDescriptor> Restore(IEnumerable<Order> orders)
        {
            var changes = new List<ChangesDescriptor>();

            _tracer.Info("Starting restoring orders from Archive");
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Order>())
            {
                foreach (var order in orders)
                {
                    var targetOrderState = order.IsTerminated ? OrderState.OnTermination : OrderState.Approved;

                    changes.Add(ChangesDescriptor.Create(order, x => x.WorkflowStepId, order.WorkflowStepId, targetOrderState));

                    order.WorkflowStepId = targetOrderState;

                    _orderRepository.Update(order);
                    scope.Updated<Order>(order.Id);
                }

                _orderRepository.Save();
                scope.Complete();
            }

            _tracer.InfoFormat("Finished restoring orders from Archive. Processed order: {0}", changes.Count);
            return changes;
        }
    }
}