﻿using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations
{
    public sealed class SpecifyOrderDocumentsDebtAggregateService : ISpecifyOrderDocumentsDebtAggregateService
    {
        private readonly ISecureRepository<Order> _orderRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public SpecifyOrderDocumentsDebtAggregateService(ISecureRepository<Order> orderRepository, IOperationScopeFactory scopeFactory)
        {
            _orderRepository = orderRepository;
            _scopeFactory = scopeFactory;
        }

        public void Specify(Order order, DocumentsDebt documentsDebt, string documentsDebtComment)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Order>())
            {
                order.DocumentsComment = documentsDebtComment;
                order.HasDocumentsDebt = documentsDebt;
                _orderRepository.Update(order);
                _orderRepository.Save();

                scope.Updated(order)
                     .Complete();
            }
        }
    }
}