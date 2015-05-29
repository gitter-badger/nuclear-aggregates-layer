using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Activate
{
    public class ActivatePriceOperationService : IActivateGenericEntityService<Price>
    {
        private readonly IPriceReadModel _priceReadModel;
        private readonly IBulkActivatePricePositionsAggregateService _bulkActivatePricePositionsAggregateService;
        private readonly IBulkActivateDeniedPositionsAggregateService _bulkActivateDeniedPositionsAggregateService;
        private readonly IActivatePriceAggregateService _activatePriceAggregateService;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IVerifyDeniedPositionsForSymmetryOperationService _verifyDeniedPositionsForSymmetryOperationService;
        private readonly IVerifyDeniedPositionsForDuplicatesOperationService _verifyDeniedPositionsForDuplicatesOperationService;

        public ActivatePriceOperationService(IPriceReadModel priceReadModel,
                                             IBulkActivatePricePositionsAggregateService bulkActivatePricePositionsAggregateService,
                                             IBulkActivateDeniedPositionsAggregateService bulkActivateDeniedPositionsAggregateService,
                                             IActivatePriceAggregateService activatePriceAggregateService,
                                             IOperationScopeFactory operationScopeFactory,
                                             IVerifyDeniedPositionsForSymmetryOperationService verifyDeniedPositionsForSymmetryOperationService,
                                             IVerifyDeniedPositionsForDuplicatesOperationService verifyDeniedPositionsForDuplicatesOperationService)
        {
            _priceReadModel = priceReadModel;
            _bulkActivatePricePositionsAggregateService = bulkActivatePricePositionsAggregateService;
            _bulkActivateDeniedPositionsAggregateService = bulkActivateDeniedPositionsAggregateService;
            _activatePriceAggregateService = activatePriceAggregateService;
            _operationScopeFactory = operationScopeFactory;
            _verifyDeniedPositionsForSymmetryOperationService = verifyDeniedPositionsForSymmetryOperationService;
            _verifyDeniedPositionsForDuplicatesOperationService = verifyDeniedPositionsForDuplicatesOperationService;
        }

        public int Activate(long entityId)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ActivateIdentity, Price>())
            {
                var price = _priceReadModel.GetPrice(entityId);
                if (price == null)
                {
                    throw new EntityNotFoundException(typeof(Price), entityId);
                }

                var allPriceDescendantsDto = _priceReadModel.GetAllPriceDescendantsDto(entityId);

                var count = _bulkActivatePricePositionsAggregateService.Activate(allPriceDescendantsDto.PricePositions,
                                                                                 allPriceDescendantsDto.AssociatedPositionsGroupsMapping,
                                                                                 allPriceDescendantsDto.AssociatedPositionsMapping);

                var deniedPositions = allPriceDescendantsDto.DeniedPositions.Where(x => !x.IsActive && !x.IsDeleted).DistinctDeniedPositions();
                _verifyDeniedPositionsForDuplicatesOperationService.VerifyWithinCollection(deniedPositions);
                _verifyDeniedPositionsForSymmetryOperationService.VerifyWithinCollection(deniedPositions);

                count += _bulkActivateDeniedPositionsAggregateService.Activate(deniedPositions);

                count += _activatePriceAggregateService.Activate(price);

                operationScope.Complete();
                return count;
            }
        }
    }
}
