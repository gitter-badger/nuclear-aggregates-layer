using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.ReadModel
{
    public sealed class PriceReadModel : IPriceReadModel
    {
        private const decimal DefaultCategoryRate = 1;
        private readonly IFinder _finder;

        public PriceReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public Price GetPrice(long priceId)
        {
            return _finder.Find(Specs.Find.ById<Price>(priceId)).One();
        }

        public PricePosition GetPricePosition(long pricePositionId)
        {
            return _finder.Find(Specs.Find.ById<PricePosition>(pricePositionId)).One();
        }

        public PricePositionRateType GetPricePositionRateType(long pricePositionId)
        {
            return _finder.FindObsolete(Specs.Find.ById<PricePosition>(pricePositionId)).Select(x => x.RateType).Single();
        }

        public decimal GetCategoryRateByFirm(long firmId)
        {
            var categoryQuery = _finder.FindObsolete(Specs.Find.ById<Firm>(firmId))
                                       .SelectMany(order => order.FirmAddresses)
                                       .Where(Specs.Find.ActiveAndNotDeleted<FirmAddress>())
                                       .SelectMany(address => address.CategoryFirmAddresses)
                                       .Where(Specs.Find.ActiveAndNotDeleted<CategoryFirmAddress>())
                                       .Select(addressCategory => addressCategory.Category)
                                       .Where(Specs.Find.ActiveAndNotDeleted<Category>());

            var orgUnitId = _finder.FindObsolete(Specs.Find.ById<Firm>(firmId)).Select(x => x.OrganizationUnitId).Single();

            return GetCategoryRateInternal(categoryQuery, orgUnitId);
        }

        // TODO {a.tukaev, 25.03.2014}: аргумент organizationUnitId лучше вычислять внутри этого метода получая от клиентского кода firmId, например также, как в методе GetCategoryRateByFirm
        public decimal GetCategoryRateByCategory(long[] categoryId, long organizationUnitId)
        {
            return GetCategoryRateInternal(_finder.FindObsolete(Specs.Find.ByIds<Category>(categoryId)), organizationUnitId);
        }

        public decimal GetPricePositionCost(long pricePositionId)
        {
            return _finder.FindObsolete(Specs.Find.ById<PricePosition>(pricePositionId)).Select(x => x.Cost).Single();
        }

        public PricePosition GetActivePricePosition(long priceId, long positionId)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<PricePosition>() &&
                                PriceSpecs.PricePositions.Find.ByPriceAndPosition(priceId, positionId))
                          .One();
        }

        public bool IsDifferentPriceExistsForDate(long priceId, long organizationUnitId, DateTime beginDate)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Price>() &&
                                new FindSpecification<Price>(x => x.Id != priceId && x.OrganizationUnitId == organizationUnitId && x.BeginDate == beginDate.Date))
                          .Any();
        }

        public PriceValidationDto GetPriceValidationDto(long priceId)
        {
            return _finder.FindObsolete(Specs.Find.ById<Price>(priceId), PriceSpecs.Prices.Select.PriceValidationDto()).Single();
        }

        public IsPricePublishedAndActiveDto IsPricePublishedAndActive(long priceId)
        {
            return _finder.FindObsolete(Specs.Find.ById<Price>(priceId))
                          .Select(x => new IsPricePublishedAndActiveDto
                                           {
                                               IsActive = x.IsActive,
                                               IsPublished = x.IsPublished
                                           }).Single();
        }

        public long GetActualPriceId(long organizationUnitId)
        {
            var now = DateTime.UtcNow;
            var startOfNextMonth = new DateTime(now.AddMonths(1).Year, now.AddMonths(1).Month, 1);

            return _finder.FindObsolete(Specs.Find.ById<OrganizationUnit>(organizationUnitId))
                          .SelectMany(x => x.Prices)
                          .Where(Specs.Find.ActiveAndNotDeleted<Price>())
                          .Where(x => x.IsPublished && x.BeginDate < startOfNextMonth)
                          .OrderByDescending(x => x.BeginDate)
                          .Select(x => x.Id)
                          .FirstOrDefault();
        }

        public bool IsPriceActive(long priceId)
        {
            return _finder.Find(Specs.Find.ById<Price>(priceId) && Specs.Find.ActiveAndNotDeleted<Price>()).Any();
        }

        public bool DoesPriceExist(long priceId)
        {
            return _finder.Find(Specs.Find.ById<Price>(priceId)).Any();
        }

        public bool IsPriceLinked(long priceId)
        {
            return _finder.Find(Specs.Find.ById<Price>(priceId) && PriceSpecs.Prices.Find.Linked()).Any();
        }

        public bool IsPricePublished(long priceId)
        {
            var nowDate = DateTime.UtcNow.Date;
            return _finder.Find(Specs.Find.ById<Price>(priceId) && Specs.Find.ActiveAndNotDeleted<Price>() &&
                                new FindSpecification<Price>(x => x.IsPublished && x.BeginDate <= nowDate))
                          .Any();
        }

        public bool DoesPriceContainPosition(long priceId, long positionId)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<PricePosition>() &&
                                PriceSpecs.PricePositions.Find.ByPriceAndPosition(priceId, positionId))
                          .Any();
        }

        public bool DoesPriceContainPositionWithinNonDeleted(long priceId, long positionId)
        {
            return _finder.Find(Specs.Find.NotDeleted<PricePosition>() &&
                                PriceSpecs.PricePositions.Find.ByPriceAndPosition(priceId, positionId))
                          .Any();
        }

        public bool DoesPricePositionExist(long priceId, long positionId, long pricePositionId)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<PricePosition>() &&
                                PriceSpecs.PricePositions.Find.ByPriceAndPositionButAnother(priceId, positionId, pricePositionId))
                          .Any();
        }

        public bool DoesPricePositionExistWithinNonDeleted(long priceId, long positionId, long pricePositionId)
        {
            return _finder.Find(Specs.Find.NotDeleted<PricePosition>() &&
                                PriceSpecs.PricePositions.Find.ByPriceAndPositionButAnother(priceId, positionId, pricePositionId))
                          .Any();
        }

        public AllPriceDescendantsDto GetAllPriceDescendantsDto(long priceId)
        {
            var dto = _finder.FindObsolete(Specs.Find.ById<Price>(priceId))
                             .Select(x => new
                                 {
                                     x.PricePositions,
                                     AssociatedPositionsGroupsMapping = x.PricePositions
                                                                         .Select(y => new
                                                                             {
                                                                                 PricePositionId = y.Id,
                                                                                 y.AssociatedPositionsGroups
                                                                             }),
                                     AssociatedPositionsMapping = x.PricePositions
                                                                   .SelectMany(y => y.AssociatedPositionsGroups)
                                                                   .Select(y => new
                                                                       {
                                                                           AssociatedPositionsGroupId = y.Id,
                                                                           y.AssociatedPositions
                                                                       }),
                                     x.DeniedPositions
                                 })
                             .Single();

            return new AllPriceDescendantsDto
                {
                    PricePositions = dto.PricePositions,
                    AssociatedPositionsGroupsMapping = dto.AssociatedPositionsGroupsMapping
                                                          .ToDictionary(x => x.PricePositionId,
                                                                        x => x.AssociatedPositionsGroups.AsEnumerable()),
                    AssociatedPositionsMapping = dto.AssociatedPositionsMapping
                                                    .ToDictionary(x => x.AssociatedPositionsGroupId, x => x.AssociatedPositions.AsEnumerable()),
                    DeniedPositions = dto.DeniedPositions
                };
        }

        public AllPricePositionDescendantsDto GetAllPricePositionDescendantsDto(long pricePositionId, long positionId)
        {
            var dto = (from pricePosition in _finder.FindObsolete(Specs.Find.ById<PricePosition>(pricePositionId))
                       let deniedPositions = pricePosition.Price.DeniedPositions
                       select new
                           {
                               pricePosition.AssociatedPositionsGroups,
                               AssociatedPositionsMapping = pricePosition.AssociatedPositionsGroups
                                                                         .Select(y => new
                                                                             {
                                                                                 AssociatedPositionsGroupId = y.Id,
                                                                                 y.AssociatedPositions
                                                                             }),
                                      DeniedPositions = deniedPositions.Where(x => x.PositionId == positionId || x.PositionDeniedId == positionId)
                           })
                .Single();

            return new AllPricePositionDescendantsDto
                {
                    AssociatedPositionsGroups = dto.AssociatedPositionsGroups,
                    AssociatedPositionsMapping = dto.AssociatedPositionsMapping.ToDictionary(x => x.AssociatedPositionsGroupId,
                                                                                             x => x.AssociatedPositions.AsEnumerable()),
                    DeniedPositions = dto.DeniedPositions
                };
        }

        public PriceDto GetPriceDto(long priceId)
        {
            return _finder.FindObsolete(Specs.Find.ById<Price>(priceId))
                          .Select(x => new PriceDto
                              {
                                  BeginDate = x.BeginDate,
                                  CreateDate = x.CreateDate,
                                  OrganizationUnitId = x.OrganizationUnitId,
                                  PublishDate = x.PublishDate
                              })
                          .Single();
        }

        public long GetPriceId(long pricePositionId)
        {
            return _finder.FindObsolete(Specs.Find.ById<PricePosition>(pricePositionId))
                         .Select(x => x.PriceId)
                         .Single();
        }

        public PricePositionDetailedInfo GetPricePositionDetailedInfo(long pricePositionId)
        {
            var pricePositionInfo = _finder.FindObsolete(Specs.Find.ById<PricePosition>(pricePositionId))
                                           .Select(item => new
                                           {
                                               item.PositionId,
                                               Platform = item.Position.Platform.Name,
                                               item.RateType,
                                               item.Amount,
                                               item.AmountSpecificationMode,
                                               PricePositionCost = item.Cost,
                                               item.Position.SalesModel,
                                               item.Position.IsComposite,
                                               BindingObjectType = item.Position.BindingObjectTypeEnum
                                           })
                                           .Single();

            return new PricePositionDetailedInfo
            {
                           PositionId = pricePositionInfo.PositionId,
                Amount = pricePositionInfo.Amount,
                AmountSpecificationMode = (int)pricePositionInfo.AmountSpecificationMode,
                Platform = pricePositionInfo.Platform ?? string.Empty,
                PricePositionCost = pricePositionInfo.PricePositionCost,
                RateType = pricePositionInfo.RateType,
                SalesModel = pricePositionInfo.SalesModel,
                           BindingObjectType = pricePositionInfo.BindingObjectType,
                           IsComposite = pricePositionInfo.IsComposite
            };
        }

        public DeniedPosition GetDeniedPosition(long deniedPositionId)
        {
            return _finder.Find(Specs.Find.ById<DeniedPosition>(deniedPositionId)).One();
        }

        public IReadOnlyCollection<DeniedPosition> GetDeniedPositions(long positionId, long priceId)
        {
            return
                _finder.Find(Specs.Find.ActiveAndNotDeleted<DeniedPosition>() &&
                                 PriceSpecs.DeniedPositions.Find.ByPrice(priceId) &&
                                 PriceSpecs.DeniedPositions.Find.ByPosition(positionId))
                       .Many();
        }

        public IReadOnlyCollection<DeniedPosition> GetDeniedPositions(long positionId, long positionDeniedId, long priceId)
        {
            return
                _finder.Find(Specs.Find.ActiveAndNotDeleted<DeniedPosition>() &&
                                 PriceSpecs.DeniedPositions.Find.ByPrice(priceId) &&
                                 PriceSpecs.DeniedPositions.Find.ByPositions(positionId, positionDeniedId))
                       .Many();
        }

        public IReadOnlyCollection<DeniedPosition> GetDeniedPositions(long positionId, long positionDeniedId, long priceId, ObjectBindingType objectBindingType)
        {
            return
                _finder.Find(Specs.Find.ActiveAndNotDeleted<DeniedPosition>() &&
                                 PriceSpecs.DeniedPositions.Find.ByPrice(priceId) &&
                                 PriceSpecs.DeniedPositions.Find.ByPositions(positionId, positionDeniedId) &&
                                 PriceSpecs.DeniedPositions.Find.ByObjectBindingType(objectBindingType))
                       .Many();
        }

        public IReadOnlyCollection<DeniedPosition> GetInactiveDeniedPositions(long positionId, long positionDeniedId, long priceId, ObjectBindingType objectBindingType)
        {
            return
                _finder.Find(Specs.Find.InactiveAndNotDeletedEntities<DeniedPosition>() &&
                                 PriceSpecs.DeniedPositions.Find.ByPrice(priceId) &&
                                 PriceSpecs.DeniedPositions.Find.ByPositions(positionId, positionDeniedId) &&
                                 PriceSpecs.DeniedPositions.Find.ByObjectBindingType(objectBindingType))
                       .Many();
        }

        public IReadOnlyCollection<DeniedPosition> GetDeniedPositionsOrSymmetric(long positionId, long priceId)
        {
            return
                _finder.Find(Specs.Find.ActiveAndNotDeleted<DeniedPosition>() &&
                                 PriceSpecs.DeniedPositions.Find.ByPrice(priceId) &&
                                 (PriceSpecs.DeniedPositions.Find.ByPosition(positionId) ||
                                  PriceSpecs.DeniedPositions.Find.ByPositionDenied(positionId)))
                       .Many();
        }

        public IReadOnlyCollection<DeniedPosition> GetDeniedPositionsOrSymmetric(long positionId, long positionDeniedId, long priceId, params long[] deniedPositionToExcludeIds)
        {
            return
                _finder.Find(Specs.Find.ExceptByIds<DeniedPosition>(deniedPositionToExcludeIds) &&
                                 Specs.Find.ActiveAndNotDeleted<DeniedPosition>() &&
                                 PriceSpecs.DeniedPositions.Find.ByPrice(priceId) &&
                                 (PriceSpecs.DeniedPositions.Find.ByPositions(positionId, positionDeniedId) ||
                                  PriceSpecs.DeniedPositions.Find.ByPositions(positionDeniedId, positionId)))
                       .Many();
        }

        private static decimal GetCategoryRateInternal(IQueryable<Category> categoryQuery, long organizationUnitId)
        {
            var categoryRate = categoryQuery.SelectMany(category => category.CategoryOrganizationUnits)
                                            .Where(Specs.Find.ActiveAndNotDeleted<CategoryOrganizationUnit>())
                                            .Where(categoryOrganizationUnit => categoryOrganizationUnit.OrganizationUnitId == organizationUnitId)
                                            .Select(categoryOrganizationUnit => (decimal?)(categoryOrganizationUnit.CategoryGroup != null
                                                                                               ? categoryOrganizationUnit.CategoryGroup.GroupRate
                                                                                               : DefaultCategoryRate))
                                            .Max();

            if (categoryRate == null)
            {
                throw new BusinessLogicException(BLResources.PricePositionCannotBeChoosedSinceThereIsNoFirmCategory);
            }

            return categoryRate.Value;
        }
    }
}