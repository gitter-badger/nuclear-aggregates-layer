using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices
{
    public class PositionRepository : IPositionRepository
    {
        private readonly IFinder _finder;

        private readonly IRepository<Position> _positionGenericRepository;
        private readonly IRepository<PositionChildren> _positionChildrenGenericRepository;
        private readonly IRepository<PositionCategory> _positionCategoryGenericRepository;
        private readonly IIdentityProvider _identityProvider;

        public PositionRepository(IFinder finder, IRepository<Position> positionGenericRepository, IRepository<PositionChildren> positionChildrenGenericRepository, IRepository<PositionCategory> positionCategoryGenericRepository, IIdentityProvider identityProvider)
        {
            _finder = finder;
            _positionGenericRepository = positionGenericRepository;
            _positionChildrenGenericRepository = positionChildrenGenericRepository;
            _positionCategoryGenericRepository = positionCategoryGenericRepository;
            _identityProvider = identityProvider;
        }

        public int Activate(Position position)
        {
            if (IsInPublishedPrices(position.Id))
            {
                throw new ArgumentException(BLResources.CantActivatePositionRelatedToCompositeRelatedToPublishedPricePosition);
            }

            position.IsActive = true;
            _positionGenericRepository.Update(position);
            return _positionGenericRepository.Save();
        }

        public int Delete(Position position)
        {
            _positionGenericRepository.Delete(position);
            return _positionGenericRepository.Save();
        }

        int IDeleteAggregateRepository<Position>.Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<Position>(entityId)).Single();
            return Delete(entity);
        }

        public int Delete(PositionChildren link)
        {
            _positionChildrenGenericRepository.Delete(link);
            return _positionChildrenGenericRepository.Save();
        }

        int IDeleteAggregateRepository<PositionChildren>.Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<PositionChildren>(entityId)).Single();
            return Delete(entity);
        }

        public int Delete(PositionCategory entity)
        {
            _positionCategoryGenericRepository.Delete(entity);
            return _positionCategoryGenericRepository.Save();
        }

        int IDeleteAggregateRepository<PositionCategory>.Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<PositionCategory>(entityId)).Single();
            return Delete(entity);
        }

        public int DeleteWithSubentities(Position position)
        {
            var positionChildrens = position.IsComposite ?
                _finder.Find<PositionChildren>(pc => !pc.IsDeleted && pc.MasterPositionId == position.Id) :
                _finder.Find<PositionChildren>(pc => !pc.IsDeleted && pc.ChildPositionId == position.Id);

            foreach (var link in positionChildrens)
            {
                _positionChildrenGenericRepository.Delete(link);
            }

            _positionGenericRepository.Delete(position);
            return _positionGenericRepository.Save() + _positionChildrenGenericRepository.Save();
        }

        public string[] GetMasterPositionNames(Position position)
        {
            return _finder.Find<Position>(p => !p.IsDeleted && p.Id == position.Id)
                .SelectMany(p => p.MasterPositions)
                .Select(children => children.MasterPosition.Name)
                .ToArray();
        }

        public bool IsReadOnlyAdvertisementTemplate(long positionId)
        {
            return _finder.Find<Position>(x => x.Id == positionId)
                .SelectMany(x => x.OrderPositionAdvertisements)
                .Select(x => x.OrderPosition)
                .Any(x => x.IsActive && !x.IsDeleted);
        }

        public Position GetPosition(long entityId)
        {
            return _finder.Find(Specs.Find.ById<Position>(entityId)).SingleOrDefault();
        }

        public CategoryWithPositionsDto GetCategoryWithPositions(long entityId)
        {
            return _finder.Find(Specs.Find.ById<PositionCategory>(entityId))
                .Select(category => new CategoryWithPositionsDto
                                    {
                                        PositionCategory = category,
                                        Positions = category.Positions
                                    })
                .SingleOrDefault();
        }

        public bool IsInPublishedPrices(long positionId)
        {
            var childPositions = _finder.Find<Position>(x => x.Id == positionId).SelectMany(x => x.ChildPositions).Where(x => x.IsActive && !x.IsDeleted).Select(x => x.ChildPosition);

            var isInPublishedPrices = _finder.Find<Position>(x => x.Id == positionId).Union(childPositions)
                                      .SelectMany(x => x.PricePositions)
                                      .Where(x => !x.IsDeleted)
                                      .Select(x => x.Price)
                                      .Distinct()
                                      .Any(x => x.IsPublished);
            return isInPublishedPrices;
        }

        public int Deactivate(Position position)
        {
            var isUsedAsChildElement = _finder.Find(PositionSpecifications.Find.UsedAsChildElement(position.Id)).Any();
            if (isUsedAsChildElement)
            {
                var masterElementName = _finder.Find(Specs.Find.ById<Position>(position.Id))
                    .SelectMany(x => x.MasterPositions)
                    .Select(x => x.MasterPosition.Name)
                    .First();
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, BLResources.PositionIsUsedInCompositePosition, masterElementName));
            }

            if (IsInPublishedPrices(position.Id))
            {
                throw new ArgumentException(BLResources.ErrorCantDeletePositionInPublishedPrice);
            }

            position.IsActive = false;
            _positionGenericRepository.Update(position);
            return _positionGenericRepository.Save();
        }

        int IActivateAggregateRepository<Position>.Activate(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<Position>(entityId)).Single();
            return Activate(entity);
        }

        int IDeactivateAggregateRepository<Position>.Deactivate(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<Position>(entityId)).Single();
            return Deactivate(entity);
        }

        public void CreateOrUpdate(Position position)
        {
            var isAlreadyExist = _finder.Find<Position>(x => x.Name == position.Name && x.PlatformId == position.PlatformId && x.Id != position.Id && x.IsActive && !x.IsDeleted).Any();
            if (isAlreadyExist)
            {
                throw new NotificationException(BLResources.PositionAlreadyExist);
            }

            var hasChildren = _finder.Find<Position>(x => x.Id == position.Id).SelectMany(x => x.ChildPositions).Any(x => x.IsActive && !x.IsDeleted);
            if (hasChildren && !position.IsComposite)
            {
                throw new NotificationException(BLResources.CantChangeCompositePositionToSimpleWithoutDeletingChildrenPositions);
            }

            var hasParent = _finder.Find<Position>(x => x.Id == position.Id).SelectMany(x => x.MasterPositions).Any(x => x.IsActive && !x.IsDeleted);
            if (hasParent && position.IsComposite)
            {
                throw new NotificationException(BLResources.ErrorPositionIsNotComposite);
            }

            if (position.IsComposite && position.RestrictChildPositionPlatforms && PositionHasDifferentChildPositionPlatforms(position))
            {
                throw new ArgumentException(BLResources.PositionViolatesPlatformRestriction);
            }

            if (ChildPositionViolatesPlatformRestriction(position))
            {
                throw new ArgumentException(BLResources.PositionViolatesPlatformRestriction);
            }

            if (!position.IsNew() &&
                _finder.Find(Specs.Find.ById<Position>(position.Id)).Single().AdvertisementTemplateId != position.AdvertisementTemplateId &&
                IsReadOnlyAdvertisementTemplate(position.Id))
            {
                throw new NotificationException(BLResources.CannotChangePositionAdvertisementTemplate);
            }

            var isInPublishedPrices = IsInPublishedPrices(position.Id);
            if (isInPublishedPrices)
            {
                var originalPosition = _finder.Find<Position>(x => x.Id == position.Id).Single();

                if (position.PlatformId != originalPosition.PlatformId)
                {
                    throw new NotificationException(BLResources.ErrorInPublishedPricesPlatform);
                }

                if (position.CategoryId != originalPosition.CategoryId)
                {
                    throw new NotificationException(BLResources.ErrorInPublishedPricesCategory);
                }
            }

            if (position.IsNew())
            {
                _positionGenericRepository.Add(position);
            }
            else
            {
                _positionGenericRepository.Update(position);
            }

            _positionGenericRepository.Save();
        }

        public void CreateOrUpdate(PositionChildren positionChildren)
        {
            var childPositionInfo = _finder.Find<Position>(x => x.Id == positionChildren.ChildPositionId)
            .Select(x => new
            {
                x.AccountingMethodEnum,
                x.IsComposite,
                x.PlatformId,
            })
            .Single();

            if (childPositionInfo.IsComposite)
            {
                throw new NotificationException(BLResources.CantAddCompositePosition);
            }

            var isAccountingMethodNotMatched = _finder.Find<PositionChildren>(x => !x.IsDeleted &&
                          x.MasterPositionId == positionChildren.MasterPositionId &&
                          x.ChildPosition.AccountingMethodEnum != childPositionInfo.AccountingMethodEnum)
                          .Any();
            if (isAccountingMethodNotMatched)
            {
                throw new NotificationException(BLResources.CantAddChildPositionWithDifferentAccountingMethod);
            }

            var masterPosition = _finder.Find(Specs.Find.ById<Position>(positionChildren.MasterPositionId)).Single();
            if (masterPosition.RestrictChildPositionPlatforms && masterPosition.PlatformId != childPositionInfo.PlatformId)
            {
                throw new NotificationException(BLResources.PositionViolatesPlatformRestriction);
            }

            if (positionChildren.IsNew())
            {
                _identityProvider.SetFor(positionChildren);
                _positionChildrenGenericRepository.Add(positionChildren);
            }
            else
            {
                _positionChildrenGenericRepository.Update(positionChildren);                
            }

            _positionChildrenGenericRepository.Save();
        }

        public void CreateOrUpdate(PositionCategory category)
        {
            if (category.IsNew())
            {
                _positionCategoryGenericRepository.Add(category);
            }
            else
            {
                _positionCategoryGenericRepository.Update(category);
            }

            _positionCategoryGenericRepository.Save();
        }

        public int Update(PositionCategory position)
        {
            _positionCategoryGenericRepository.Update(position);
            return _positionCategoryGenericRepository.Save();
        }

        private bool ChildPositionViolatesPlatformRestriction(Position position)
        {
            return _finder.Find<PositionChildren>(link => link.IsActive && !link.IsDeleted && link.ChildPositionId == position.Id)
                   .Select(link => link.MasterPosition)
                   .Any(masterPosition => masterPosition.RestrictChildPositionPlatforms
                       && masterPosition.PlatformId != position.PlatformId);
        }

        private bool PositionHasDifferentChildPositionPlatforms(Position position)
        {
            return _finder.Find<PositionChildren>(link => link.IsActive && !link.IsDeleted && link.MasterPositionId == position.Id)
                   .Select(link => link.ChildPosition)
                   .Any(childPosition => childPosition.PlatformId != position.PlatformId);
        }
    }
}
