using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Storage;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices
{
    public class PositionRepository : IPositionRepository
    {
        private readonly IFinder _finder;
        private readonly IIdentityProvider _identityProvider;

        private readonly IRepository<PositionCategory> _positionCategoryGenericRepository;
        private readonly IRepository<PositionChildren> _positionChildrenGenericRepository;
        private readonly IRepository<Position> _positionGenericRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public PositionRepository(IFinder finder,
                                  IIdentityProvider identityProvider,
                                  IRepository<PositionCategory> positionCategoryGenericRepository,
                                  IRepository<PositionChildren> positionChildrenGenericRepository,
                                  IRepository<Position> positionGenericRepository,
                                  IOperationScopeFactory scopeFactory)
        {
            _finder = finder;
            _identityProvider = identityProvider;
            _positionCategoryGenericRepository = positionCategoryGenericRepository;
            _positionChildrenGenericRepository = positionChildrenGenericRepository;
            _positionGenericRepository = positionGenericRepository;
            _scopeFactory = scopeFactory;
        }

        int IDeleteAggregateRepository<Position>.Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<Position>(entityId)).Single();
            return Delete(entity);
        }

        int IDeleteAggregateRepository<PositionChildren>.Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<PositionChildren>(entityId)).Single();
            return Delete(entity);
        }

        public int Delete(PositionCategory entity)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, PositionCategory>())
            {
                _positionCategoryGenericRepository.Delete(entity);
                var cnt = _positionCategoryGenericRepository.Save();

                scope.Deleted(entity)
                     .Complete();

                return cnt;
            }
        }

        int IDeleteAggregateRepository<PositionCategory>.Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<PositionCategory>(entityId)).Single();
            return Delete(entity);
        }

        public int DeleteWithSubentities(Position position)
        {
            var positionChildrenQuery = position.IsComposite
                                            ? _finder.Find<PositionChildren>(pc => !pc.IsDeleted && pc.MasterPositionId == position.Id)
                                            : _finder.Find<PositionChildren>(pc => !pc.IsDeleted && pc.ChildPositionId == position.Id);

            var positionChildrenCollection = positionChildrenQuery.ToArray();

            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, Position>())
            {
                foreach (var link in positionChildrenCollection)
                {
                    _positionChildrenGenericRepository.Delete(link);
                }

                _positionGenericRepository.Delete(position);

                var cnt = _positionChildrenGenericRepository.Save();
                cnt += _positionGenericRepository.Save();

                scope.Deleted(positionChildrenCollection.AsEnumerable())
                     .Deleted(position)
                     .Complete();

                return cnt;
            }
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
            var masterPositions = _finder.Find(Specs.Find.ById<Position>(positionId))
                                         .SelectMany(x => x.MasterPositions)
                                         .Where(Specs.Find.ActiveAndNotDeleted<PositionChildren>())
                                         .Select(x => x.MasterPosition);

            var isInPublishedPrices = _finder.Find(Specs.Find.ById<Position>(positionId)).Union(masterPositions)
                                             .SelectMany(x => x.PricePositions)
                                             .Where(Specs.Find.NotDeleted<PricePosition>())
                                             .Select(x => x.Price)
                                             .Distinct()
                                             .Any(x => x.IsPublished);
            return isInPublishedPrices;
        }

        int IActivateAggregateRepository<Position>.Activate(long entityId)
        {
            if (IsInPublishedPrices(entityId))
            {
                throw new ArgumentException(BLResources.CantActivatePositionRelatedToCompositeRelatedToPublishedPricePosition);
            }

            var positionInfo = _finder.Find(Specs.Find.ById<Position>(entityId))
                          .Select(x => new
                          {
                              Position = x,
                              ChildPositions = x.ChildPositions.Where(cp => !cp.IsActive && !cp.IsDeleted)
                          })
                          .Single();

            using (var scope = _scopeFactory.CreateSpecificFor<ActivateIdentity, Position>())
            {
                foreach (var childPosition in positionInfo.ChildPositions)
                {
                    childPosition.IsActive = true;
                    _positionChildrenGenericRepository.Update(childPosition);
                }

                positionInfo.Position.IsActive = true;
                _positionGenericRepository.Update(positionInfo.Position);

                var cnt = _positionChildrenGenericRepository.Save();
                cnt += _positionGenericRepository.Save();

                scope.Updated(positionInfo.Position)
                     .Updated(positionInfo.ChildPositions)
                     .Complete();

                return cnt;
            }
        }

        int IDeactivateAggregateRepository<Position>.Deactivate(long entityId)
        {
            var isUsedAsChildElement = _finder.Find(PriceSpecs.Positions.Find.UsedAsChildElement(entityId)).Any();
            if (isUsedAsChildElement)
            {
                var masterElementName = _finder.Find(Specs.Find.ById<Position>(entityId))
                                               .SelectMany(x => x.MasterPositions)
                                               .Select(x => x.MasterPosition.Name)
                                               .First();

                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, BLResources.PositionIsUsedInCompositePosition, masterElementName));
            }

            if (IsInPublishedPrices(entityId))
            {
                throw new ArgumentException(BLResources.ErrorCantDeletePositionInPublishedPrice);
            }

            var positionInfo = _finder.Find(Specs.Find.ById<Position>(entityId))
                                      .Select(x => new
                                          {
                                              Position = x,
                                              ChildPositions = x.ChildPositions.Where(cp => cp.IsActive && !cp.IsDeleted)
                                          })
                                      .Single();

            using (var scope = _scopeFactory.CreateSpecificFor<DeactivateIdentity, Position>())
            {
                foreach (var childPosition in positionInfo.ChildPositions)
                {
                    childPosition.IsActive = false;
                    _positionChildrenGenericRepository.Update(childPosition);
                }

                positionInfo.Position.IsActive = false;
                _positionGenericRepository.Update(positionInfo.Position);

                var cnt = _positionChildrenGenericRepository.Save();
                cnt += _positionGenericRepository.Save();

                scope.Updated(positionInfo.Position)
                     .Updated(positionInfo.ChildPositions)
                     .Complete();

                return cnt;
            }
        }

        public void CreateOrUpdate(Position position)
        {
            var isAlreadyExist =
                _finder.Find<Position>(x => x.Name == position.Name && x.PlatformId == position.PlatformId && x.Id != position.Id && x.IsActive && !x.IsDeleted)
                       .Any();
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

            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(position))
            {
                if (position.IsNew())
                {
                    _positionGenericRepository.Add(position);
                    scope.Added(position);
                }
                else
                {
                    _positionGenericRepository.Update(position);
                    scope.Updated(position);
                }

                _positionGenericRepository.Save();

                scope.Complete();
            }
        }

        public void CreateOrUpdate(PositionChildren positionChildren)
        {
            var childPositionInfo = _finder.Find<Position>(x => x.Id == positionChildren.ChildPositionId)
                                           .Select(x => new
                                               {
                                                   SalesModel = x.SalesModel,
                                                   x.IsComposite,
                                                   x.PlatformId,
                                               })
                                           .Single();

            if (childPositionInfo.IsComposite)
            {
                throw new NotificationException(BLResources.CantAddCompositePosition);
            }

            var isSalesMethodNotMatched = _finder.Find<PositionChildren>(x => !x.IsDeleted &&
                                                                                   x.MasterPositionId == positionChildren.MasterPositionId &&
                                                                                   x.ChildPosition.SalesModel !=
                                                                                   childPositionInfo.SalesModel)
                                                      .Any();
            if (isSalesMethodNotMatched)
            {
                throw new NotificationException(BLResources.CantAddChildPositionWithDifferentSalesModel);
            }

            var masterPosition = _finder.Find(Specs.Find.ById<Position>(positionChildren.MasterPositionId)).Single();
            if (masterPosition.RestrictChildPositionPlatforms && masterPosition.PlatformId != childPositionInfo.PlatformId)
            {
                throw new NotificationException(BLResources.PositionViolatesPlatformRestriction);
            }

            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(positionChildren))
            {
                if (positionChildren.IsNew())
                {
                    _identityProvider.SetFor(positionChildren);
                    _positionChildrenGenericRepository.Add(positionChildren);
                    scope.Added(positionChildren);
                }
                else
                {
                    _positionChildrenGenericRepository.Update(positionChildren);
                    scope.Updated(positionChildren);
                }

                _positionChildrenGenericRepository.Save();

                scope.Complete();
            }
        }

        public void CreateOrUpdate(PositionCategory category)
        {
            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(category))
            {
                if (category.IsNew())
                {
                    _positionCategoryGenericRepository.Add(category);
                    scope.Added(category);
                }
                else
                {
                    _positionCategoryGenericRepository.Update(category);
                    scope.Updated(category);
                }

                _positionCategoryGenericRepository.Save();

                scope.Complete();
            }
        }

        public int Delete(Position position)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, Position>())
            {
                _positionGenericRepository.Delete(position);
                var cnt = _positionGenericRepository.Save();

                scope.Deleted(position)
                     .Complete();

                return cnt;
            }
        }

        private int Delete(PositionChildren link)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, PositionChildren>())
            {
                _positionChildrenGenericRepository.Delete(link);
                var cnt = _positionChildrenGenericRepository.Save();

                scope.Deleted(link)
                     .Complete();

                return cnt;
            }
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