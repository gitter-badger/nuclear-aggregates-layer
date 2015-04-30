using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using NuClear.Storage;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

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
            return _finder.FindOne(Specs.Find.ById<Price>(priceId));
        }

        public PricePosition GetPricePosition(long pricePositionId)
        {
            return _finder.FindOne(Specs.Find.ById<PricePosition>(pricePositionId));
        }

        public PricePositionRateType GetPricePositionRateType(long pricePositionId)
        {
            return _finder.Find(Specs.Find.ById<PricePosition>(pricePositionId)).Select(x => x.RateType).Single();
        }

        public decimal GetCategoryRateByFirm(long firmId)
        {
            var categoryQuery = _finder.Find(Specs.Find.ById<Firm>(firmId))
                                       .SelectMany(order => order.FirmAddresses)
                                       .Where(Specs.Find.ActiveAndNotDeleted<FirmAddress>())
                                       .SelectMany(address => address.CategoryFirmAddresses)
                                       .Where(Specs.Find.ActiveAndNotDeleted<CategoryFirmAddress>())
                                       .Select(addressCategory => addressCategory.Category)
                                       .Where(Specs.Find.ActiveAndNotDeleted<Category>());

            var orgUnitId = _finder.Find(Specs.Find.ById<Firm>(firmId)).Select(x => x.OrganizationUnitId).Single();

            return GetCategoryRateInternal(categoryQuery, orgUnitId);
        }

        // TODO {a.tukaev, 25.03.2014}: аргумент organizationUnitId лучше вычислять внутри этого метода получая от клиентского кода firmId, например также, как в методе GetCategoryRateByFirm
        public decimal GetCategoryRateByCategory(long[] categoryId, long organizationUnitId)
        {
            return GetCategoryRateInternal(_finder.Find(Specs.Find.ByIds<Category>(categoryId)), organizationUnitId);
        }

        public decimal GetPricePositionCost(long pricePositionId)
        {
            return _finder.Find(Specs.Find.ById<PricePosition>(pricePositionId)).Select(x => x.Cost).Single();
        }

        public PricePosition GetPricePosition(long priceId, long positionId)
        {
            return _finder.FindOne(PriceSpecs.PricePositions.Find.ByPriceAndPosition(priceId, positionId));
        }

        public bool IsDifferentPriceExistsForDate(long priceId, long organizationUnitId, DateTime beginDate)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Price>())
                          .Any(x => x.Id != priceId && x.OrganizationUnitId == organizationUnitId && x.BeginDate == beginDate.Date);
        }

        public PriceValidationDto GetPriceValidationDto(long priceId)
        {
            return _finder.Find<Price, PriceValidationDto>(PriceSpecs.Prices.Select.PriceValidationDto(), Specs.Find.ById<Price>(priceId)).Single();
        }

        public long GetActualPriceId(long organizationUnitId)
        {
            var now = DateTime.UtcNow;
            var startOfNextMonth = new DateTime(now.AddMonths(1).Year, now.AddMonths(1).Month, 1);

            return _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId))
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

        public bool IsPriceExist(long priceId)
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
            return _finder.Find(Specs.Find.ById<Price>(priceId) && Specs.Find.ActiveAndNotDeleted<Price>()).Any(x => x.IsPublished && x.BeginDate <= nowDate);
        }

        public bool IsPriceContainsPosition(long priceId, long positionId)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<PricePosition>())
                          .Where(PriceSpecs.PricePositions.Find.ByPriceAndPosition(priceId, positionId))
                          .Any();
        }

        public bool IsPriceContainsPositionWithinNonDeleted(long priceId, long positionId)
        {
            return _finder.Find(Specs.Find.NotDeleted<PricePosition>())
                          .Where(PriceSpecs.PricePositions.Find.ByPriceAndPosition(priceId, positionId))
                          .Any();
        }

        public bool IsPricePositionExist(long priceId, long positionId, long pricePositionId)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<PricePosition>() &&
                                PriceSpecs.PricePositions.Find.ByPriceAndPositionButAnother(priceId, positionId, pricePositionId))
                          .Any();
        }

        public bool IsPricePositionExistWithinNonDeleted(long priceId, long positionId, long pricePositionId)
        {
            return _finder.Find(Specs.Find.NotDeleted<PricePosition>() &&
                                PriceSpecs.PricePositions.Find.ByPriceAndPositionButAnother(priceId, positionId, pricePositionId))
                          .Any();
        }

        public AllPriceDescendantsDto GetAllPriceDescendantsDto(long priceId)
        {
            var dto = _finder.Find(Specs.Find.ById<Price>(priceId))
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
            var dto = (from pricePosition in _finder.Find(Specs.Find.ById<PricePosition>(pricePositionId))
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
                               DeniedPositions = deniedPositions.Where(x => x.PositionId == positionId && x.PositionId == x.PositionDeniedId)
                                                                .Concat(deniedPositions
                                                                            .Where(x => x.PositionId == positionId && x.PositionId != x.PositionDeniedId))
                                                                .Concat(deniedPositions
                                                                            .Where(x => x.PositionDeniedId == positionId && x.PositionId != x.PositionDeniedId))
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

        public long GetPriceId(long pricePositionId)
        {
            return _finder.Find(Specs.Find.ById<PricePosition>(pricePositionId))
                         .Select(x => x.PriceId)
                         .Single();
        }

        public PricePositionDetailedInfo GetPricePositionDetailedInfo(long pricePositionId)
        {
            var pricePositionInfo = _finder.Find(Specs.Find.ById<PricePosition>(pricePositionId))
                                           .Select(item => new
                                           {
                                               Platform = item.Position.Platform.Name,
                                               item.RateType,
                                               item.Amount,
                                               item.AmountSpecificationMode,
                                               item.PositionId,
                                               PricePositionCost = item.Cost,
                                               item.Position.IsComposite,
                                               LinkingObjectType = item.Position.BindingObjectTypeEnum,
                                               SalesModel = item.Position.SalesModel
                                           })
                                           .Single();

            return new PricePositionDetailedInfo
            {
                Amount = pricePositionInfo.Amount,
                AmountSpecificationMode = (int)pricePositionInfo.AmountSpecificationMode,
                IsComposite = pricePositionInfo.IsComposite,
                Platform = pricePositionInfo.Platform ?? string.Empty,
                PricePositionCost = pricePositionInfo.PricePositionCost,
                RateType = pricePositionInfo.RateType,

                LinkingObjectType = pricePositionInfo.LinkingObjectType,
                SalesModel = pricePositionInfo.SalesModel,
                PositionId = pricePositionInfo.PositionId,
            };
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