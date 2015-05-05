using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Positions;
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

        public IEnumerable<LinkingObjectsSchemaPositionDto> GetPositionBindingObjectsInfo(bool isPricePositionComposite, long positionId)
        {
            var positions = _finder.Find(Specs.Find.ById<Position>(positionId));

            if (isPricePositionComposite)
            {
                positions = positions.SelectMany(x => x.ChildPositions)
                                     .Where(x => !x.IsDeleted)
                                     .Select(x => x.ChildPosition);
            }

            return positions.Select(x => new LinkingObjectsSchemaPositionDto
                                             {
                                                 Id = x.Id,
                                                 Name = x.Name,
                                                 BindingObjectType = x.BindingObjectTypeEnum,
                                                 AdvertisementTemplateId = x.AdvertisementTemplateId,
                                                 DummyAdvertisementId = x.AdvertisementTemplate.DummyAdvertisementId,
                                                 PositionsGroup = x.PositionsGroup
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

        public IReadOnlyDictionary<long, PositionBindingObjectType> GetPositionBindingObjectTypes(IEnumerable<long> positionIds)
        {
            return _finder.Find(Specs.Find.ByIds<Position>(positionIds))
                          .Select(x => new { x.Id, x.BindingObjectTypeEnum })
                          .ToDictionary(x => x.Id, y => y.BindingObjectTypeEnum);
        }

        public IReadOnlyDictionary<long, string> GetPositionNames(IEnumerable<long> positionIds)
        {
            return _finder.Find(Specs.Find.ByIds<Position>(positionIds))
                          .Select(x => new { x.Id, x.Name })
                          .ToDictionary(x => x.Id, y => y.Name);
        }

        public IEnumerable<PositionSortingOrderDto> GetPositionsSortingOrder()
        {
            return _finder.Find(PriceSpecs.Positions.Select.PositionSortingOrderDto(),
                                PriceSpecs.Positions.Find.WithSortingSpecified())
                          .ToArray();
        }

        public IEnumerable<Position> GetPositions(IEnumerable<long> ids)
        {
            return _finder.FindMany(Specs.Find.ByIds<Position>(ids));
        }

        public IReadOnlyCollection<long> GetChildPositionIds(long positionId)
        {
            return _finder.Find(Specs.Find.ById<Position>(positionId))
                          .SelectMany(x => x.ChildPositions)
                          .Where(Specs.Find.ActiveAndNotDeleted<PositionChildren>())
                          .Select(x => x.ChildPositionId)
                          .ToArray();
        }

        public bool KeepCategoriesSynced(long positionId)
        {
            return _finder.Find(Specs.Find.ById<Position>(positionId)).Any(x => x.IsComposite && x.DgppId != PositionTools.AdditionalPackageDgppId);
        }

        public bool AllSubpositionsMustBePicked(long positionId)
        {
            return _finder.Find(Specs.Find.ById<Position>(positionId)).Any(x => x.IsComposite && x.DgppId != PositionTools.AdditionalPackageDgppId);
        }

        public bool AutoCheckPositionsWithFirmBindingType(long positionId)
        {
            return _finder.Find(Specs.Find.ById<Position>(positionId)).Any(x => x.DgppId != PositionTools.AdditionalPackageDgppId);
        }

        public PositionSalesModelAndCompositenessDto GetPositionSalesModelAndCompositenessByPricePosition(long pricePositionId)
        {
            return
                _finder.Find(PriceSpecs.Positions.Find.ByPricePosition(pricePositionId) && Specs.Find.ActiveAndNotDeleted<Position>())
                       .Select(x => new PositionSalesModelAndCompositenessDto
                                        {
                                            PositionId = x.Id,
                                            IsComposite = x.IsComposite,
                                            SalesModel = x.SalesModel
                                        })
                       .SingleOrDefault();
        }

        public IDictionary<long, PositionsGroup> GetPositionGroups(IEnumerable<long> positionIds)
        {
            return _finder.Find(Specs.Find.ByIds<Position>(positionIds))
                          .Select(x => new
                                           {
                                               Id = x.Id,
                                               PositionsGroup = x.PositionsGroup
                                           })
                          .ToDictionary(x => x.Id, x => x.PositionsGroup);
        }

        public IReadOnlyDictionary<PlatformEnum, long> GetPlatformsDictionary(IEnumerable<long> platformDgppIds)
        {
            return _finder.Find<Platform.Model.Entities.Erm.Platform>(x => platformDgppIds.Contains(x.DgppId))
                                .ToDictionary(x => (PlatformEnum)x.DgppId, x => x.Id);
        }
    }
}