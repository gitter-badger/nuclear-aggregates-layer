using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.Aggregates.Prices.ReadModel;
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
        private readonly IBulkCreateDeniedPositionsAggregateService _createDeniedPositionAggregateService;
        private readonly IBulkCreateAssociatedPositionsGroupsAggregateService _bulkCreateAssociatedPositionsGroupsAggregateService;
        private readonly IBulkCreateAssociatedPositionsAggregateService _bulkCreateAssociatedPositionsAggregateService;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public CopyPriceOperationService(IPriceReadModel priceReadModel,
                                         IOrganizationUnitReadModel organizationUnitReadModel,
                                         ICreatePriceAggregateService createPriceAggregateService,
                                         ICreatePricePositionAggregateService createPricePositionAggregateService,
                                         IBulkCreateDeniedPositionsAggregateService createDeniedPositionAggregateService,
                                         IBulkCreateAssociatedPositionsGroupsAggregateService bulkCreateAssociatedPositionsGroupsAggregateService,
                                         IBulkCreateAssociatedPositionsAggregateService bulkCreateAssociatedPositionsAggregateService,
                                         IOperationScopeFactory operationScopeFactory)
        {
            _priceReadModel = priceReadModel;
            _organizationUnitReadModel = organizationUnitReadModel;
            _createPriceAggregateService = createPriceAggregateService;
            _createPricePositionAggregateService = createPricePositionAggregateService;
            _createDeniedPositionAggregateService = createDeniedPositionAggregateService;
            _bulkCreateAssociatedPositionsGroupsAggregateService = bulkCreateAssociatedPositionsGroupsAggregateService;
            _bulkCreateAssociatedPositionsAggregateService = bulkCreateAssociatedPositionsAggregateService;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Copy(long sourcePriceId, long organizationUnitId, DateTime publishDate, DateTime beginDate)
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

            return CopyInternal(sourcePriceId, targetPrice);
        }

        public int Copy(long sourcePriceId, long organizationUnitId, DateTime createDate, DateTime publishDate, DateTime beginDate)
        {
            var targetPrice = new Price
                {
                    OrganizationUnitId = organizationUnitId,
                    CreateDate = createDate,
                    PublishDate = publishDate,
                    BeginDate = beginDate,

                    IsActive = true
                };

            return CopyInternal(sourcePriceId, targetPrice);
        }

        private int CopyInternal(long sourcePriceId, Price targetPrice)
        {
            using (var operationScope = _operationScopeFactory.CreateNonCoupled<CopyPriceIdentity>())
            {
                var allPriceDescendantsDto = _priceReadModel.GetAllPriceDescendantsDto(sourcePriceId);

                var currencyId = _organizationUnitReadModel.GetCurrencyId(targetPrice.OrganizationUnitId);
                var count = _createPriceAggregateService.Create(targetPrice, currencyId);

                var associatedPositionsGroupsToCreate = CreatePricePositions(targetPrice.Id,
                                                                             allPriceDescendantsDto.PricePositions,
                                                                             allPriceDescendantsDto.AssociatedPositionsGroupsMapping,
                                                                             ref count);

                CreateDeniedPositions(targetPrice.Id, allPriceDescendantsDto.DeniedPositions, ref count);

                var associatedPositionsToCreate = CreateAssociatedPositionsGroups(associatedPositionsGroupsToCreate,
                                                                                  allPriceDescendantsDto.AssociatedPositionsMapping,
                                                                                  ref count);
                CreateAssociatedPositions(associatedPositionsToCreate, ref count);

                operationScope.Complete();

                return count;
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
            IDictionary<long, IEnumerable<AssociatedPositionsGroup>> associatedPositionsGroupsMapping,
            ref int count)
        {
            var associatedPositionsGroupsToCreate = new Dictionary<long, IEnumerable<AssociatedPositionsGroup>>();
            foreach (var pricePosition in pricePositions)
            {
                var associatedPositionsGroups = associatedPositionsGroupsMapping[pricePosition.Id];

                count += _createPricePositionAggregateService.Create(pricePosition, targetPriceId, pricePosition.PositionId);

                associatedPositionsGroupsToCreate.Add(pricePosition.Id, associatedPositionsGroups);
            }

            return associatedPositionsGroupsToCreate;
        }

        private void CreateDeniedPositions(long targetPriceId, IEnumerable<DeniedPosition> deniedPositions, ref int count)
        {
            count += _createDeniedPositionAggregateService.Create(deniedPositions, targetPriceId);
        }

        private Dictionary<long, IEnumerable<AssociatedPosition>> CreateAssociatedPositionsGroups(
            Dictionary<long, IEnumerable<AssociatedPositionsGroup>> associatedPositionsGroupsToCreate,
            IDictionary<long, IEnumerable<AssociatedPosition>> associatedPositionsMapping,
            ref int count)
        {
            var associatedPositionsToCreate = new Dictionary<long, IEnumerable<AssociatedPosition>>();
            foreach (var associatedPositionsGroups in associatedPositionsGroupsToCreate)
            {
                var pricePositionId = associatedPositionsGroups.Key;
                var groups = associatedPositionsGroups.Value;

                var associatedPositionsSnapshot = groups.Select(x => associatedPositionsMapping[x.Id]).ToArray();

                count += _bulkCreateAssociatedPositionsGroupsAggregateService.Create(groups, pricePositionId);

                var associatedPositionsByUpdatedGroupIds = groups
                    .Select(x => x.Id)
                    .Zip(associatedPositionsSnapshot,
                         (groupId, associatedPositions) => new KeyValuePair<long, IEnumerable<AssociatedPosition>>(groupId, associatedPositions));

                associatedPositionsToCreate = associatedPositionsToCreate.Concat(associatedPositionsByUpdatedGroupIds).ToDictionary(x => x.Key, x => x.Value);
            }

            return associatedPositionsToCreate;
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