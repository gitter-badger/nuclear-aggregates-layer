using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Boundaries;

using NuClear.Aggregates;

namespace DoubleGis.Erm.BLFlex.Operations.Global
{
    public interface IBargainPrintFormReadModel : IPrintingBoundedContext, IAggregateReadModel<Bargain>
    {
        BargainRelationsDto GetBargainRelationsDto(long bargainId);
        IQueryable<Bargain> GetBargainQuery(long bargainId);
        IQueryable<BranchOffice> GetBranchOfficeQuery(long bargainId);
    }
}