using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Prices
{
    public class CopyPricePositionOperationService : ICopyPricePositionOperationService
    {
        private readonly ITracer _tracer;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IPriceReadModel _priceReadModel;
        private readonly ICreatePricePositionAggregateService _createPricePositionAggregateService;
        private readonly IBulkCreateDeniedPositionsAggregateService _bulkCreateDeniedPositionsAggregateService;
        private readonly IBulkCreateAssociatedPositionsGroupsAggregateService _bulkCreateAssociatedPositionsGroupsAggregateService;
        private readonly IBulkCreateAssociatedPositionsAggregateService _bulkCreateAssociatedPositionsAggregateService;
        private readonly ISymmetricDeniedPositionsVerifier _symmetricDeniedPositionsVerifier;
        private readonly IDeniedPositionsDuplicatesVerifier _deniedPositionsDuplicatesVerifier;

        public CopyPricePositionOperationService(ITracer tracer,
                                                 IOperationScopeFactory operationScopeFactory,
                                                 IPriceReadModel priceReadModel,
                                                 ICreatePricePositionAggregateService createPricePositionAggregateService,
                                                 IBulkCreateDeniedPositionsAggregateService bulkCreateDeniedPositionsAggregateService,
                                                 IBulkCreateAssociatedPositionsGroupsAggregateService bulkCreateAssociatedPositionsGroupsAggregateService,
                                                 IBulkCreateAssociatedPositionsAggregateService bulkCreateAssociatedPositionsAggregateService,
                                                 ISymmetricDeniedPositionsVerifier symmetricDeniedPositionsVerifier,
                                                 IDeniedPositionsDuplicatesVerifier deniedPositionsDuplicatesVerifier)
        {
            _tracer = tracer;
            _operationScopeFactory = operationScopeFactory;
            _priceReadModel = priceReadModel;
            _createPricePositionAggregateService = createPricePositionAggregateService;
            _bulkCreateDeniedPositionsAggregateService = bulkCreateDeniedPositionsAggregateService;
            _bulkCreateAssociatedPositionsGroupsAggregateService = bulkCreateAssociatedPositionsGroupsAggregateService;
            _bulkCreateAssociatedPositionsAggregateService = bulkCreateAssociatedPositionsAggregateService;
            _symmetricDeniedPositionsVerifier = symmetricDeniedPositionsVerifier;
            _deniedPositionsDuplicatesVerifier = deniedPositionsDuplicatesVerifier;
        }

        public long Copy(long priceId, long sourcePricePositionId, long positionId)
        {
            PerformValidation(priceId, positionId);

            using (var operationScope = _operationScopeFactory.CreateNonCoupled<CopyPricePositionIdentity>())
            {
                var sourcePricePosition = _priceReadModel.GetPricePosition(sourcePricePositionId);
                if (sourcePricePosition == null)
                {
                    _tracer.Fatal(BLResources.UnableToGetExisitingPricePosition);
                    throw new EntityNotFoundException(typeof(PricePosition), sourcePricePositionId);
                }

                var pricePosition = sourcePricePosition.ResetToNew();

                var sourcePositionId = pricePosition.PositionId;
                var allPricePositionDescendantsDto = _priceReadModel.GetAllPricePositionDescendantsDto(sourcePricePositionId, sourcePositionId);
                
                pricePosition.PositionId = positionId;
                _createPricePositionAggregateService.Create(pricePosition);

                CreateDeniedPositions(allPricePositionDescendantsDto.DeniedPositions.Where(x => x.IsActive && !x.IsDeleted), sourcePositionId, positionId);

                var associatedPositionsToCreate = CreateAssociatedPositionsGroups(allPricePositionDescendantsDto.AssociatedPositionsGroups,
                                                                                  allPricePositionDescendantsDto.AssociatedPositionsMapping,
                                                                                  pricePosition.Id);

                CreateAssociatedPositions(associatedPositionsToCreate);

                operationScope.Complete();

                return pricePosition.Id;
            }
        }

        private void PerformValidation(long priceId, long positionId)
        {
            if (!_priceReadModel.DoesPriceExist(priceId))
            {
                _tracer.Fatal(BLResources.UnableToGetExisitingPrice);
                throw new EntityNotFoundException(typeof(Price), priceId);
            }

            if (_priceReadModel.DoesPriceContainPosition(priceId, positionId))
            {
                _tracer.Fatal(BLResources.PricePositionForPositionAlreadyCreated);
                throw new NotificationException(BLResources.PricePositionForPositionAlreadyCreated);
            }

            if (_priceReadModel.DoesPriceContainPositionWithinNonDeleted(priceId, positionId))
            {
                _tracer.Fatal(BLResources.HiddenPricePositionForPositionAlreadyCreated);
                throw new NotificationException(BLResources.HiddenPricePositionForPositionAlreadyCreated);
            }
        }

        private void CreateDeniedPositions(IEnumerable<DeniedPosition> enumerableDeniedPositions,
                                           long sourcePositionId,
                                           long positionId)
        {
            var allDeniedPositions = enumerableDeniedPositions.AsArray();

            var positionDeniedPositions = allDeniedPositions.Where(x => x.PositionId == sourcePositionId)
                                                            .ToArray();

            var symmetricDeniedPositions = allDeniedPositions.Where(x => x.PositionDeniedId == sourcePositionId)
                                                             .ToArray();

            if (!positionDeniedPositions.Any() && !symmetricDeniedPositions.Any())
            {
                return;
            }

            foreach (var deniedPosition in positionDeniedPositions)
            {
                deniedPosition.PositionId = positionId;
            }

            foreach (var deniedPosition in symmetricDeniedPositions)
            {
                deniedPosition.PositionDeniedId = positionId;
            }

            var allPositionDeniedPositions = positionDeniedPositions.Concat(symmetricDeniedPositions).DistinctDeniedPositions();
            _deniedPositionsDuplicatesVerifier.VerifyForDuplicatesWithinCollection(allPositionDeniedPositions);
            _symmetricDeniedPositionsVerifier.VerifyForSymmetryWithinCollection(allPositionDeniedPositions);

            _bulkCreateDeniedPositionsAggregateService.Create(allPositionDeniedPositions);
        }

        private Dictionary<long, IEnumerable<AssociatedPosition>> CreateAssociatedPositionsGroups(
            IEnumerable<AssociatedPositionsGroup> enumerableAssociatedPositionsGroups,
            IDictionary<long, IEnumerable<AssociatedPosition>> associatedPositionsMapping,
            long pricePositionId)
        {
            var associatedPositionsGroups = enumerableAssociatedPositionsGroups as AssociatedPositionsGroup[] ?? enumerableAssociatedPositionsGroups.ToArray();
            
            var associatedPositionsSnapshot = associatedPositionsGroups.Select(x => associatedPositionsMapping[x.Id]).ToArray();

            _bulkCreateAssociatedPositionsGroupsAggregateService.Create(associatedPositionsGroups, pricePositionId);

            var associatedPositionsToCreate = associatedPositionsGroups
                .Select(x => x.Id)
                .Zip(associatedPositionsSnapshot,
                     (groupId, associatedPositions) => new KeyValuePair<long, IEnumerable<AssociatedPosition>>(groupId, associatedPositions));

            return associatedPositionsToCreate.ToDictionary(x => x.Key, x => x.Value);
        }

        private void CreateAssociatedPositions(Dictionary<long, IEnumerable<AssociatedPosition>> associatedPositionsToCreate)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var associatedPositions in associatedPositionsToCreate)
            {
                var associatedPositionsGroupId = associatedPositions.Key;
                var positions = associatedPositions.Value;
                _bulkCreateAssociatedPositionsAggregateService.Create(positions, associatedPositionsGroupId);
            }
        }
    }
}