using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public sealed class DeleteDeniedPositionOperationService : IDeleteGenericEntityService<DeniedPosition>
    {        
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IPriceReadModel _priceReadModel;
        private readonly IGetSymmetricDeniedPositionOperationService _getSymmetricDeniedPositionOperationService;
        private readonly IDeleteDeniedPositionAggregateService _deleteDeniedPositionAggregateService;

        public DeleteDeniedPositionOperationService(IOperationScopeFactory operationScopeFactory,
                                                    IPriceReadModel priceReadModel,
                                                    IGetSymmetricDeniedPositionOperationService getSymmetricDeniedPositionOperationService,
                                                    IDeleteDeniedPositionAggregateService deleteDeniedPositionAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _priceReadModel = priceReadModel;
            _getSymmetricDeniedPositionOperationService = getSymmetricDeniedPositionOperationService;
            _deleteDeniedPositionAggregateService = deleteDeniedPositionAggregateService;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, DeniedPosition>())
            {
                var deniedPosition = _priceReadModel.GetDeniedPosition(entityId);
                if (deniedPosition.IsSelfDenied())
                {
                    _deleteDeniedPositionAggregateService.DeleteSelfDenied(deniedPosition);
                }
                else
                {
                    var symmetricDeniedPosition = _getSymmetricDeniedPositionOperationService.Get(deniedPosition.PositionId, deniedPosition.PositionDeniedId, deniedPosition.PriceId);
                    _deleteDeniedPositionAggregateService.Delete(deniedPosition, symmetricDeniedPosition);
                }

                scope.Complete();
            }

            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            return null;
        }
    }
}