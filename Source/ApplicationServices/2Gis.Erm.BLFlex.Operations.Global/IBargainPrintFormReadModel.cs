using System.Linq;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global
{
    public interface IBargainPrintFormReadModel : IAggregateReadModel<Bargain>
    {
        BargainRelationsDto GetBargainRelationsDto(long orderId);
        IQueryable<Bargain> GetBargainQuery(long orderId);
        IQueryable<BranchOffice> GetBranchOfficeQuery(long orderId);
        IQueryable<Order> GetOrderQuery(long orderId);
    }
}