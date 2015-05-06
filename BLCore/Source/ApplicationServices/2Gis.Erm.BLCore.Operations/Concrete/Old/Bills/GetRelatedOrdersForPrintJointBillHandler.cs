using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Common.Caching;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Bills
{
    public sealed class GetRelatedOrdersForPrintJointBillHandler : RequestHandler<GetRelatedOrdersForPrintJointBillRequest, GetRelatedOrdersForPrintJointBillResponse>
    {
        private const string RelatedOrdersKey = "erm:bills-relatedorders-preparejointbill.modelorder:{0}.user:{1}";
        private static readonly TimeSpan CachedRelatedOrdersExpiration = TimeSpan.FromSeconds(60);

        private readonly IOrderReadModel _orderReadModel;
        private readonly IUserContext _userContext;
        private readonly ICacheAdapter _cacheAdapter;

        public GetRelatedOrdersForPrintJointBillHandler(IOrderReadModel orderReadModel, IUserContext userContext, ICacheAdapter cacheAdapter)
        {
            _orderReadModel = orderReadModel;
            _userContext = userContext;
            _cacheAdapter = cacheAdapter;
        }

        protected override GetRelatedOrdersForPrintJointBillResponse Handle(GetRelatedOrdersForPrintJointBillRequest request)
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
                relatedOrders = _orderReadModel.GetRelatedOrdersForPrintJointBill(request.OrderId) as RelatedOrderDescriptor[];
                if (relatedOrders != null)
                {
                    _cacheAdapter.Add(cacheKey, relatedOrders, CachedRelatedOrdersExpiration);
                }
            }

            return new GetRelatedOrdersForPrintJointBillResponse { Orders = relatedOrders };
        }
    }
}

