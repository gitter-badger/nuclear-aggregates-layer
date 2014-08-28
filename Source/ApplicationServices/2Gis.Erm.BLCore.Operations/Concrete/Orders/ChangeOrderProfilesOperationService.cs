using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders
{
    public class ChangeOrderProfilesOperationService : IChangeOrderProfilesOperationService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOperationScopeFactory _scopeFactory;

        public ChangeOrderProfilesOperationService(
            IOrderRepository orderRepository,
            IOperationScopeFactory scopeFactory,
            IOrderReadModel orderReadModel)
        {
            _orderRepository = orderRepository;
            _scopeFactory = scopeFactory;
            _orderReadModel = orderReadModel;
        }

        public void ChangeProfiles(long orderId, long profileId)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<ChangeOrderProfilesIdentity>())
            {
                var order = _orderReadModel.GetOrder(orderId);
                order.LegalPersonProfileId = profileId;

                _orderRepository.Update(order);

                scope.Updated<Order>(orderId)
                     .Complete();
            }
        }
    }
}
