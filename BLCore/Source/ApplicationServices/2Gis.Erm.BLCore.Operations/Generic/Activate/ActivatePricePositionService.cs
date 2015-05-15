using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Activate
{
    public class ActivatePricePositionService : IActivateGenericEntityService<PricePosition>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IPriceReadModel _priceReadModel;
        private readonly IBulkActivatePricePositionsAggregateService _bulkActivatePricePositionsAggregateService;
        private readonly IBulkActivateDeniedPositionsAggregateService _bulkActivateDeniedPositionsAggregateService;

        public ActivatePricePositionService(IOperationScopeFactory operationScopeFactory,
                                            IPriceReadModel priceReadModel,
                                            IBulkActivatePricePositionsAggregateService bulkActivatePricePositionsAggregateService,
                                            IBulkActivateDeniedPositionsAggregateService bulkActivateDeniedPositionsAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _priceReadModel = priceReadModel;
            _bulkActivatePricePositionsAggregateService = bulkActivatePricePositionsAggregateService;
            _bulkActivateDeniedPositionsAggregateService = bulkActivateDeniedPositionsAggregateService;
        }

        public int Activate(long entityId)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, PricePosition>())
            {
                var pricePosition = _priceReadModel.GetPricePosition(entityId);
                if (pricePosition == null)
                {
                    throw new NotificationException(BLResources.UnableToGetExisitingPricePosition);
                }

                var isPricePublished = _priceReadModel.IsPricePublished(pricePosition.PriceId);
                if (isPricePublished)
                {
                    throw new ArgumentException(BLResources.CantActivatePricePositionWhenPriceIsPublished);
                }

                var isPriceContainsPosition = _priceReadModel.IsPriceContainsPosition(pricePosition.PriceId, pricePosition.PositionId);
                if (isPriceContainsPosition)
                {
                    throw new ArgumentException(BLResources.AlreadyExistsPricePositionWithSamePositionNotification);
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

                count += _bulkActivateDeniedPositionsAggregateService.Activate(allPricePositionDescendantsDto.DeniedPositions);

                operationScope.Complete();
                return count;
            }
        }
    }
}
