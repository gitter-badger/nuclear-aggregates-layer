using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders
{
    public sealed class RemoveBargainFromOrderHandler : RequestHandler<RemoveBargainFromOrderRequest, EmptyResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IBargainRepository _bargainRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public RemoveBargainFromOrderHandler(IOrderRepository orderRepository,
                                             IBargainRepository bargainRepository,
                                             IOperationScopeFactory scopeFactory,
                                             IOrderReadModel orderReadModel)
        {
            _orderRepository = orderRepository;
            _bargainRepository = bargainRepository;
            _scopeFactory = scopeFactory;
            _orderReadModel = orderReadModel;
        }

        protected override EmptyResponse Handle(RemoveBargainFromOrderRequest request)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var orderInfo = _orderReadModel.GetOrderUsage(request.OrderId);

                var order = orderInfo.Order;
                if (!order.BargainId.HasValue)
                {
                    throw new NotificationException(BLResources.BargainForOrderNotFound);
                }

                if (orderInfo.AnyLocks)
                {
                    throw new NotificationException(BLResources.CannotRemoveBargainBecauseLocksForOrderNotFound);
                }

                var bargainInfo = _bargainRepository.GetBargainUsage(order.BargainId.Value);

                using (var operationScope = _scopeFactory.CreateNonCoupled<RemoveBargainIdentity>())
                {
                    if (bargainInfo.OrderNumbers.Count(number => !Equals(number, order.Number)) == 0)
                    {
                        _bargainRepository.Delete(bargainInfo.Bargain);
                    }

                    order.BargainId = null;
                    _orderRepository.Update(order);

                    operationScope
                        .Updated<Order>(order.Id)
                        .Complete();
                }

                transaction.Complete();
            }

            return Response.Empty;
        }
        }
    }
