using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Positions.ReadModel
{
    public sealed class PositionReadModel : IPositionReadModel
    {
        private readonly IFinder _finder;

        public PositionReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public PositionBindingObjectType GetPositionBindingObjectType(long positionId)
        {
            return _finder.Find(Specs.Find.ById<Position>(positionId)).Select(x => x.BindingObjectTypeEnum).Single();
        }

        public bool IsSupportedByExport(long positionId)
        {
            return _finder.Find(Specs.Find.ById<Position>(positionId)).Select(x => x.Platform.IsSupportedByExport).SingleOrDefault();
        }

        public bool PositionsExist(IReadOnlyCollection<long> positionIds, out string message)
        {
            var existingPositionIds = _finder.Find(Specs.Find.ActiveAndNotDeleted<Position>() &&
                                                   Specs.Find.ByIds<Position>(positionIds))
                                             .Select(x => x.Id)
                                             .ToArray();

            var missingPositionIds = positionIds.Except(existingPositionIds).ToArray();
            if (missingPositionIds.Any())
            {
                message = string.Format("The following positions could not be found: {0}.", string.Join(", ", missingPositionIds));
                return false;
            }

            message = null;
            return true;
        }

        public Position GetPositionByPricePositionId(long pricePositionId)
        {
            return _finder.FindOne(PositionSpecs.Find.ByPricePosition(pricePositionId) && Specs.Find.ActiveAndNotDeleted<Position>());
        }

        public IEnumerable<LinkingObjectsSchemaDto.PositionDto> GetPositionBindingObjectsInfo(bool isPricePositionComposite, long positionId)
        {
            var positions = _finder.Find(Specs.Find.ById<Position>(positionId));

            if (isPricePositionComposite)
            {
                positions = positions.SelectMany(x => x.ChildPositions)
                                     .Where(x => !x.IsDeleted)
                                     .Select(x => x.ChildPosition);
            }

            return positions.Select(x => new
                                             {
                                                 x.Id,
                                                 x.Name,
                                                 x.BindingObjectTypeEnum,
                                                 x.AdvertisementTemplateId,
                                                 x.AdvertisementTemplate.DummyAdvertisementId,
                                                 x.PositionsGroup
                                             })
                            .ToArray()
                            .Select(x => new LinkingObjectsSchemaDto.PositionDto
                                             {
                                                 Id = x.Id,
                                                 Name = x.Name,
                                                 LinkingObjectType = x.BindingObjectTypeEnum.ToString(),
                                                 AdvertisementTemplateId = x.AdvertisementTemplateId,
                                                 DummyAdvertisementId = x.DummyAdvertisementId,
                                                 IsLinkingObjectOfSingleType = IsPositionBindingOfSingleType(x.BindingObjectTypeEnum),
                                                 PositionsGroup = (int)x.PositionsGroup
                                             })
                            .ToArray();
        }

        public IReadOnlyCollection<long> GetDependedByPositionOrderIds(long positionId)
        {
            var childPositionIds = _finder.Find(Specs.Find.ById<Position>(positionId))
                                          .SelectMany(x => x.ChildPositions)
                                          .Select(x => x.ChildPositionId)
                                          .ToArray();

            return _finder.Find(Specs.Find.ByIds<Position>(childPositionIds.With(positionId)))
                          .SelectMany(x => x.OrderPositionAdvertisements)
                          .Select(x => x.OrderPosition)
                          .Where(Specs.Find.ActiveAndNotDeleted<OrderPosition>())
                          .Select(x => x.Order)
                          .Where(Specs.Find.ActiveAndNotDeleted<Order>() &&
                                 OrderSpecs.Orders.Find.WithStatuses(OrderState.Approved,
                                                                     OrderState.OnApproval,
                                                                     OrderState.OnRegistration,
                                                                     OrderState.OnTermination))
                          .Select(x => x.Id)
                          .ToArray();
        }

        private static bool IsPositionBindingOfSingleType(PositionBindingObjectType type)
        {
            switch (type)
            {
                case PositionBindingObjectType.Firm:
                case PositionBindingObjectType.AddressCategorySingle:
                case PositionBindingObjectType.AddressSingle:
                case PositionBindingObjectType.CategorySingle:
                case PositionBindingObjectType.AddressFirstLevelCategorySingle:
                    return true;
                case PositionBindingObjectType.AddressMultiple:
                case PositionBindingObjectType.CategoryMultiple:
                case PositionBindingObjectType.CategoryMultipleAsterix:
                case PositionBindingObjectType.AddressCategoryMultiple:
                case PositionBindingObjectType.AddressFirstLevelCategoryMultiple:
                case PositionBindingObjectType.ThemeMultiple:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        public IReadOnlyDictionary<PlatformEnum, long> GetPlatformsDictionary(IEnumerable<long> platformDgppIds)
        {
            return _finder.Find<Platform.Model.Entities.Erm.Platform>(x => platformDgppIds.Contains(x.DgppId))
                                .ToDictionary(x => (PlatformEnum)x.DgppId, x => x.Id);
        }

        public string GetPositionName(long positionId)
        {
            return _finder.Find(Specs.Find.ById<Position>(positionId))
                          .Select(item => item.Name)
                          .Single();
        }
    }
}