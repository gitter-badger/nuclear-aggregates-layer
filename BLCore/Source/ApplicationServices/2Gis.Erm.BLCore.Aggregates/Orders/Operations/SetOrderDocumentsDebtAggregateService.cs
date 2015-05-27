using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations
{
    public sealed class SetOrderDocumentsDebtAggregateService : ISetOrderDocumentsDebtAggregateService
    {
        private readonly ISecureRepository<Order> _orderRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public SetOrderDocumentsDebtAggregateService(ISecureRepository<Order> orderRepository, IOperationScopeFactory scopeFactory)
        {
            _orderRepository = orderRepository;
            _scopeFactory = scopeFactory;
        }

        public void Set(Order order, DocumentsDebt documentsDebt, string documentsDebtComment)
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