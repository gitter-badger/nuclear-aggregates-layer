using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel
{
    public interface IPositionReadModel : IAggregateReadModel<Position>
    {
        PositionBindingObjectType GetPositionBindingObjectType(long positionId);
        bool IsSupportedByExport(long positionId);
        bool PositionsExist(IReadOnlyCollection<long> positionIds, out string message);

        LinkingObjectsSchemaDto GetLinkingObjectsSchema(OrderLinkingObjectsDto dto, PricePositionDetailedInfo pricePositionInfo, bool includeHiddenAddresses, long? orderPositionId);
        bool IsNewSalesModel(PositionAccountingMethod accountingMethod);
        IDictionary<long, string> GetNewSalesModelDeniedCategories(PositionAccountingMethod accountingMethod, long destOrganizationUnitId, IEnumerable<long> categoryIds);
    }
}