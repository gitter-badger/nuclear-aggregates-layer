using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders
{
    public class ChangeOrderLegalPersonProfileOperationService : IChangeOrderLegalPersonProfileOperationService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderReadModel _orderReadModel;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IOperationScopeFactory _scopeFactory;

        public ChangeOrderLegalPersonProfileOperationService(
            IOrderRepository orderRepository,
            IOperationScopeFactory scopeFactory,
            IOrderReadModel orderReadModel, 
            ILegalPersonReadModel legalPersonReadModel)
        {
            _orderRepository = orderRepository;
            _scopeFactory = scopeFactory;
            _orderReadModel = orderReadModel;
            _legalPersonReadModel = legalPersonReadModel;
        }

        public void ChangeLegalPersonProfile(long orderId, long profileId)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<ChangeOrderLegalPersonProfileIdentity>())
            {
                var order = _orderReadModel.GetOrderSecure(orderId);
                var profile = _legalPersonReadModel.GetLegalPersonProfile(profileId);
                if (profile == null || profile.IsDeleted || !profile.IsActive)
                {
                    throw new InvalidLegalPersonProfileForOrderException();
                }

                order.LegalPersonProfileId = profileId;

                _orderRepository.Update(order);

                scope.Updated<Order>(orderId)
                     .Complete();
            }
        }
    }
}
