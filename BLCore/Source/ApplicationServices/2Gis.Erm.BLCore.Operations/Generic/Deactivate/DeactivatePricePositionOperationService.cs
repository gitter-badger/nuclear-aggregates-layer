﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public class DeactivatePricePositionOperationService : IDeactivateGenericEntityService<PricePosition>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IPriceReadModel _priceReadModel;
        private readonly IBulkDeactivatePricePositionsAggregateService _bulkDeactivatePricePositionsAggregateService;
        private readonly IBulkDeactivateDeniedPositionsAggregateService _bulkDeactivateDeniedPositionsAggregateService;

        public DeactivatePricePositionOperationService(IOperationScopeFactory operationScopeFactory,
                                                       IPriceReadModel priceReadModel,
                                                       IBulkDeactivatePricePositionsAggregateService bulkDeactivatePricePositionsAggregateService,
                                                       IBulkDeactivateDeniedPositionsAggregateService bulkDeactivateDeniedPositionsAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _priceReadModel = priceReadModel;
            _bulkDeactivatePricePositionsAggregateService = bulkDeactivatePricePositionsAggregateService;
            _bulkDeactivateDeniedPositionsAggregateService = bulkDeactivateDeniedPositionsAggregateService;
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, PricePosition>())
            {
                var pricePosition = _priceReadModel.GetPricePosition(entityId);
                if (pricePosition == null)
                {
                    throw new EntityNotFoundException(typeof(PricePosition), entityId);
                }

                if (!pricePosition.IsActive)
                {
                    throw new ArgumentException(BLResources.PricePositionIsInactiveAlready);
                }

                if (!_priceReadModel.IsPriceActive(pricePosition.PriceId))
                {
                    throw new ArgumentException(BLResources.CantDeativatePricePositionWhenPriceIsDeactivated);
                }

                if (_priceReadModel.IsPricePublished(pricePosition.PriceId))
                {
                    throw new ArgumentException(BLResources.CantDeativatePricePositionWhenPriceIsPublished);
                }

                var allPricePositionDescendantsDto = _priceReadModel.GetAllPricePositionDescendantsDto(pricePosition.Id, pricePosition.PositionId);

                var pricePositions = new[] { pricePosition };
                var associatedPositionsGroupsMapping = new Dictionary<long, IEnumerable<AssociatedPositionsGroup>>
                                                           {
                                                               {
                                                                   pricePosition.Id, allPricePositionDescendantsDto.AssociatedPositionsGroups
                                                               }
                                                           };

                _bulkDeactivatePricePositionsAggregateService.Deactivate(pricePositions,
                                                                         associatedPositionsGroupsMapping,
                                                                         allPricePositionDescendantsDto.AssociatedPositionsMapping);

                _bulkDeactivateDeniedPositionsAggregateService.Deactivate(allPricePositionDescendantsDto.DeniedPositions.Where(x => x.IsActive && !x.IsDeleted));

                operationScope.Complete();
            }

            return null;
        }
    }
}