using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Exceptions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Price;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Activate
{
    public class ActivateDeniedPositionService : IActivateGenericEntityService<DeniedPosition>
    {
        private readonly IActivateDeniedPositionAggregateService _activateDeniedPositionAggregateService;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IPriceReadModel _priceReadModel;
        private readonly IDeniedPositionsDuplicatesVerifier _deniedPositionsDuplicatesVerifier;
        private readonly IGetSymmetricDeniedPositionOperationService _getSymmetricDeniedPositionOperationService;

        public ActivateDeniedPositionService(IActivateDeniedPositionAggregateService activateDeniedPositionAggregateService,
                                             IOperationScopeFactory operationScopeFactory,
                                             IPriceReadModel priceReadModel,
                                             IDeniedPositionsDuplicatesVerifier deniedPositionsDuplicatesVerifier,
                                             IGetSymmetricDeniedPositionOperationService getSymmetricDeniedPositionOperationService)
        {
            _activateDeniedPositionAggregateService = activateDeniedPositionAggregateService;
            _operationScopeFactory = operationScopeFactory;
            _priceReadModel = priceReadModel;
            _deniedPositionsDuplicatesVerifier = deniedPositionsDuplicatesVerifier;
            _getSymmetricDeniedPositionOperationService = getSymmetricDeniedPositionOperationService;
        }

        public int Activate(long entityId)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<ActivateIdentity, DeniedPosition>())
            {
                var deniedPosition = _priceReadModel.GetDeniedPosition(entityId);
                if (deniedPosition.IsActive)
                {
                    throw new ActiveEntityActivationException(typeof(DeniedPosition), deniedPosition.Id);
                }

                var isPricePublishedAndActiveInfo = _priceReadModel.IsPricePublishedAndActive(deniedPosition.PriceId);
                if (isPricePublishedAndActiveInfo.IsPublished)
                {
                    throw new PublishedPriceModificationException(BLResources.CantActivateDeniedPositionWhenPriceIsPublished);
                }

                if (!isPricePublishedAndActiveInfo.IsActive)
                {
                    throw new InactiveEntityModificationException(BLResources.CantActivateDeniedPositionWhenPriceIsDeactivated);
                }

                _deniedPositionsDuplicatesVerifier.VerifyForDuplicates(deniedPosition);

                if (deniedPosition.IsSelfDenied())
                {
                    _activateDeniedPositionAggregateService.ActivateSelfDeniedPosition(deniedPosition);
                }
                else
                {
                    var symmetricDeniedPosition = _getSymmetricDeniedPositionOperationService.GetInactiveWithObjectBindingTypeConsideration(entityId);
                    _activateDeniedPositionAggregateService.Activate(deniedPosition, symmetricDeniedPosition);
                }

                scope.Complete();
            }

            return 0;
        }
    }
}