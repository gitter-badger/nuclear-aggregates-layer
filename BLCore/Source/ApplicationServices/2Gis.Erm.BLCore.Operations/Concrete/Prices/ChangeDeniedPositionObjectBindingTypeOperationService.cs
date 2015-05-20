using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Prices
{
    public class ChangeDeniedPositionObjectBindingTypeOperationService : IChangeDeniedPositionObjectBindingTypeOperationService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IPriceReadModel _priceReadModel;
        private readonly IVerifyDeniedPositionsForDuplicatesOperationService _verifyDeniedPositionsForDuplicatesOperationService;
        private readonly IGetSymmetricDeniedPositionOperationService _getSymmetricDeniedPositionOperationService;
        private readonly IChangeDeniedPositionObjectBindingTypeAggregateService _changeDeniedPositionObjectBindingTypeAggregateService;

        public ChangeDeniedPositionObjectBindingTypeOperationService(IOperationScopeFactory operationScopeFactory,
                                                                     IPriceReadModel priceReadModel,
                                                                     IVerifyDeniedPositionsForDuplicatesOperationService verifyDeniedPositionsForDuplicatesOperationService,
                                                                     IGetSymmetricDeniedPositionOperationService getSymmetricDeniedPositionOperationService,
                                                                     IChangeDeniedPositionObjectBindingTypeAggregateService changeDeniedPositionObjectBindingTypeAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _priceReadModel = priceReadModel;
            _verifyDeniedPositionsForDuplicatesOperationService = verifyDeniedPositionsForDuplicatesOperationService;
            _getSymmetricDeniedPositionOperationService = getSymmetricDeniedPositionOperationService;
            _changeDeniedPositionObjectBindingTypeAggregateService = changeDeniedPositionObjectBindingTypeAggregateService;
        }

        public void Change(long deniedPositionId, ObjectBindingType objectBindingType)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<ChangeDeniedPositionObjectBindingTypeIdentity>())
            {
                var originalDeniedPosition = _priceReadModel.GetDeniedPosition(deniedPositionId);

                if (originalDeniedPosition.IsSelfDenied())
                {
                    _verifyDeniedPositionsForDuplicatesOperationService.Verify(originalDeniedPosition.PositionId,
                                                                               originalDeniedPosition.PositionDeniedId,
                                                                               originalDeniedPosition.PriceId,
                                                                               originalDeniedPosition.Id);

                    _changeDeniedPositionObjectBindingTypeAggregateService.ChangeSelfDenied(originalDeniedPosition, objectBindingType);
                }
                else
                {
                    var symmetricOriginalDeniedPosition =
                        _getSymmetricDeniedPositionOperationService.GetSingle(originalDeniedPosition.PositionId,
                                                                              originalDeniedPosition.PositionDeniedId,
                                                                              originalDeniedPosition.PriceId);

                    _verifyDeniedPositionsForDuplicatesOperationService.Verify(originalDeniedPosition.PositionId,
                                                                               originalDeniedPosition.PositionDeniedId,
                                                                               originalDeniedPosition.PriceId,
                                                                               originalDeniedPosition.Id,
                                                                               symmetricOriginalDeniedPosition.Id);

                    _changeDeniedPositionObjectBindingTypeAggregateService.Change(originalDeniedPosition, symmetricOriginalDeniedPosition, objectBindingType);
                }

                scope.Complete();
            }
        }
    }
}
