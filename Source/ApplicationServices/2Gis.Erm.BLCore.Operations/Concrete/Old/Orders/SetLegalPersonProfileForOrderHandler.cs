﻿using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Transactions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders
{
    public sealed class SetLegalPersonProfileForOrderHandler : RequestHandler<ChangeOrderLegalPersonProfileRequest, EmptyResponse>
    {
        private readonly IOrderRepository _orderRepository;
        
        public SetLegalPersonProfileForOrderHandler(
            IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        protected override EmptyResponse Handle(ChangeOrderLegalPersonProfileRequest request)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var order = _orderRepository.GetOrder(request.OrderId);
                if (order == null)
                {
                    throw new NotificationException(BLResources.EntityNotFound);
                }

                order.LegalPersonProfileId = request.LegalPersonProfileId;
                _orderRepository.Update(order);
                
                transaction.Complete();
            }

            return Response.Empty;
        }
    }
}
