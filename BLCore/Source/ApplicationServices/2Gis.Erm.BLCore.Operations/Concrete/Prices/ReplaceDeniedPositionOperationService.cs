using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Prices
{
    public class ReplaceDeniedPositionOperationService : IReplaceDeniedPositionOperationService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IPriceReadModel _priceReadModel;
        private readonly IReplaceDeniedPositionAggregateService _replaceDeniedPositionAggregateService;
        private readonly IVerifyDeniedPositionsForDuplicatesOperationService _verifyDeniedPositionsForDuplicatesOperationService;
        private readonly IGetSymmetricDeniedPositionOperationService _getSymmetricDeniedPositionOperationService;

        public ReplaceDeniedPositionOperationService(IOperationScopeFactory operationScopeFactory,
                                                     IPriceReadModel priceReadModel,
                                                     IReplaceDeniedPositionAggregateService replaceDeniedPositionAggregateService,
                                                     IVerifyDeniedPositionsForDuplicatesOperationService verifyDeniedPositionsForDuplicatesOperationService,
                                                     IGetSymmetricDeniedPositionOperationService getSymmetricDeniedPositionOperationService)
        {
            _operationScopeFactory = operationScopeFactory;
            _priceReadModel = priceReadModel;
            _replaceDeniedPositionAggregateService = replaceDeniedPositionAggregateService;
            _verifyDeniedPositionsForDuplicatesOperationService = verifyDeniedPositionsForDuplicatesOperationService;
            _getSymmetricDeniedPositionOperationService = getSymmetricDeniedPositionOperationService;
        }

        public void Replace(long deniedPositionId, long newPositionDeniedId)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<ReplaceDeniedPositionIdentity>())
            {
                var originalDeniedPosition = _priceReadModel.GetDeniedPosition(deniedPositionId);
                _verifyDeniedPositionsForDuplicatesOperationService.Verify(originalDeniedPosition.PositionId, newPositionDeniedId, originalDeniedPosition.PriceId);

                if (originalDeniedPosition.IsSelfDenied())
                {
                    _replaceDeniedPositionAggregateService.ReplaceSelfDenied(originalDeniedPosition, newPositionDeniedId);
                }
                else
                {
                    var symmetricOriginalDeniedPosition =
                        _getSymmetricDeniedPositionOperationService.GetTheOnlyOneWithObjectBindingTypeConsiderationOrDie(originalDeniedPosition.PositionId,
                                                                                                                         originalDeniedPosition.PositionDeniedId,
                                                                                                                         originalDeniedPosition.PriceId,
                                                                                                                         originalDeniedPosition.ObjectBindingType);

                    _replaceDeniedPositionAggregateService.Replace(originalDeniedPosition, symmetricOriginalDeniedPosition, newPositionDeniedId);
                }

                scope.Complete();
            }
        }
    }
}
