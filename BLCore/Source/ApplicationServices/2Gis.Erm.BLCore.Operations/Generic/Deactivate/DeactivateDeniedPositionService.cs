using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Exceptions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Price;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public class DeactivateDeniedPositionService : IDeactivateGenericEntityService<DeniedPosition>
    {
        private readonly IDeactivateDeniedPositionAggregateService _deactivateDeniedPositionAggregateService;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IPriceReadModel _priceReadModel;
        private readonly IGetSymmetricDeniedPositionOperationService _getSymmetricDeniedPositionOperationService;

        public DeactivateDeniedPositionService(IDeactivateDeniedPositionAggregateService deactivateDeniedPositionAggregateService,
                                               IOperationScopeFactory operationScopeFactory,
                                               IPriceReadModel priceReadModel,
                                               IGetSymmetricDeniedPositionOperationService getSymmetricDeniedPositionOperationService)
        {            
            _deactivateDeniedPositionAggregateService = deactivateDeniedPositionAggregateService;
            _operationScopeFactory = operationScopeFactory;
            _priceReadModel = priceReadModel;
            _getSymmetricDeniedPositionOperationService = getSymmetricDeniedPositionOperationService;
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, DeniedPosition>())
            {
                var deniedPosition = _priceReadModel.GetDeniedPosition(entityId);
                if (!deniedPosition.IsActive)
                {
                    throw new InactiveEntityDeactivationException(typeof(DeniedPosition), deniedPosition.Id);
                }

                var isPricePublishedAndActiveInfo = _priceReadModel.IsPricePublishedAndActive(deniedPosition.PriceId);
                if (isPricePublishedAndActiveInfo.IsPublished)
                {
                    throw new PublishedPriceModificationException(BLResources.CantDeactivateDeniedPositionWhenPriceIsPublished);
                }

                if (!isPricePublishedAndActiveInfo.IsActive)
                {
                    throw new InactiveEntityModificationException(BLResources.CantDeactivateDeniedPositionWhenPriceIsDeactivated);
                }

                if (deniedPosition.IsSelfDenied())
                {
                    _deactivateDeniedPositionAggregateService.DeactivateSelfDeniedPosition(deniedPosition);
                }
                else
                {
                    var symmetricDeniedPosition = _getSymmetricDeniedPositionOperationService.GetWithObjectBindingTypeConsideration(entityId);
                    _deactivateDeniedPositionAggregateService.Deactivate(deniedPosition, symmetricDeniedPosition);
                }

                scope.Complete();
            }

            return null;
        }
    }
}