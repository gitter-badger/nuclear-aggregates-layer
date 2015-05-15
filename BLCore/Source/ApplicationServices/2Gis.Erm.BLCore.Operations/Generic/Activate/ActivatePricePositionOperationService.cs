using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Activate
{
    public class ActivatePricePositionOperationService : IActivateGenericEntityService<PricePosition>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IPriceReadModel _priceReadModel;
        private readonly IBulkActivatePricePositionsAggregateService _bulkActivatePricePositionsAggregateService;
        private readonly IBulkActivateDeniedPositionsAggregateService _bulkActivateDeniedPositionsAggregateService;
        private readonly IVerifyDeniedPositionsForSymmetryOperationService _verifyDeniedPositionsForSymmetryOperationService;
        private readonly IVerifyDeniedPositionsForDuplicatesOperationService _verifyDeniedPositionsForDuplicatesOperationService;

        public ActivatePricePositionOperationService(IOperationScopeFactory operationScopeFactory,
                                                     IPriceReadModel priceReadModel,
                                                     IBulkActivatePricePositionsAggregateService bulkActivatePricePositionsAggregateService,
                                                     IBulkActivateDeniedPositionsAggregateService bulkActivateDeniedPositionsAggregateService,
                                                     IVerifyDeniedPositionsForSymmetryOperationService verifyDeniedPositionsForSymmetryOperationService,
                                                     IVerifyDeniedPositionsForDuplicatesOperationService verifyDeniedPositionsForDuplicatesOperationService)
        {
            _operationScopeFactory = operationScopeFactory;
            _priceReadModel = priceReadModel;
            _bulkActivatePricePositionsAggregateService = bulkActivatePricePositionsAggregateService;
            _bulkActivateDeniedPositionsAggregateService = bulkActivateDeniedPositionsAggregateService;
            _verifyDeniedPositionsForSymmetryOperationService = verifyDeniedPositionsForSymmetryOperationService;
            _verifyDeniedPositionsForDuplicatesOperationService = verifyDeniedPositionsForDuplicatesOperationService;
        }

        public int Activate(long entityId)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ActivateIdentity, PricePosition>())
            {
                var pricePosition = _priceReadModel.GetPricePosition(entityId);
                if (pricePosition == null)
                {
                    throw new EntityNotFoundException(typeof(PricePosition), entityId);
                }

                if (_priceReadModel.IsPricePublished(pricePosition.PriceId))
                {
                    throw new ArgumentException(BLResources.CantActivatePricePositionWhenPriceIsPublished);
                }

                if (_priceReadModel.DoesPriceContainPosition(pricePosition.PriceId, pricePosition.PositionId))
                {
                    throw new EntityIsNotUniqueException(typeof(PricePosition), BLResources.AlreadyExistsPricePositionWithSamePositionNotification);
                }

                var allPricePositionDescendantsDto = _priceReadModel.GetAllPricePositionDescendantsDto(pricePosition.Id, pricePosition.PositionId);

                var pricePositions = new[] { pricePosition };
                var associatedPositionsGroupsMapping = new Dictionary<long, IEnumerable<AssociatedPositionsGroup>>
                                                           {
                                                               { pricePosition.Id, allPricePositionDescendantsDto.AssociatedPositionsGroups }
                                                           };

                var count = _bulkActivatePricePositionsAggregateService.Activate(pricePositions,
                                                                                 associatedPositionsGroupsMapping,
                                                                                 allPricePositionDescendantsDto.AssociatedPositionsMapping);

                var deniedPositions = allPricePositionDescendantsDto.DeniedPositions.Where(x => !x.IsActive && !x.IsDeleted).DistinctDeniedPositions();
                _verifyDeniedPositionsForDuplicatesOperationService.VerifyWithinCollection(deniedPositions);
                _verifyDeniedPositionsForSymmetryOperationService.VerifyWithinCollection(deniedPositions);

                var storedDeniedPositions = _priceReadModel.GetDeniedPositionsOrSymmetric(pricePosition.PositionId, pricePosition.PriceId);
                _verifyDeniedPositionsForDuplicatesOperationService.VerifyWithinCollection(deniedPositions.Concat(storedDeniedPositions).DistinctDeniedPositions());

                var deniedPositionsToActivate = deniedPositions.ExceptDeniedPositions(storedDeniedPositions);

                count += _bulkActivateDeniedPositionsAggregateService.Activate(deniedPositionsToActivate);

                operationScope.Complete();
                return count;
            }
        }
    }
}
