using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations
{
    public sealed class ChangeOrderLegalPersonProfileAggregateService : IChangeOrderLegalPersonProfileAggregateService
    {
        private readonly ISecureRepository<Order> _orderRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public ChangeOrderLegalPersonProfileAggregateService(ISecureRepository<Order> orderRepository, IOperationScopeFactory scopeFactory)
        {
            _orderRepository = orderRepository;
            _scopeFactory = scopeFactory;
        }

        public void Change(Order order, LegalPersonProfile profile)
        {
            if (profile == null || profile.IsDeleted || !profile.IsActive)
            {
                throw new InvalidLegalPersonProfileForOrderException(BLResources.InvalidLegalPersonProfileForOrder);
            }

            if (order.LegalPersonId != profile.LegalPersonId)
            {
                throw new InvalidLegalPersonProfileForOrderException(BLResources.OrderLegalPersonProfileShouldBelongToOrderLegalPerson);
            }

            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Order>())
            {
                order.LegalPersonProfileId = profile.Id;
                _orderRepository.Update(order);
                _orderRepository.Save();

                scope.Updated(order)
                     .Complete();
            }
        }
    }
}