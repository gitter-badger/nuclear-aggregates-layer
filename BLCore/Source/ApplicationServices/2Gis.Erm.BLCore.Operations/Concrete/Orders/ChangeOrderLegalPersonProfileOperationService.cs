using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders
{
    public class ChangeOrderLegalPersonProfileOperationService : IChangeOrderLegalPersonProfileOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IChangeOrderLegalPersonProfileAggregateService _profileAggregateService;

        public ChangeOrderLegalPersonProfileOperationService(
            IOperationScopeFactory scopeFactory,
            IOrderReadModel orderReadModel, 
            ILegalPersonReadModel legalPersonReadModel,
            IChangeOrderLegalPersonProfileAggregateService profileAggregateService)
        {
            _scopeFactory = scopeFactory;
            _orderReadModel = orderReadModel;
            _legalPersonReadModel = legalPersonReadModel;
            _profileAggregateService = profileAggregateService;
        }

        public void ChangeLegalPersonProfile(long orderId, long profileId)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<ChangeOrderLegalPersonProfileIdentity>())
            {
                var order = _orderReadModel.GetOrderSecure(orderId);
                if (order == null)
                {
                    throw new EntityNotFoundException(typeof(Order), orderId);
                }

                var profile = _legalPersonReadModel.GetLegalPersonProfile(profileId);

                _profileAggregateService.Change(order, profile);

                scope.Updated<Order>(orderId)
                     .Complete();
            }
        }
    }
}
