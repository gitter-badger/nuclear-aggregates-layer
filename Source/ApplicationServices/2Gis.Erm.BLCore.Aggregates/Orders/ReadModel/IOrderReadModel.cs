using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO.ForRelease;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel
{
    public interface IOrderReadModel : IAggregateReadModel<Order>
    {
        IEnumerable<OrderReleaseInfo> GetOrderReleaseInfos(long organizationUnitId, TimePeriod period);
        IEnumerable<Order> GetOrdersForRelease(long organizationUnitId, TimePeriod period);
        OrderValidationAdditionalInfo[] GetOrderValidationAdditionalInfos(IEnumerable<long> orderIds);
        IEnumerable<OrderInfo> GetOrderInfosForRelease(long organizationUnitId, TimePeriod period, int skipCount, int takeCount);
        IEnumerable<Order> GetOrdersCompletelyReleasedBySourceOrganizationUnit(long sourceOrganizationUnitId);
    }
}
