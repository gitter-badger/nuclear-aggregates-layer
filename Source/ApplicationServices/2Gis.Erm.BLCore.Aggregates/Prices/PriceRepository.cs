using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices
{
    #region Dto Definitions

    public sealed class PriceDto
    {
        public long OrganizationUnitId { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public sealed class PricePositionDto
    {
        public long Id { get; set; }

        public long PositionId { get; set; }

        public long PriceId { get; set; }

        public IEnumerable<IEnumerable<RelatedItemDto>> Groups { get; set; }

        public IEnumerable<RelatedItemDto> DeniedPositions { get; set; }

        public sealed class RelatedItemDto
        {
            public ObjectBindingType BindingCheckMode { get; set; }
            public long PositionId { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                var dto = obj as RelatedItemDto;
                if (dto == null)
                {
                    return false;
                }

                return PositionId == dto.PositionId && BindingCheckMode == dto.BindingCheckMode;
            }

            public override int GetHashCode()
            {
                return BindingCheckMode.GetHashCode() ^ PositionId.GetHashCode();
            }
        }
    }

    public sealed class PriceToCopyDto
    {
        public Price Price { get; set; }
        public IEnumerable<DeniedPosition> DeniedPositions { get; set; }
        public IEnumerable<PricePositionWithGroupsDto> PricePositions { get; set; }

        public sealed class PricePositionWithGroupsDto
        {
            public PricePosition Item { get; set; }
            public IEnumerable<GroupWithAssociatedPositionsDto> AssociatedPositionsGroups { get; set; }
        }

        public sealed class GroupWithAssociatedPositionsDto
        {
            public AssociatedPositionsGroup Item { get; set; }
            public IEnumerable<AssociatedPosition> AssociatedPositions { get; set; } 
        }
    }

    #endregion

    public class PriceRepository : IPriceRepository
    {
        private readonly IRepository<Price> _priceGenericRepository;
        private readonly IRepository<PricePosition> _pricePositionGenericRepository;
        private readonly IRepository<AssociatedPositionsGroup> _associatedPositionsGroupGenericRepository;
        private readonly IRepository<AssociatedPosition> _associatedPositionGenericRepository;
        private readonly IRepository<DeniedPosition> _deniedPositionGenericRepository;
        private readonly IFinder _finder;
        private readonly IIdentityProvider _identityProvider;

        public PriceRepository(
            IFinder finder,
            IRepository<Price> priceGenericRepository,
            IRepository<PricePosition> pricePositionGenericRepository,
            IRepository<AssociatedPositionsGroup> associatedPositionsGroupGenericRepository,
            IRepository<AssociatedPosition> associatedPositionGenericRepository,
            IRepository<DeniedPosition> deniedPositionGenericRepository, 
            IIdentityProvider identityProvider)
        {
            _priceGenericRepository = priceGenericRepository;
            _pricePositionGenericRepository = pricePositionGenericRepository;
            _associatedPositionsGroupGenericRepository = associatedPositionsGroupGenericRepository;
            _associatedPositionGenericRepository = associatedPositionGenericRepository;
            _deniedPositionGenericRepository = deniedPositionGenericRepository;
            _identityProvider = identityProvider;
            _finder = finder;
        }

        public int Add(Price price)
        {
            _identityProvider.SetFor(price);
            _priceGenericRepository.Add(price);
            return _priceGenericRepository.Save();
        }

        public int Add(PricePosition pricePosition)
        {
            _identityProvider.SetFor(pricePosition);
            _pricePositionGenericRepository.Add(pricePosition);
            return _pricePositionGenericRepository.Save();
        }

        public int Add(AssociatedPositionsGroup associatedPositionsGroup)
        {
            _identityProvider.SetFor(associatedPositionsGroup);
            _associatedPositionsGroupGenericRepository.Add(associatedPositionsGroup);
            return _associatedPositionsGroupGenericRepository.Save();
        }

        public int Add(AssociatedPosition associatedPosition)
        {
            _identityProvider.SetFor(associatedPosition);
            _associatedPositionGenericRepository.Add(associatedPosition);
            return _associatedPositionGenericRepository.Save();
        }

        public int Add(DeniedPosition deniedPosition)
        {
            _identityProvider.SetFor(deniedPosition);
            _deniedPositionGenericRepository.Add(deniedPosition);
            return _deniedPositionGenericRepository.Save();
        }

        public int Update(Price price)
        {
            _priceGenericRepository.Update(price);
            return _priceGenericRepository.Save();
        }

        public int Activate(DeniedPosition deniedPosition)
        {
            deniedPosition.IsActive = true;
            _deniedPositionGenericRepository.Update(deniedPosition);

            if (deniedPosition.PositionId != deniedPosition.PositionDeniedId)
            {
                var symmetricDeniedPosition = _finder
                    .Find<DeniedPosition>(x => !x.IsActive && !x.IsDeleted &&
                               x.PriceId == deniedPosition.PriceId &&
                               x.PositionId == deniedPosition.PositionDeniedId &&
                               x.PositionDeniedId == deniedPosition.PositionId)
                    .Single();

                symmetricDeniedPosition.IsActive = true;
                _deniedPositionGenericRepository.Update(symmetricDeniedPosition);
            }

            return _deniedPositionGenericRepository.Save();
        }

        public int Activate(AssociatedPositionsGroup associatedPositionsGroup)
        {
            var associatedPositions = _finder
                .Find(AssociatedPositionSpecifications.Find.AssociatedPositionsByGroup(associatedPositionsGroup.Id))
                .ToArray();

            foreach (var associatedPosition in associatedPositions)
            {
                associatedPosition.IsActive = true;
                _associatedPositionGenericRepository.Update(associatedPosition);
            }
            _associatedPositionGenericRepository.Save();
            
            associatedPositionsGroup.IsActive = true;
            _associatedPositionsGroupGenericRepository.Update(associatedPositionsGroup);
            return _associatedPositionsGroupGenericRepository.Save();
        }

        public int Activate(PricePosition pricePosition)
        {
            var samePositionExists = _finder.Find<PricePosition>(x => x.PriceId == pricePosition.PriceId &&
                                                                               x.PositionId == pricePosition.PositionId &&
                                                                               x.IsActive && !x.IsDeleted).Any();
            if (samePositionExists)
            {
                throw new ArgumentException(BLResources.AlreadyExistsPricePositionWithSamePositionNotification);
            }

            var pricePositionInfo = _finder.Find(Specs.Find.ById<PricePosition>(pricePosition.Id))
                .Select(x => new
                    {
                        x.AssociatedPositionsGroups,
                        AssociatedPositions = x.AssociatedPositionsGroups.SelectMany(y => y.AssociatedPositions),
                        DeniedPositions = x.Price.DeniedPositions.Where(y => y.PositionId == x.PositionId || y.PositionDeniedId == x.PositionId)
                    })
                .Single();

            foreach (var associatedPositionsGroup in pricePositionInfo.AssociatedPositionsGroups)
            {
                associatedPositionsGroup.IsActive = true;
                _associatedPositionsGroupGenericRepository.Update(associatedPositionsGroup);
            }
            _associatedPositionsGroupGenericRepository.Save();

            foreach (var associatedPosition in pricePositionInfo.AssociatedPositions)
            {
                associatedPosition.IsActive = true;
                _associatedPositionGenericRepository.Update(associatedPosition);
            }
            _associatedPositionGenericRepository.Save();

            foreach (var deniedPosition in pricePositionInfo.DeniedPositions)
            {
                deniedPosition.IsActive = true;
                _deniedPositionGenericRepository.Update(deniedPosition);
            }
            _deniedPositionGenericRepository.Save();

            pricePosition.IsActive = true;
            _pricePositionGenericRepository.Update(pricePosition);
            return _pricePositionGenericRepository.Save();
        }

        public int Activate(Price price)
        {
            var priceInfo = _finder.Find(Specs.Find.ById<Price>(price.Id))
                .Select(x => new
                    {
                        x.PricePositions,
                        AssociatedPositionsGroups = x.PricePositions.SelectMany(y => y.AssociatedPositionsGroups),
                        AssociatedPositions = x.PricePositions.SelectMany(y => y.AssociatedPositionsGroups).SelectMany(y => y.AssociatedPositions),
                        x.DeniedPositions
                    })
                .Single();

            foreach (var pricePosition in priceInfo.PricePositions)
            {
                pricePosition.IsActive = true;
                _pricePositionGenericRepository.Update(pricePosition);
            }
            _pricePositionGenericRepository.Save();

            foreach (var associatedPositionsGroup in priceInfo.AssociatedPositionsGroups)
            {
                associatedPositionsGroup.IsActive = true;
                _associatedPositionsGroupGenericRepository.Update(associatedPositionsGroup);
            }
            _associatedPositionsGroupGenericRepository.Save();

            foreach (var associatedPosition in priceInfo.AssociatedPositions)
            {
                associatedPosition.IsActive = true;
                _associatedPositionGenericRepository.Update(associatedPosition);
            }
            _associatedPositionGenericRepository.Save();

            foreach (var deniedPosition in priceInfo.DeniedPositions)
            {
                deniedPosition.IsActive = true;
                _deniedPositionGenericRepository.Update(deniedPosition);
            }
            _deniedPositionGenericRepository.Save();

            price.IsActive = true;
            price.IsPublished = false;
            _priceGenericRepository.Update(price);
            return _priceGenericRepository.Save();
        }

        public int Deactivate(Price price)
        {
            var priceInfo = _finder.Find(Specs.Find.ById<Price>(price.Id))
                .Select(x => new
                    {
                        x.IsActive,
                        x.PricePositions,
                        AssociatedPositionsGroups = x.PricePositions.SelectMany(y => y.AssociatedPositionsGroups),
                        AssociatedPositions = x.PricePositions.SelectMany(y => y.AssociatedPositionsGroups).SelectMany(y => y.AssociatedPositions),
                        x.DeniedPositions
                    })
                .Single();
            if (!priceInfo.IsActive)
            {
                throw new ArgumentException(BLResources.PriceIsInactiveAlready);
            }

            foreach (var pricePosition in priceInfo.PricePositions)
            {
                pricePosition.IsActive = false;
                _pricePositionGenericRepository.Update(pricePosition);
            }
            _pricePositionGenericRepository.Save();

            foreach (var associatedPositionsGroup in priceInfo.AssociatedPositionsGroups)
            {
                associatedPositionsGroup.IsActive = false;
                _associatedPositionsGroupGenericRepository.Update(associatedPositionsGroup);
            }
            _associatedPositionsGroupGenericRepository.Save();

            foreach (var associatedPosition in priceInfo.AssociatedPositions)
            {
                associatedPosition.IsActive = false;
                _associatedPositionGenericRepository.Update(associatedPosition);
            }
            _associatedPositionGenericRepository.Save();

            foreach (var deniedPosition in priceInfo.DeniedPositions)
            {
                deniedPosition.IsActive = false;
                _deniedPositionGenericRepository.Update(deniedPosition);
            }
            _deniedPositionGenericRepository.Save();

            price.IsActive = false;
            _priceGenericRepository.Update(price);
            return _priceGenericRepository.Save();
        }

        public int Deactivate(PricePosition pricePosition)
        {
            var pricePositionInfo = _finder.Find(Specs.Find.ById<PricePosition>(pricePosition.Id))
                .Select(x => new
                    {
                        x.IsActive,
                        IsPriceActive = x.Price.IsActive,
                        IsPricePublished = x.Price.IsPublished,
                        x.AssociatedPositionsGroups,
                        AssociatedPositions = x.AssociatedPositionsGroups.SelectMany(y => y.AssociatedPositions),
                        DeniedPositions = x.Price.DeniedPositions.Where(y => y.PositionId == x.PositionId || y.PositionDeniedId == x.PositionId)
                    })
                .Single();

            if (!pricePositionInfo.IsActive)
            {
                throw new ArgumentException(BLResources.PricePositionIsInactiveAlready);
            }
            if (!pricePositionInfo.IsPriceActive)
            {
                throw new ArgumentException(BLResources.CantDeativatePricePositionWhenPriceIsDeactivated);
            }
            if (pricePositionInfo.IsPricePublished)
            {
                throw new ArgumentException(BLResources.CantDeativatePricePositionWhenPriceIsPublished);
            }

            foreach (var associatedPositionsGroup in pricePositionInfo.AssociatedPositionsGroups)
            {
                associatedPositionsGroup.IsActive = false;
                _associatedPositionsGroupGenericRepository.Update(associatedPositionsGroup);
            }
            _associatedPositionsGroupGenericRepository.Save();

            foreach (var associatedPosition in pricePositionInfo.AssociatedPositions)
            {
                associatedPosition.IsActive = false;
                _associatedPositionGenericRepository.Update(associatedPosition);
            }
            _associatedPositionGenericRepository.Save();

            foreach (var deniedPosition in pricePositionInfo.DeniedPositions)
            {
                deniedPosition.IsActive = false;
                _deniedPositionGenericRepository.Update(deniedPosition);
            }
            _deniedPositionGenericRepository.Save();

            pricePosition.IsActive = false;
            _pricePositionGenericRepository.Update(pricePosition);
            return _pricePositionGenericRepository.Save();
        }

        public int Deactivate(AssociatedPositionsGroup associatedPositionsGroup)
        {
            if (!associatedPositionsGroup.IsActive)
            {
                throw new ArgumentException(BLResources.AssociatedPositionsGroupIsInactiveAlready);
            }

            var associatedPositionsGroupInfo = _finder
                .Find(Specs.Find.ById<AssociatedPositionsGroup>(associatedPositionsGroup.Id))
                .Select(x => new
                    {
                        IsPricePublished = x.PricePosition.Price.IsPublished,
                        IsPriceActive = x.PricePosition.Price.IsActive,
                        x.AssociatedPositions
                    })
                .Single();
            if (associatedPositionsGroupInfo.IsPricePublished)
            {
                throw new ArgumentException(BLResources.CantDeactivateAssociatedPositionsGroupWhenPriceIsPublished);
            }
            if (!associatedPositionsGroupInfo.IsPriceActive)
            {
                throw new ArgumentException(BLResources.CantDeactivateAssociatedPositionsGroupWhenPriceIsDeactivated);
            }

            foreach (var associatedPosition in associatedPositionsGroupInfo.AssociatedPositions)
            {
                associatedPosition.IsActive = false;
                _associatedPositionGenericRepository.Update(associatedPosition);
            }
            _associatedPositionGenericRepository.Save();

            associatedPositionsGroup.IsActive = false;
            _associatedPositionsGroupGenericRepository.Update(associatedPositionsGroup);
            return _associatedPositionsGroupGenericRepository.Save();
        }

        public int Deactivate(AssociatedPosition associatedPosition)
        {
            if (!associatedPosition.IsActive)
            {
                throw new ArgumentException(BLResources.AssociatedPositionIsInactiveAlready);
            }

            var priceInfo = _finder.Find(Specs.Find.ById<AssociatedPosition>(associatedPosition.Id))
                .Select(x => new
                    {
                        x.AssociatedPositionsGroup.PricePosition.Price.IsPublished,
                        x.AssociatedPositionsGroup.PricePosition.Price.IsActive
                    })
                .Single();
            if (priceInfo.IsPublished)
            {
                throw new ArgumentException(BLResources.CantDeactivateAssociatedPositionWhenPriceIsPublished);
            }
            if (!priceInfo.IsActive)
            {
                throw new ArgumentException(BLResources.CantDeactivateAssociatedPositionWhenPriceIsDeactivated);
            }

            associatedPosition.IsActive = false;
            _associatedPositionGenericRepository.Update(associatedPosition);
            return _associatedPositionGenericRepository.Save();
        }

        public int Deactivate(DeniedPosition deniedPosition)
        {
            if (!deniedPosition.IsActive)
            {
                throw new NotificationException(BLResources.DeniedPositionIsDeactivatedAlready);
            }

            var deniedPositionInfo = _finder.Find(Specs.Find.ById<DeniedPosition>(deniedPosition.Id))
                .Select(x => new
                    {
                        IsPricePublished = x.Price.IsPublished,
                        IsPriceActive = x.Price.IsActive
                    })
                .Single();

            if (deniedPositionInfo.IsPricePublished)
            {
                throw new NotificationException(BLResources.CantDeactivateDeniedPositionWhenPriceIsPublished);
            }
            if (!deniedPositionInfo.IsPriceActive)
            {
                throw new NotificationException(BLResources.CantDeactivateDeniedPositionWhenPriceIsDeactivated);
            }

            deniedPosition.IsActive = false;
            _deniedPositionGenericRepository.Update(deniedPosition);
            if (deniedPosition.PositionId != deniedPosition.PositionDeniedId)
            {
                var symmetricDeniedPosition = _finder
                    .Find<DeniedPosition>(x => x.IsActive && !x.IsDeleted &&
                               x.PriceId == deniedPosition.PriceId &&
                               x.PositionId == deniedPosition.PositionDeniedId &&
                               x.PositionDeniedId == deniedPosition.PositionId)
                    .Single();
                symmetricDeniedPosition.IsActive = false;
                _deniedPositionGenericRepository.Update(symmetricDeniedPosition);
            }

            return _deniedPositionGenericRepository.Save();
        }

        public void CheckPriceBusinessRules(long priceId, long organizationUnitId, DateTime? beginDateTime, DateTime? publishDateTime)
        {
            var minimalDate = DateTime.UtcNow.AddMonths(1);
            if (!beginDateTime.HasValue || !publishDateTime.HasValue) 
                return;

            var beginDate = beginDateTime.Value;
            var publishDate = publishDateTime.Value;

            bool lowBoundSatisfied = beginDate.Year > minimalDate.Year || (beginDate.Year == minimalDate.Year && beginDate.Month >= minimalDate.Month);

            if (!lowBoundSatisfied)
            {
                throw new NotificationException(BLResources.BeginMonthMustBeGreaterOrEqualThanNextMonth);
            }

            if (beginDate < publishDate)
            {
                throw new NotificationException(string.Format(BLResources.BeginDateMustBeNotLessThan,
                                                              publishDate.AddDays(1 - publishDate.Day)
                                                                  .AddMonths(1)
                                                                  .ToShortDateString()));
            }

            var isPriceExist = IsPriceExistForDate(organizationUnitId, beginDate.Date);
            if (isPriceExist)
            {
                var orgUnit = _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId)).Single();
                throw new NotificationException(string.Format(CultureInfo.InvariantCulture, BLResources.PriceForOrgUnitExistsForDate,
                                                              orgUnit.Name, beginDate.ToShortDateString()));
            }
        }

        public long GetCurrencyId(long organizationUnitId)
        {
            return _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId))
                .Select(x => x.Country.CurrencyId)
                .Single();
        }

        public PriceDto GetPriceDto(long priceId)
        {
            return _finder.Find(Specs.Find.ById<Price>(priceId))
                          .Select(x => new PriceDto
                                           {
                                               BeginDate = x.BeginDate,
                                               CreateDate = x.CreateDate,
                                               OrganizationUnitId = x.OrganizationUnitId,
                                               PublishDate = x.PublishDate
                                           })
                          .Single();
        }

        public IEnumerable<AssociatedPositionsGroup> GetAssociatedPositionsGroups(long pricePositionId)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<AssociatedPositionsGroup>())
                .Where(positionGroup => positionGroup.PricePositionId == pricePositionId)
                .ToArray();
        }

        public IEnumerable<AssociatedPosition> GetAssociatedPositions(long positionsGroupId)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<AssociatedPosition>())
                .Where(position => position.AssociatedPositionsGroupId == positionsGroupId)
                .ToArray();
        }

        public bool PriceHasLinkedOrders(long entityId)
        {
            return _finder.Find(Specs.Find.ById<Price>(entityId))
                .Any(price => price.PricePositions.Any(y => y.OrderPositions.Any(z => z.Order.IsActive)));
        }

        public void DeleteWithSubentities(long entityId)
        {
            var priceInfo = _finder.Find(Specs.Find.ById<Price>(entityId))
                .Select(price => new
                {
                    Price = price,
                    price.DeniedPositions,
                    PricePositions = price.PricePositions
                                 .Select(pp => new
                                 {
                                     PricePosition = pp,
                                     AssociatedPositionsGroups = pp.AssociatedPositionsGroups
                                               .Select(apg => new
                                               {
                                                   AssociatedPositionsGroup = apg,
                                                   apg.AssociatedPositions
                                               })
                                 })
                })
                .FirstOrDefault();

            if (priceInfo == null)
            {
                throw new ArgumentException(BLResources.EntityNotFound);
            }

            foreach (var pricePosition in priceInfo.PricePositions)
            {
                foreach (var group in pricePosition.AssociatedPositionsGroups)
                {
                    foreach (var associatedPosition in group.AssociatedPositions)
                        Delete(associatedPosition);

                    Delete(group.AssociatedPositionsGroup);
                }
                Delete(pricePosition.PricePosition);
            }

            foreach (var deniedPosition in priceInfo.DeniedPositions)
                Delete(deniedPosition);

            Delete(priceInfo.Price);
        }

        public PricePosition GetPricePosition(long pricePositionId)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<PricePosition>())
                .SingleOrDefault(position => position.Id == pricePositionId);
        }

        public PriceToCopyDto GetPriceToCopyDto(long sourcePriceId)
        {
            return _finder.Find(Specs.Find.ById<Price>(sourcePriceId)).Select(priceItem => new PriceToCopyDto
            {
                Price = priceItem,
                DeniedPositions = priceItem.DeniedPositions.Where(x => x.IsActive && !x.IsDeleted),
                PricePositions = priceItem.PricePositions.Where(x => x.IsActive && !x.IsDeleted).Select(pricePositionItem => new PriceToCopyDto.PricePositionWithGroupsDto
                {
                    Item = pricePositionItem,
                    AssociatedPositionsGroups = pricePositionItem.AssociatedPositionsGroups.Where(x => x.IsActive && !x.IsDeleted).Select(positionGroupItem => new PriceToCopyDto.GroupWithAssociatedPositionsDto
                    {
                        Item = positionGroupItem,
                        AssociatedPositions = positionGroupItem.AssociatedPositions.Where(x => x.IsActive && !x.IsDeleted)
                    })
                })
            })
                .SingleOrDefault();
        }

        public IEnumerable<DeniedPosition> GetDeniedPositions(long priceId, long positionId)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<DeniedPosition>())
                .Where(position => position.PriceId == priceId && position.PositionId == positionId)
                .ToArray();
        }

        public bool PriceExists(long priceId)
        {
            return _finder.Find(Specs.Find.ById<Price>(priceId)).Any();
        }

        public bool PricePublishedForToday(long priceId)
        {
            var utcToday = DateTime.UtcNow.Date;
            return _finder.Find<Price>(x => x.Id == priceId).Where(x => x.IsActive && !x.IsDeleted).Any(x => x.IsPublished && x.BeginDate <= utcToday);
        }

        public bool PriceContainsPosition(long priceId, long positionId)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<PricePosition>())
                .Any(position => position.PriceId == priceId && position.PositionId == positionId);
        }

        public IEnumerable<PricePositionDto> GetPricePositions(IEnumerable<long> requiredPriceIds, IEnumerable<long> requiredPositionIds)
        {
            return _finder.Find<PricePosition>(x => requiredPriceIds.Contains(x.PriceId) && requiredPositionIds.Contains(x.PositionId))
                .Select(x => new PricePositionDto()
                {
                    Id = x.Id,
                    PositionId = x.PositionId,
                    PriceId = x.PriceId,
                    Groups = x.AssociatedPositionsGroups
                             .Where(y => y.IsActive && !y.IsDeleted)
                             .Select(y => y.AssociatedPositions
                                              .Where(z => z.IsActive && !z.IsDeleted)
                                              .Select(z => new PricePositionDto.RelatedItemDto()
                                              {
                                                  PositionId = z.PositionId,
                                                  BindingCheckMode = (ObjectBindingType)z.ObjectBindingType,
                                              })),
                    DeniedPositions = x.Price.DeniedPositions
                             .Where(y => y.PositionId == x.PositionId && y.IsActive && !y.IsDeleted)
                             .Select(y => new PricePositionDto.RelatedItemDto()
                             {
                                 PositionId = y.PositionDeniedId,
                                 BindingCheckMode = (ObjectBindingType)y.ObjectBindingType
                             })
                })
                .ToArray();
        }

        int IActivateAggregateRepository<AssociatedPositionsGroup>.Activate(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<AssociatedPositionsGroup>(entityId)).Single();
            return Activate(entity);
        }

        int IActivateAggregateRepository<DeniedPosition>.Activate(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<DeniedPosition>(entityId)).Single();
            return Activate(entity);
        }

        int IActivateAggregateRepository<PricePosition>.Activate(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<PricePosition>(entityId)).Single();
            return Activate(entity);
        }

        int IActivateAggregateRepository<Price>.Activate(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<Price>(entityId)).Single();
            return Activate(entity);
        }

        int IDeactivateAggregateRepository<Price>.Deactivate(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<Price>(entityId)).Single();
            return Deactivate(entity);
        }

        int IDeactivateAggregateRepository<PricePosition>.Deactivate(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<PricePosition>(entityId)).Single();
            return Deactivate(entity);
        }

        int IDeactivateAggregateRepository<AssociatedPositionsGroup>.Deactivate(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<AssociatedPositionsGroup>(entityId)).Single();
            return Deactivate(entity);
        }

        int IDeactivateAggregateRepository<AssociatedPosition>.Deactivate(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<AssociatedPosition>(entityId)).Single();
            return Deactivate(entity);
        }

        int IDeactivateAggregateRepository<DeniedPosition>.Deactivate(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<DeniedPosition>(entityId)).Single();
            return Deactivate(entity);
        }   

        private bool IsPriceExistForDate(long organizationUnitId, DateTime beginDate)
        {
            return _finder.Find<Price>(price => !price.IsDeleted
                                                         && price.IsActive
                                                         && price.OrganizationUnitId == organizationUnitId
                                                         && price.BeginDate == beginDate)
                .Any();
        }

        private void Delete(Price price)
        {
            _priceGenericRepository.Delete(price);
            _priceGenericRepository.Save();
        }

        private void Delete(PricePosition pricePosition)
        {
            _pricePositionGenericRepository.Delete(pricePosition);
            _pricePositionGenericRepository.Save();
        }

        private void Delete(AssociatedPositionsGroup associatedPositionsGroup)
        {
            _associatedPositionsGroupGenericRepository.Delete(associatedPositionsGroup);
            _associatedPositionsGroupGenericRepository.Save();
        }

        private void Delete(AssociatedPosition associatedPosition)
        {
            _associatedPositionGenericRepository.Delete(associatedPosition);
            _associatedPositionGenericRepository.Save();
        }

        private void Delete(DeniedPosition deniedPosition)
        {
            _deniedPositionGenericRepository.Delete(deniedPosition);
            _deniedPositionGenericRepository.Save();
        }

        public long GetPriceOrganizationUnitId(long priceId)
        {
            return _finder.Find(Specs.Find.ById<Price>(priceId))
                .Select(x => x.OrganizationUnitId)
                .Single();
        }

        public void CreateOrUpdate(Price price)
        {
            ValidatePrice(price.Id, price.OrganizationUnitId, price.BeginDate, price.PublishDate);

            var currency = _finder.Find<OrganizationUnit>(x => x.Id == price.OrganizationUnitId).Select(x => x.Country.Currency).SingleOrDefault();
            if (currency == null)
                throw new NotificationException(BLResources.CurrencyNotSpecifiedForPrice);

            price.CurrencyId = currency.Id;

            if (price.IsNew())
            {
                _identityProvider.SetFor(price);
                _priceGenericRepository.Add(price);
            }
            else
            {
                _priceGenericRepository.Update(price);
            }

            _priceGenericRepository.Save();
        }

        public bool TryGetActualPriceId(long organizationUnitId, DateTime date, out long actualPriceId)
        {
            var priceId = _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId))
                                 .SelectMany(unit => unit.Prices)
                                 .Where(price => price.IsActive && !price.IsDeleted && price.IsPublished && price.BeginDate <= date)
                                 .OrderByDescending(y => y.BeginDate)
                                 .Select(price => price.Id)
                                 .FirstOrDefault();

            actualPriceId = priceId;
            return priceId != 0;
        }

        public void Publish(long priceId, long organizationUnitId, DateTime beginDate, DateTime publishDate)
        {
            ValidatePrice(priceId, organizationUnitId, beginDate, publishDate);

            if (priceId == 0)
            {
                throw new NotificationException(BLResources.PriceIsNeedToBeSavedBeforePublishing);                
            }
            if (publishDate < DateTime.UtcNow.Date)
            {
                throw new BusinessLogicException(BLResources.CantPublishOverduePrice);
            }

            var priceDto = _finder.Find<Price>(x => x.Id == priceId).Select(x => new
            {
                Price = x,

                DeniedPositionsNotValid = x.DeniedPositions.Where(y => !y.IsDeleted).Select(y => y.PositionDenied).Distinct().Any(y => !y.IsActive || y.IsDeleted),
                PricePositionsNotValid = x.PricePositions.Where(y => y.IsActive && !y.IsDeleted).Select(y => y.Position).Distinct().Any(y => !y.IsActive || y.IsDeleted),

                AssociatedPositionsNotValid = x.PricePositions.Where(y => y.IsActive && !y.IsDeleted)
                                              .SelectMany(y => y.AssociatedPositionsGroups).Distinct()
                                              .SelectMany(y => y.AssociatedPositions).Distinct()
                                              .Select(y => y.Position).Any(y => !y.IsActive || y.IsDeleted),
            }).Single();

            if (priceDto.Price.IsDeleted)
            {
                throw new BusinessLogicException(BLResources.CantPublishInactivePrice);
            }

            if (priceDto.DeniedPositionsNotValid)
            {
                throw new BusinessLogicException(BLResources.DeniedPositionsReferencesToInactivePositions);
            }

            if (priceDto.PricePositionsNotValid)
            {
                throw new BusinessLogicException(BLResources.PricePositionsReferencesToInactivePositions);
            }
            if (priceDto.AssociatedPositionsNotValid)
            {
                throw new BusinessLogicException(BLResources.AssociatedPositionsReferencesToInactivePositions);
            }

            var price = priceDto.Price;
            price.PublishDate = publishDate;
            price.IsPublished = true;

            price.OrganizationUnitId = organizationUnitId;
            price.BeginDate = beginDate;

            _priceGenericRepository.Update(price);
            _priceGenericRepository.Save();
        }

        private void ValidatePrice(long priceId, long organizationUnitId, DateTime beginDate, DateTime publishDate)
        {
            var minimalDate = DateTime.UtcNow.AddMonths(1);

            var lowBoundSatisfied = beginDate.Year > minimalDate.Year || (beginDate.Year == minimalDate.Year && beginDate.Month >= minimalDate.Month);
            if (!lowBoundSatisfied)
            {
                throw new NotificationException(BLResources.BeginMonthMustBeGreaterOrEqualThanNextMonth);
            }

            if (beginDate < publishDate)
            {
                throw new NotificationException(string.Format(BLResources.BeginDateMustBeNotLessThan, publishDate.AddDays(1 - publishDate.Day).AddMonths(1).ToShortDateString()));
            }

            var isPriceExist = _finder.Find<Price>(price => !price.IsDeleted && price.IsActive
                                                    && price.Id != priceId
                                                    && price.OrganizationUnitId == organizationUnitId
                                                    && price.BeginDate == beginDate.Date).Any();
            if (isPriceExist)
            {
                var organizationUnitName = _finder.Find<OrganizationUnit>(x => x.Id == organizationUnitId).Select(x => x.Name).Single();
                throw new NotificationException(string.Format(CultureInfo.InvariantCulture, BLResources.PriceForOrgUnitExistsForDate, organizationUnitName, beginDate.ToShortDateString()));
            }
        }

        public void Unpublish(long priceId)
        {
            // TODO {v.lapeev, 07.11.2013}: Код ниже выполняет проверки актуальности прайса и наличия размещаемых заказов. Нужно включить эти проверки снова, как только процесс работы с прайсами наладится
            /*
            var now = DateTime.UtcNow;
            var startOfNextMonth = new DateTime(now.AddMonths(1).Year, now.AddMonths(1).Month, 1);

            var actualPriceId = _finder.Find(Specs.Find.ById<Price>(priceId))
                                   .SelectMany(x => x.OrganizationUnit.Prices.Where(p => p.IsActive && !p.IsDeleted && p.IsPublished && p.BeginDate < startOfNextMonth))
                                   .OrderByDescending(x => x.BeginDate)
                                   .Select(x => x.Id)
                                   .FirstOrDefault();

            if (priceId == actualPriceId)
            {
                throw new NotificationException(BLResources.CannotUnpublishActualPrice);
            }

            var isLinked = _finder.Find(Specs.Find.ById<Price>(priceId) && PriceSpecs.Prices.Find.Linked()).Any();

            if (isLinked)
            {
                throw new NotificationException(BLResources.CannotUnpublishLinkedPrice);
            }
            */

            var price = _finder.Find(Specs.Find.ById<Price>(priceId)).Single();

            price.IsPublished = false;

            _priceGenericRepository.Update(price);
            _priceGenericRepository.Save();
        }

        public void CreateOrUpdate(PricePosition pricePosition)
        {
            if (pricePosition.Amount == null && pricePosition.AmountSpecificationMode == (int)PricePositionAmountSpecificationMode.FixedValue)
            {
                throw new NotificationException(BLResources.CountMustBeSpecified);
            }

            var isAlreadyCreated = _finder.Find<Price>(x => x.Id == pricePosition.PriceId).SelectMany(x => x.PricePositions)
                                    .Where(x => x.IsActive && !x.IsDeleted)
                                    .Any(x => x.PositionId == pricePosition.PositionId && x.Id != pricePosition.Id);
            if (isAlreadyCreated)
            {
                throw new NotificationException(BLResources.PricePositionForPositionAlreadyCreated);                
            }

            var isSupportedByExport = _finder.Find<Position>(x => x.Id == pricePosition.PositionId).Select(x => x.Platform.IsSupportedByExport).SingleOrDefault();
            if (!isSupportedByExport)
            {
                throw new NotificationException(BLResources.PricePositionPlatformIsNotSupportedByExport);
            }

            if (pricePosition.IsNew())
            {
                _identityProvider.SetFor(pricePosition);
                _pricePositionGenericRepository.Add(pricePosition);
            }
            else
            {
                _pricePositionGenericRepository.Update(pricePosition);
            }

            _pricePositionGenericRepository.Save();
        }

        public void CreateOrUpdate(AssociatedPositionsGroup associatedPositionGroup)
        {
            if (associatedPositionGroup.IsNew())
            {
                _identityProvider.SetFor(associatedPositionGroup);
                _associatedPositionsGroupGenericRepository.Add(associatedPositionGroup);
            }
            else
            {
                _associatedPositionsGroupGenericRepository.Update(associatedPositionGroup);
            }

            _associatedPositionsGroupGenericRepository.Save();
        }

        public void CreateOrUpdate(AssociatedPosition associatedPosition)
        {
            var isPositionsMatch = _finder.Find<AssociatedPositionsGroup>(x => x.IsActive && !x.IsDeleted
                                                                               && x.Id == associatedPosition.AssociatedPositionsGroupId
                                                                               && x.PricePosition.IsActive && !x.PricePosition.IsDeleted
                                                                               && x.PricePosition.PositionId == associatedPosition.PositionId).Any();

            if (isPositionsMatch)
            {
                throw new NotificationException(BLResources.AssociatedPositionMustDifferFromPricePosition);
            }

            var isPositionExist = _finder.Find<AssociatedPosition>(x => x.IsActive && !x.IsDeleted &&
                            x.AssociatedPositionsGroupId == associatedPosition.AssociatedPositionsGroupId &&
                            x.Id != associatedPosition.Id &&
                            x.PositionId == associatedPosition.PositionId).Any();

            if (isPositionExist)
            {
                throw new NotificationException(BLResources.SelectedAssociatedPositionAlreadyExist);
            }

            if (associatedPosition.IsNew())
            {
                _identityProvider.SetFor(associatedPosition);
                _associatedPositionGenericRepository.Add(associatedPosition);
            }
            else
            {
                _associatedPositionGenericRepository.Update(associatedPosition);
            }

            _associatedPositionGenericRepository.Save();
        }

        public void CreateOrUpdate(DeniedPosition deniedPosition)
        {
            var isDeniedPositionAlreadyExist = _finder.Find<DeniedPosition>(x => x.IsActive && !x.IsDeleted &&
                                    x.Id != deniedPosition.Id &&
                                    x.PriceId == deniedPosition.PriceId &&
                                    x.PositionId == deniedPosition.PositionId &&
                                    x.PositionDeniedId == deniedPosition.PositionDeniedId).Any();
            if (isDeniedPositionAlreadyExist)
            {
                throw new NotificationException(BLResources.DeniedPositionAlreadyExist);
            }

            if (deniedPosition.IsNew())
            {
                _identityProvider.SetFor(deniedPosition);
                _deniedPositionGenericRepository.Add(deniedPosition);
            }
            else
            {
                _deniedPositionGenericRepository.Update(deniedPosition);
            }

            var isSelfDeniedPosition = deniedPosition.PositionId == deniedPosition.PositionDeniedId;
            if(!isSelfDeniedPosition)
            {
                // symmetric denied position
                var symmetricDeniedPosition = _finder.Find<DeniedPosition>(x => x.IsActive && !x.IsDeleted &&
                                                                    x.PriceId == deniedPosition.PriceId &&
                                                                    x.PositionId == deniedPosition.PositionDeniedId &&
                                                                    x.PositionDeniedId == deniedPosition.PositionId)
                                                                    .SingleOrDefault();
                if (symmetricDeniedPosition == null)
                {
                    // create-only properties
                    symmetricDeniedPosition = new DeniedPosition
                    {
                        PriceId = deniedPosition.PriceId,
                        CreatedBy = deniedPosition.CreatedBy,
                        CreatedOn = deniedPosition.CreatedOn,
                        OwnerCode = deniedPosition.OwnerCode,
                            IsActive = true,

                        // common properties
                        PositionId = deniedPosition.PositionDeniedId,
                        PositionDeniedId = deniedPosition.PositionId,
                        ObjectBindingType = deniedPosition.ObjectBindingType
                    };

                    _identityProvider.SetFor(symmetricDeniedPosition);
                    _deniedPositionGenericRepository.Add(symmetricDeniedPosition);
                }
                else
                {
                    // commom properties
                    symmetricDeniedPosition.PositionId = deniedPosition.PositionDeniedId;
                    symmetricDeniedPosition.PositionDeniedId = deniedPosition.PositionId;
                    symmetricDeniedPosition.ObjectBindingType = deniedPosition.ObjectBindingType;

                    _deniedPositionGenericRepository.Update(symmetricDeniedPosition);
                }
            }

            _deniedPositionGenericRepository.Save();
        }
    }
}
