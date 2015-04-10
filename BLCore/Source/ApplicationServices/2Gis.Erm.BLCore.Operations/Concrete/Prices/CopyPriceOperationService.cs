using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Prices
{
    public class CopyPriceOperationService : ICopyPriceOperationService
    {
        private readonly IPriceReadModel _priceReadModel;
        private readonly IOrganizationUnitReadModel _organizationUnitReadModel;
        private readonly ICreatePriceAggregateService _createPriceAggregateService;
        private readonly ICreatePricePositionAggregateService _createPricePositionAggregateService;
        private readonly IBulkCreateAssociatedPositionsGroupsAggregateService _bulkCreateAssociatedPositionsGroupsAggregateService;
        private readonly IBulkCreateAssociatedPositionsAggregateService _bulkCreateAssociatedPositionsAggregateService;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISymmetricDeniedPositionsVerifier _symmetricDeniedPositionsVerifier;
        private readonly IDeniedPositionsDuplicatesVerifier _deniedPositionsDuplicatesVerifier;
        private readonly ICopyDeniedPositionsAggregateService _copyDeniedPositionsAggregateService;

        public CopyPriceOperationService(IPriceReadModel priceReadModel,
                                         IOrganizationUnitReadModel organizationUnitReadModel,
                                         ICreatePriceAggregateService createPriceAggregateService,
                                         ICreatePricePositionAggregateService createPricePositionAggregateService,
                                         IBulkCreateAssociatedPositionsGroupsAggregateService bulkCreateAssociatedPositionsGroupsAggregateService,
                                         IBulkCreateAssociatedPositionsAggregateService bulkCreateAssociatedPositionsAggregateService,
                                         IOperationScopeFactory operationScopeFactory,
                                         ISymmetricDeniedPositionsVerifier symmetricDeniedPositionsVerifier,
                                         IDeniedPositionsDuplicatesVerifier deniedPositionsDuplicatesVerifier,
                                         ICopyDeniedPositionsAggregateService copyDeniedPositionsAggregateService)
        {
            _priceReadModel = priceReadModel;
            _organizationUnitReadModel = organizationUnitReadModel;
            _createPriceAggregateService = createPriceAggregateService;
            _createPricePositionAggregateService = createPricePositionAggregateService;
            _bulkCreateAssociatedPositionsGroupsAggregateService = bulkCreateAssociatedPositionsGroupsAggregateService;
            _bulkCreateAssociatedPositionsAggregateService = bulkCreateAssociatedPositionsAggregateService;
            _operationScopeFactory = operationScopeFactory;
            _symmetricDeniedPositionsVerifier = symmetricDeniedPositionsVerifier;
            _deniedPositionsDuplicatesVerifier = deniedPositionsDuplicatesVerifier;
            _copyDeniedPositionsAggregateService = copyDeniedPositionsAggregateService;
        }

        public void Copy(long sourcePriceId, long organizationUnitId, DateTime publishDate, DateTime beginDate)
        {
            ValidatePrice(sourcePriceId, organizationUnitId, publishDate, beginDate);

            var targetPrice = new Price
                                  {
                                      OrganizationUnitId = organizationUnitId,
                                      CreateDate = DateTime.UtcNow.Date,
                                      PublishDate = publishDate,
                                      BeginDate = beginDate,

                                      IsActive = true
                                  };

            CopyInternal(sourcePriceId, targetPrice);
        }

        public void Copy(long sourcePriceId, long organizationUnitId, DateTime createDate, DateTime publishDate, DateTime beginDate)
        {
            var targetPrice = new Price
                                  {
                                      OrganizationUnitId = organizationUnitId,
                                      CreateDate = createDate,
                                      PublishDate = publishDate,
                                      BeginDate = beginDate,

                                      IsActive = true
                                  };

            CopyInternal(sourcePriceId, targetPrice);
        }

        private void CopyInternal(long sourcePriceId, Price targetPrice)
        {
            using (var operationScope = _operationScopeFactory.CreateNonCoupled<CopyPriceIdentity>())
            {
                var allPriceDescendantsDto = _priceReadModel.GetAllPriceDescendantsDto(sourcePriceId);

                var currencyId = _organizationUnitReadModel.GetCurrencyId(targetPrice.OrganizationUnitId);
                _createPriceAggregateService.Create(targetPrice, currencyId);

                var associatedPositionsGroupsToCreate = CreatePricePositions(targetPrice.Id,
                                                                             allPriceDescendantsDto.PricePositions.Where(x => x.IsActive && !x.IsDeleted),
                                                                             allPriceDescendantsDto.AssociatedPositionsGroupsMapping);

                CreateDeniedPositions(allPriceDescendantsDto.DeniedPositions.Where(x => x.IsActive && !x.IsDeleted).ToArray(), targetPrice.Id);

                var associatedPositionsToCreate = CreateAssociatedPositionsGroups(associatedPositionsGroupsToCreate,
                                                                                  allPriceDescendantsDto.AssociatedPositionsMapping);
                CreateAssociatedPositions(associatedPositionsToCreate);

                operationScope.Complete();
            }
        }

        // TODO {all, 12.03.2014}: Валидация прайса выполняется и при публикации и при сохранении, и теперь при копировании. Нужно вынести эту логику отдельно
        public void ValidatePrice(long priceId, long organizationUnitId, DateTime publishDate, DateTime beginDate)
        {
            var minimalDate = DateTime.UtcNow.AddMonths(1);
            
            var lowBoundSatisfied = beginDate.Year > minimalDate.Year || (beginDate.Year == minimalDate.Year && beginDate.Month >= minimalDate.Month);
            if (!lowBoundSatisfied)
            {
                throw new NotificationException(BLResources.BeginMonthMustBeGreaterOrEqualThanNextMonth);
            }

            if (beginDate < publishDate)
            {
                throw new NotificationException(string.Format(BLResources.BeginDateMustBeNotLessThan,
                                                              publishDate.AddDays(1 - publishDate.Day).AddMonths(1).ToShortDateString()));
            }

            var isPriceExist = _priceReadModel.IsDifferentPriceExistsForDate(priceId, organizationUnitId, beginDate);
            if (isPriceExist)
            {
                var organizationUnitName = _organizationUnitReadModel.GetName(organizationUnitId);
                throw new NotificationException(string.Format(BLResources.PriceForOrgUnitExistsForDate, organizationUnitName, beginDate.ToShortDateString()));
            }
        }

        private Dictionary<long, IEnumerable<AssociatedPositionsGroup>> CreatePricePositions(
            long targetPriceId,
            IEnumerable<PricePosition> pricePositions,
            IDictionary<long, IEnumerable<AssociatedPositionsGroup>> associatedPositionsGroupsMapping)
        {
            var associatedPositionsGroupsToCreate = new Dictionary<long, IEnumerable<AssociatedPositionsGroup>>();
            foreach (var pricePosition in pricePositions)
            {
                var associatedPositionsGroups = associatedPositionsGroupsMapping[pricePosition.Id];

                pricePosition.PriceId = targetPriceId;
                _createPricePositionAggregateService.Create(pricePosition);

                associatedPositionsGroupsToCreate.Add(pricePosition.Id, associatedPositionsGroups);
            }

            return associatedPositionsGroupsToCreate;
        }

        private Dictionary<long, IEnumerable<AssociatedPosition>> CreateAssociatedPositionsGroups(
            Dictionary<long, IEnumerable<AssociatedPositionsGroup>> associatedPositionsGroupsToCreate,
            IDictionary<long, IEnumerable<AssociatedPosition>> associatedPositionsMapping)
        {
            var associatedPositionsToCreate = new Dictionary<long, IEnumerable<AssociatedPosition>>();
            foreach (var associatedPositionsGroups in associatedPositionsGroupsToCreate)
            {
                var pricePositionId = associatedPositionsGroups.Key;
                var groups = associatedPositionsGroups.Value;

                var associatedPositionsSnapshot = groups.Select(x => associatedPositionsMapping[x.Id]).ToArray();

                _bulkCreateAssociatedPositionsGroupsAggregateService.Create(groups, pricePositionId);

                var associatedPositionsByUpdatedGroupIds = groups
                    .Select(x => x.Id)
                    .Zip(associatedPositionsSnapshot,
                         (groupId, associatedPositions) => new KeyValuePair<long, IEnumerable<AssociatedPosition>>(groupId, associatedPositions));

                associatedPositionsToCreate = associatedPositionsToCreate.Concat(associatedPositionsByUpdatedGroupIds).ToDictionary(x => x.Key, x => x.Value);
            }

            return associatedPositionsToCreate;
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

        private void CreateDeniedPositions(IEnumerable<DeniedPosition> deniedPositionsToCreate, long targetPriceId)
        {
            var deniedPositions = deniedPositionsToCreate.DistinctDeniedPositions();
            _deniedPositionsDuplicatesVerifier.VerifyForDuplicatesWithinCollection(deniedPositions);
            _symmetricDeniedPositionsVerifier.VerifyForSymmetryWithinCollection(deniedPositions);

            _copyDeniedPositionsAggregateService.Copy(deniedPositions, targetPriceId);
        }
    }
}