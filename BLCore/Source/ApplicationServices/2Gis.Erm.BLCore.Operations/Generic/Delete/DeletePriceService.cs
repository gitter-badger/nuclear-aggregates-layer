using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeletePriceService : IDeleteGenericEntityService<Price>
    {
        private readonly IPriceReadModel _priceReadModel;
        private readonly IBulkDeletePricePositionsAggregateService _bulkDeletePricePositionsAggregateService;
        private readonly IBulkDeleteDeniedPositionsAggregateService _bulkDeleteDeniedPositionsAggregateService;
        private readonly IDeletePriceAggregateService _deletePriceAggregateService;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DeletePriceService(IPriceReadModel priceReadModel,
                                  IBulkDeletePricePositionsAggregateService bulkDeletePricePositionsAggregateService,
                                  IBulkDeleteDeniedPositionsAggregateService bulkDeleteDeniedPositionsAggregateService,
                                  IDeletePriceAggregateService deletePriceAggregateService,
                                  IOperationScopeFactory operationScopeFactory)
        {
            _priceReadModel = priceReadModel;
            _bulkDeletePricePositionsAggregateService = bulkDeletePricePositionsAggregateService;
            _bulkDeleteDeniedPositionsAggregateService = bulkDeleteDeniedPositionsAggregateService;
            _deletePriceAggregateService = deletePriceAggregateService;
            _operationScopeFactory = operationScopeFactory;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, Price>())
            {
                var price = _priceReadModel.GetPrice(entityId);
                if (price == null)
                {
                    throw new EntityNotFoundException(typeof(Price), entityId);
                }

                var isPricePublished = _priceReadModel.IsPricePublished(entityId);
                if (isPricePublished)
                    {
                        throw new ArgumentException(BLResources.PriceInActionCannotBeDeactivated);
                    }

                var isPriceLinked = _priceReadModel.IsPriceLinked(entityId);
                if (isPriceLinked)
                    {
                        throw new ArgumentException(BLResources.PriceIsLinkedWithActiveOrdersAndCannotBeDeleted);
                    }

                var allPriceDescendantsDto = _priceReadModel.GetAllPriceDescendantsDto(entityId);

                _bulkDeletePricePositionsAggregateService.Delete(allPriceDescendantsDto.PricePositions,
                                                                 allPriceDescendantsDto.AssociatedPositionsGroupsMapping,
                                                                 allPriceDescendantsDto.AssociatedPositionsMapping);

                _bulkDeleteDeniedPositionsAggregateService.Delete(allPriceDescendantsDto.DeniedPositions);

                _deletePriceAggregateService.Delete(price);

                operationScope.Complete();

                return null;
            }
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var isPricePublished = _priceReadModel.IsPricePublished(entityId);
            if (isPricePublished)
            {
                return new DeleteConfirmationInfo
                {
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.PriceInActionCannotBeDeactivated
                };
            }

            var priceExists = _priceReadModel.DoesPriceExist(entityId);
            if (!priceExists)
            {
                return new DeleteConfirmationInfo
                {
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.EntityNotFound
                };
            }

            return new DeleteConfirmationInfo
            {
                EntityCode = string.Empty,
                IsDeleteAllowed = true,
                DeleteConfirmation = string.Empty
            };
        }
    }
}