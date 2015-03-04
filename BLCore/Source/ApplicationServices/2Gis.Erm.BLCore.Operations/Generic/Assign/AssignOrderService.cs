using System;
using System.Linq;
using System.Security;
using System.ServiceModel.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Old;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignOrderService : IAssignGenericEntityService<Order>
    {
        private readonly IPublicService _publicService;
        private readonly ISecureFinder _finder;
        private readonly IOrderRepository _orderRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;

        public AssignOrderService(
            IPublicService publicService,
            ISecureFinder finder,
            IOrderRepository orderRepository,
            IOperationScopeFactory scopeFactory,
            ICommonLog logger,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext)
        {
            _publicService = publicService;
            _finder = finder;
            _orderRepository = orderRepository;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
        }

        public virtual AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            string orderNumber = null;
            try
            {
                using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, Order>())
                {
                    _publicService.Handle(new ValidateOwnerIsNotReserveRequest<Order> { Id = entityId });

                    var order = _finder.Find(Specs.Find.ById<Order>(entityId))
                            .Select(x => new { HasDeal = x.DealId.HasValue, x.Number, OldOwnerCode = x.OwnerCode })
                        .Single();

                    orderNumber = order.Number;

                    {
                        var permissions = _entityAccessService.RestrictEntityAccess(EntityName.Order,
                                                            EntityAccessTypes.All,
                                                            _userContext.Identity.Code,
                                                            entityId,
                                                            ownerCode,
                                                            order.OldOwnerCode);

                        if (!permissions.HasFlag(EntityAccessTypes.Assign) || !permissions.HasFlag(EntityAccessTypes.Update))
                        {
                            throw new SecurityException(string.Format(BLResources.AssignOrderAccessDenied, orderNumber));
                        }
                    }

                    if (order.HasDeal)
                    {
                        throw new ArgumentException(string.Format(BLResources.OrderHasLinkedDeal, order.Number));
                    }

                    _orderRepository.Assign(entityId, ownerCode);

                    operationScope
                        .Updated<Order>(entityId)
                        .Complete();

                    _logger.InfoFormat("Куратором заказа с id={0} назначен пользователь {1}", entityId, ownerCode);
                    return null;
                }
            }
            catch (SecurityAccessDeniedException ex)
            {
                if (orderNumber != null)
                {
                    throw new SecurityException(string.Format(BLResources.AssignOrderAccessDenied, orderNumber), ex.InnerException);
                }

                throw;
            }
        }
    }
}