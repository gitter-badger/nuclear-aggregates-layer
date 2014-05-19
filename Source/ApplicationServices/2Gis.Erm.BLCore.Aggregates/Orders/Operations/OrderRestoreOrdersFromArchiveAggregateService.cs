﻿using System.Collections.Generic;

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
    public sealed class OrderRestoreOrdersFromArchiveAggregateService : IOrderRestoreOrdersFromArchiveAggregateService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public OrderRestoreOrdersFromArchiveAggregateService(
            IRepository<Order> orderRepository, 
            IOperationScopeFactory scopeFactory,
            ICommonLog logger)
        {
            _orderRepository = orderRepository;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public IEnumerable<ChangesDescriptor> Restore(IEnumerable<Order> orders)
        {
            var changes = new List<ChangesDescriptor>();

            _logger.InfoEx("Starting restoring orders from Archive");
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Order>())
            {
                foreach (var order in orders)
                {
                    var targetOrderState = order.IsTerminated ? OrderState.OnTermination : OrderState.Approved;

                    changes.Add(ChangesDescriptor.Create(order, x => x.WorkflowStepId, order.WorkflowStepId, (int)targetOrderState));

                    order.WorkflowStepId = (int)targetOrderState;

                    _orderRepository.Update(order);
                    scope.Updated<Order>(order.Id);
                }

                _orderRepository.Save();
                scope.Complete();
            }

            _logger.InfoFormatEx("Finished restoring orders from Archive. Processed order: {0}", changes.Count);
            return changes;
        }
    }
}