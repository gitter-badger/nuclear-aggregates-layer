using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Boundaries;

using NuClear.Aggregates;

namespace DoubleGis.Erm.BLFlex.Operations.Global
{
    public interface IOrderPrintFormReadModel : IPrintingBoundedContext, IAggregateReadModel<Order>
    {
        OrderRelationsDto GetOrderRelationsDto(long orderId);
        decimal GetOrderDicount(long orderId);
        ContributionTypeEnum GetOrderContributionType(long organizationUnitId);

        IQueryable<Bill> GetBillQuery(long orderId);
        IQueryable<OrderPosition> GetOrderPositionQuery(long orderId);
        IQueryable<Order> GetOrderQuery(long orderId);
        IQueryable<FirmAddress> GetFirmAddressQuery(long orderId);
        IQueryable<BranchOffice> GetBranchOfficeQuery(long orderId);
        Bargain GetOrderBargain(long orderId);
    }
}