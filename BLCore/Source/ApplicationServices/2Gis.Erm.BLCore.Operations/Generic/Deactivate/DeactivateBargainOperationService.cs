using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Bargains;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public class DeactivateBargainOperationService : IDeactivateGenericEntityService<Bargain>
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IDeactivateAggregateRepository<Bargain> _deactivateAggregateService;
        private readonly IOrderReadModel _orderReadModel;

        public DeactivateBargainOperationService(IOperationScopeFactory scopeFactory,
                                                 IDeactivateAggregateRepository<Bargain> deactivateAggregateService,
                                                 IOrderReadModel orderReadModel)
        {
            _scopeFactory = scopeFactory;
            _deactivateAggregateService = deactivateAggregateService;
            _orderReadModel = orderReadModel;
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            var ordersForBargain = _orderReadModel.GetBargainUsage(entityId);

            if (ordersForBargain.Any())
            {
                throw new BargainInUseDeactivationException(string.Format(BLResources.CannotDeactivateBargainBecauseItIsLinkedWithOrders,
                                                                          string.Join(", ", ordersForBargain.Keys)));
            }

            using (var scope = _scopeFactory.CreateSpecificFor<DeactivateIdentity, Bargain>())
            {
                _deactivateAggregateService.Deactivate(entityId);

                scope.Updated<Bargain>(entityId)
                     .Complete();
            }

            return null;
        }
    }
}