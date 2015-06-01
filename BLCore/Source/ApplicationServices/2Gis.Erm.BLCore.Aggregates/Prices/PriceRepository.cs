using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;
using NuClear.Storage.Specifications;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices
{
    public class PriceRepository : IPriceRepository
    {
        private readonly IRepository<Price> _priceGenericRepository;
        private readonly IRepository<AssociatedPositionsGroup> _associatedPositionsGroupGenericRepository;
        private readonly IRepository<AssociatedPosition> _associatedPositionGenericRepository;
        private readonly IFinder _finder;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public PriceRepository(
            IFinder finder,
            IRepository<Price> priceGenericRepository,
            IRepository<AssociatedPositionsGroup> associatedPositionsGroupGenericRepository,
            IRepository<AssociatedPosition> associatedPositionGenericRepository,
            IIdentityProvider identityProvider,
            IOperationScopeFactory operationScopeFactory)
        {
            _priceGenericRepository = priceGenericRepository;
            _associatedPositionsGroupGenericRepository = associatedPositionsGroupGenericRepository;
            _associatedPositionGenericRepository = associatedPositionGenericRepository;
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
            _finder = finder;
        }

        public int Activate(AssociatedPositionsGroup associatedPositionsGroup)
        {
            var associatedPositions = _finder
                .Find(AssociatedPositionSpecifications.Find.AssociatedPositionsByGroup(associatedPositionsGroup.Id))
                .Many();

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

        public int Deactivate(AssociatedPositionsGroup associatedPositionsGroup)
        {
            if (!associatedPositionsGroup.IsActive)
            {
                throw new ArgumentException(BLResources.AssociatedPositionsGroupIsInactiveAlready);
            }

            var associatedPositionsGroupInfo = _finder
                .FindObsolete(Specs.Find.ById<AssociatedPositionsGroup>(associatedPositionsGroup.Id))
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

            var priceInfo = _finder.FindObsolete(Specs.Find.ById<AssociatedPosition>(associatedPosition.Id))
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

        public IEnumerable<PricePositionDto> GetPricePositions(IEnumerable<long> requiredPriceIds, IEnumerable<long> requiredPositionIds)
        {
            return _finder.Find(new FindSpecification<PricePosition>(x => requiredPriceIds.Contains(x.PriceId) && requiredPositionIds.Contains(x.PositionId)))
                          .Map(q => q.Select(x => new PricePositionDto
                              {
                                  Id = x.Id,
                                  PositionId = x.PositionId,
                                  PositionName = x.Position.Name,
                                  PriceId = x.PriceId,
                                  Groups = x.AssociatedPositionsGroups
                                            .Where(y => y.IsActive && !y.IsDeleted)
                                            .Select(y => y.AssociatedPositions
                                                          .Where(z => z.IsActive && !z.IsDeleted)
                                                          .Select(z => new PricePositionDto.RelatedItemDto
                                                              {
                                                                  PositionId = z.PositionId,
                                                                  BindingCheckMode = z.ObjectBindingType,
                                                              })),
                                  DeniedPositions = x.Price.DeniedPositions
                                                     .Where(y => y.PositionId == x.PositionId && y.IsActive && !y.IsDeleted)
                                                     .Select(y => new PricePositionDto.RelatedItemDto
                                                         {
                                                             PositionId = y.PositionDeniedId,
                                                             BindingCheckMode = y.ObjectBindingType
                                                         })
                              }))
                          .Many();
        }

        public bool TryGetActualPriceId(long organizationUnitId, DateTime date, out long actualPriceId)
        {
            var priceId = _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId))
                                 .Map(q => q.SelectMany(unit => unit.Prices)
                                            .Where(price => price.IsActive && !price.IsDeleted && price.IsPublished && price.BeginDate <= date)
                                            .OrderByDescending(y => y.BeginDate)
                                            .Select(price => price.Id))
                                 .Top();

            actualPriceId = priceId;
            return priceId != 0;
        }

        public void Publish(Price price, long organizationUnitId, DateTime beginDate, DateTime publishDate)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Price>())
            {
            price.PublishDate = publishDate;
            price.IsPublished = true;

            price.OrganizationUnitId = organizationUnitId;
            price.BeginDate = beginDate;

            _priceGenericRepository.Update(price);
                operationScope.Updated<Price>(price.Id);

            _priceGenericRepository.Save();

                operationScope.Complete();
            }
        }

        public void Unpublish(Price price)
            {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Price>())
            {
            price.IsPublished = false;

            _priceGenericRepository.Update(price);
                operationScope.Updated<Price>(price.Id);

            _priceGenericRepository.Save();

                operationScope.Complete();
            }
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
            var isPositionsMatch = _finder.Find(new FindSpecification<AssociatedPositionsGroup>(x => x.IsActive && !x.IsDeleted
                                                                               && x.Id == associatedPosition.AssociatedPositionsGroupId
                                                                               && x.PricePosition.IsActive && !x.PricePosition.IsDeleted
                                                                                                     && x.PricePosition.PositionId == associatedPosition.PositionId))
                                          .Any();

            if (isPositionsMatch)
            {
                throw new NotificationException(BLResources.AssociatedPositionMustDifferFromPricePosition);
            }

            var isPositionExist = _finder.Find(new FindSpecification<AssociatedPosition>(x => x.IsActive && !x.IsDeleted &&
                            x.AssociatedPositionsGroupId == associatedPosition.AssociatedPositionsGroupId &&
                            x.Id != associatedPosition.Id &&
                                                                                              x.PositionId == associatedPosition.PositionId))
                                         .Any();

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

        int IActivateAggregateRepository<AssociatedPositionsGroup>.Activate(long entityId)
        {
            var entity = _finder.FindObsolete(Specs.Find.ById<AssociatedPositionsGroup>(entityId)).Single();
            return Activate(entity);
        }

        int IDeactivateAggregateRepository<AssociatedPositionsGroup>.Deactivate(long entityId)
        {
            var entity = _finder.FindObsolete(Specs.Find.ById<AssociatedPositionsGroup>(entityId)).Single();
            return Deactivate(entity);
        }

        int IDeactivateAggregateRepository<AssociatedPosition>.Deactivate(long entityId)
        {
            var entity = _finder.FindObsolete(Specs.Find.ById<AssociatedPosition>(entityId)).Single();
            return Deactivate(entity);
        }
    }
}
