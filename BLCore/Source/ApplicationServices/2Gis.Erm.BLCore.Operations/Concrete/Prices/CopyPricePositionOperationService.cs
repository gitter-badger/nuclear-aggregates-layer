using System.Collections.Generic;
using System.IO;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Prices
{
    public class CopyPricePositionOperationService : ICopyPricePositionOperationService
    {
        private readonly ICommonLog _logger;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IPriceReadModel _priceReadModel;
        private readonly ICreatePricePositionAggregateService _createPricePositionAggregateService;
        private readonly IBulkCreateDeniedPositionsAggregateService _bulkCreateDeniedPositionsAggregateService;
        private readonly IBulkCreateAssociatedPositionsGroupsAggregateService _bulkCreateAssociatedPositionsGroupsAggregateService;
        private readonly IBulkCreateAssociatedPositionsAggregateService _bulkCreateAssociatedPositionsAggregateService;

        public CopyPricePositionOperationService(ICommonLog logger,
                                                 IOperationScopeFactory operationScopeFactory,
                                                 IPriceReadModel priceReadModel,
                                                 ICreatePricePositionAggregateService createPricePositionAggregateService,
                                                 IBulkCreateDeniedPositionsAggregateService bulkCreateDeniedPositionsAggregateService,
                                                 IBulkCreateAssociatedPositionsGroupsAggregateService bulkCreateAssociatedPositionsGroupsAggregateService,
                                                 IBulkCreateAssociatedPositionsAggregateService bulkCreateAssociatedPositionsAggregateService)
        {
            _logger = logger;
            _operationScopeFactory = operationScopeFactory;
            _priceReadModel = priceReadModel;
            _createPricePositionAggregateService = createPricePositionAggregateService;
            _bulkCreateDeniedPositionsAggregateService = bulkCreateDeniedPositionsAggregateService;
            _bulkCreateAssociatedPositionsGroupsAggregateService = bulkCreateAssociatedPositionsGroupsAggregateService;
            _bulkCreateAssociatedPositionsAggregateService = bulkCreateAssociatedPositionsAggregateService;
        }

        public int Copy(long priceId, long sourcePricePositionId, long positionId)
        {
            PerformValidation(priceId, positionId);

            using (var operationScope = _operationScopeFactory.CreateNonCoupled<CopyPricePositionIdentity>())
            {
                var pricePosition = _priceReadModel.GetPricePosition(sourcePricePositionId);
                if (pricePosition == null)
                {
                    _logger.Fatal(BLResources.UnableToGetExisitingPricePosition);
                    throw new NotificationException(BLResources.UnableToGetExisitingPricePosition);
                }

                var sourcePositionId = pricePosition.PositionId;
                var allPricePositionDescendantsDto = _priceReadModel.GetAllPricePositionDescendantsDto(sourcePricePositionId, sourcePositionId);

                var count = _createPricePositionAggregateService.Create(pricePosition, priceId, positionId);

                CreateDeniedPositions(allPricePositionDescendantsDto.DeniedPositions, priceId, sourcePositionId, positionId, ref count);

                var associatedPositionsToCreate = CreateAssociatedPositionsGroups(allPricePositionDescendantsDto.AssociatedPositionsGroups,
                                                                                  allPricePositionDescendantsDto.AssociatedPositionsMapping,
                                                                                  pricePosition.Id,
                                                                                  ref count);

                CreateAssociatedPositions(associatedPositionsToCreate, ref count);

                operationScope.Complete();

                return count;
            }
        }

        private void PerformValidation(long priceId, long positionId)
        {
            var isPriceExist = _priceReadModel.IsPriceExist(priceId);
            if (!isPriceExist)
            {
                _logger.Fatal(BLResources.UnableToGetExisitingPrice);
                throw new NotificationException(BLResources.UnableToGetExisitingPrice);
            }

            var isPriceContainsPosition = _priceReadModel.IsPriceContainsPosition(priceId, positionId);
            if (isPriceContainsPosition)
            {
                _logger.Fatal(BLResources.PricePositionForPositionAlreadyCreated);
                throw new NotificationException(BLResources.PricePositionForPositionAlreadyCreated);
            }

            var isPriceContainsPositionWithinNonDeleted = _priceReadModel.IsPriceContainsPositionWithinNonDeleted(priceId, positionId);
            if (isPriceContainsPositionWithinNonDeleted)
            {
                _logger.Fatal(BLResources.HiddenPricePositionForPositionAlreadyCreated);
                throw new NotificationException(BLResources.HiddenPricePositionForPositionAlreadyCreated);
            }
        }

        private void CreateDeniedPositions(IEnumerable<DeniedPosition> enumerableDeniedPositions,
                                           long priceId,
                                           long sourcePositionId,
                                           long positionId,
                                           ref int count)
        {
            var deniedPositions = enumerableDeniedPositions as DeniedPosition[] ?? enumerableDeniedPositions.ToArray();

            var selfDeniedPositions = deniedPositions.Where(x => x.PositionId == x.PositionDeniedId).ToArray();
            foreach (var deniedPosition in selfDeniedPositions)
            {
                deniedPosition.PositionId = positionId;
                deniedPosition.PositionDeniedId = positionId;
            }

            count += _bulkCreateDeniedPositionsAggregateService.Create(selfDeniedPositions, priceId);

            var nonSelfDeniedPositions = deniedPositions.Where(x => x.PositionId != x.PositionDeniedId && x.PositionId == sourcePositionId)
                                                        .GroupBy(x => x.PositionId)
                                                        .SingleOrDefault();

            var symmetricNonSelfDeniedPositions = deniedPositions.Where(x => x.PositionId != x.PositionDeniedId && x.PositionDeniedId == sourcePositionId)
                                                                 .GroupBy(x => x.PositionDeniedId)
                                                                 .SingleOrDefault();

            if ((nonSelfDeniedPositions == null && symmetricNonSelfDeniedPositions != null) ||
                (nonSelfDeniedPositions != null && symmetricNonSelfDeniedPositions == null))
            {
                throw new InvalidDataException("Price denied positions configurations is invalid");
            }
            
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (nonSelfDeniedPositions == null && symmetricNonSelfDeniedPositions == null)
            {
                return;
            }

            foreach (var deniedPosition in nonSelfDeniedPositions)
            {
                deniedPosition.PositionId = positionId;
            }

            count += _bulkCreateDeniedPositionsAggregateService.Create(nonSelfDeniedPositions, priceId);

            foreach (var deniedPosition in symmetricNonSelfDeniedPositions)
            {
                deniedPosition.PositionDeniedId = positionId;
            }

            count += _bulkCreateDeniedPositionsAggregateService.Create(symmetricNonSelfDeniedPositions, priceId);
        }

        private Dictionary<long, IEnumerable<AssociatedPosition>> CreateAssociatedPositionsGroups(
            IEnumerable<AssociatedPositionsGroup> enumerableAssociatedPositionsGroups,
            IDictionary<long, IEnumerable<AssociatedPosition>> associatedPositionsMapping,
            long pricePositionId,
            ref int count)
        {
            var associatedPositionsGroups = enumerableAssociatedPositionsGroups as AssociatedPositionsGroup[] ?? enumerableAssociatedPositionsGroups.ToArray();
            
            var associatedPositionsSnapshot = associatedPositionsGroups.Select(x => associatedPositionsMapping[x.Id]).ToArray();

            count += _bulkCreateAssociatedPositionsGroupsAggregateService.Create(associatedPositionsGroups, pricePositionId);

            var associatedPositionsToCreate = associatedPositionsGroups
                .Select(x => x.Id)
                .Zip(associatedPositionsSnapshot,
                     (groupId, associatedPositions) => new KeyValuePair<long, IEnumerable<AssociatedPosition>>(groupId, associatedPositions));

            return associatedPositionsToCreate.ToDictionary(x => x.Key, x => x.Value);
        }

        private void CreateAssociatedPositions(Dictionary<long, IEnumerable<AssociatedPosition>> associatedPositionsToCreate, ref int count)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var associatedPositions in associatedPositionsToCreate)
            {
                var associatedPositionsGroupId = associatedPositions.Key;
                var positions = associatedPositions.Value;
                count += _bulkCreateAssociatedPositionsAggregateService.Create(positions, associatedPositionsGroupId);
            }
        }
    }
}