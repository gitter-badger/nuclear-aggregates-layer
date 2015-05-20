using System;

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
    public class DeactivatePriceOperationService : IDeactivateGenericEntityService<Price>
    {
        private readonly IPriceReadModel _priceReadModel;
        private readonly IBulkDeactivatePricePositionsAggregateService _bulkDeactivatePricePositionsAggregateService;
        private readonly IBulkDeactivateDeniedPositionsAggregateService _bulkDeactivateDeniedPositionsAggregateService;
        private readonly IDeactivatePriceAggregateService _deactivatePriceAggregateService;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DeactivatePriceOperationService(IPriceReadModel priceReadModel,
                                               IBulkDeactivatePricePositionsAggregateService bulkDeactivatePricePositionsAggregateService,
                                               IBulkDeactivateDeniedPositionsAggregateService bulkDeactivateDeniedPositionsAggregateService,
                                               IDeactivatePriceAggregateService deactivatePriceAggregateService,
                                               IOperationScopeFactory operationScopeFactory)
        {
            _priceReadModel = priceReadModel;
            _bulkDeactivatePricePositionsAggregateService = bulkDeactivatePricePositionsAggregateService;
            _bulkDeactivateDeniedPositionsAggregateService = bulkDeactivateDeniedPositionsAggregateService;
            _deactivatePriceAggregateService = deactivatePriceAggregateService;
            _operationScopeFactory = operationScopeFactory;
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, Price>())
            {
                var pricePublishedForToday = _priceReadModel.IsPricePublished(entityId);
                if (pricePublishedForToday)
                {
                    throw new NotificationException(BLResources.PriceInActionCannotBeDeactivated);
                }

                var price = _priceReadModel.GetPrice(entityId);
                if (price == null)
                {
                    throw new EntityNotFoundException(typeof(Price), entityId);
                }

                if (!price.IsActive)
                {
                    throw new ArgumentException(BLResources.PriceIsInactiveAlready);
                }

                var priceWithAllDescendantsDto = _priceReadModel.GetAllPriceDescendantsDto(entityId);

                _bulkDeactivatePricePositionsAggregateService.Deactivate(priceWithAllDescendantsDto.PricePositions,
                                                                         priceWithAllDescendantsDto.AssociatedPositionsGroupsMapping,
                                                                         priceWithAllDescendantsDto.AssociatedPositionsMapping);

                _bulkDeactivateDeniedPositionsAggregateService.Deactivate(priceWithAllDescendantsDto.DeniedPositions);

                _deactivatePriceAggregateService.Deactivate(price);

                operationScope.Complete();
            }

            return null;
        }
    }
}