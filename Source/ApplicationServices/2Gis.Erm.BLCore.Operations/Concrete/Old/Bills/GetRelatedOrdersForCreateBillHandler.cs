using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Caching;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Bills
{
    public sealed class GetRelatedOrdersForCreateBillHandler : RequestHandler<GetRelatedOrdersForCreateBillRequest, GetRelatedOrdersForCreateBillResponse>
    {
        private const string RelatedOrdersKey = "erm:bills-relatedorders-createbill.modelorder:{0}.user:{1}";
        private static readonly TimeSpan CachedRelatedOrdersExpiration = TimeSpan.FromSeconds(120);
        private readonly IOrderRepository _orderRepository;
        private readonly IUserContext _userContext;
        private readonly ICacheAdapter _cacheAdapter;

        public GetRelatedOrdersForCreateBillHandler(IUserContext userContext, ICacheAdapter cacheAdapter, IOrderRepository orderRepository)
        {
            _userContext = userContext;
            _cacheAdapter = cacheAdapter;
            _orderRepository = orderRepository;
        }

        protected override GetRelatedOrdersForCreateBillResponse Handle(GetRelatedOrdersForCreateBillRequest request)
        {
            RelatedOrderDescriptor[] relatedOrders;
            var userCode = _userContext.Identity.Code;
            var cacheKey = string.Format(RelatedOrdersKey, request.OrderId, userCode);
            if (_cacheAdapter.Contains(cacheKey))
            {
                relatedOrders = _cacheAdapter.Get<RelatedOrderDescriptor[]>(cacheKey);
            }
            else
            {
                relatedOrders = _orderRepository.GetRelatedOrdersToCreateBill(request.OrderId).ToArray();

                _cacheAdapter.Add(cacheKey, relatedOrders, CachedRelatedOrdersExpiration);
            }

            return new GetRelatedOrdersForCreateBillResponse { Orders = relatedOrders };
        }
    }
}
