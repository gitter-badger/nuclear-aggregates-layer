using System;

using DoubleGis.Erm.BLCore.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders
{
    public sealed class ChangeOrderDealHandler : RequestHandler<ChangeOrderDealRequest, EmptyResponse>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _securityServiceFunctionalAccess;
        private readonly IOrderRepository _orderRepository;
        private readonly IDealReadModel _dealReadModel;
        private readonly IOperationScopeFactory _scopeFactory;

        public ChangeOrderDealHandler(
            ISubRequestProcessor subRequestProcessor,
            ISecurityServiceFunctionalAccess securityServiceFunctionalAccess, 
            IUserContext userContext, 
            IOrderRepository orderRepository, 
            IDealReadModel dealReadModel, 
            IOperationScopeFactory scopeFactory)
        {
            _subRequestProcessor = subRequestProcessor;
            _securityServiceFunctionalAccess = securityServiceFunctionalAccess;
            _userContext = userContext;
            _orderRepository = orderRepository;
            _dealReadModel = dealReadModel;
            _scopeFactory = scopeFactory;
        }

        protected override EmptyResponse Handle(ChangeOrderDealRequest request)
        {
            var hasExtendedCreationPrivilege = _securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.OrderChangeDealExtended, _userContext.Identity.Code);
            if (!hasExtendedCreationPrivilege)
            {
                throw new NotificationException(BLResources.AccessDeniedChangeOrderDeal);
            }

            if (!request.DealId.HasValue)
            {
                throw new ArgumentNullException(@"request.DealId");
            }

            using (var operationScope = _scopeFactory.CreateNonCoupled<ChangeDealIdentity>())
            {
                var newDealInfo = _dealReadModel.GetDeal(request.DealId.Value);
                if (newDealInfo == null)
                {
                    return Response.Empty;
                }

                var order = _orderRepository.GetOrder(request.OrderId);
                var oldDealId = order.DealId;
                if (oldDealId == null || (oldDealId != newDealInfo.Id))
                {
                    order.DealId = newDealInfo.Id;
                    order.OwnerCode = newDealInfo.OwnerCode;

                    _orderRepository.Update(order);

                    var dealIds = oldDealId.HasValue ? new[] { newDealInfo.Id, oldDealId.Value } : new[] { newDealInfo.Id };
                    _subRequestProcessor.HandleSubRequest(new ActualizeDealProfitIndicatorsRequest { DealIds = dealIds }, Context);
                }

                operationScope
                    .Updated<Order>(order.Id)
                    .Complete();
            }

            return Response.Empty;
        }
    }
}
