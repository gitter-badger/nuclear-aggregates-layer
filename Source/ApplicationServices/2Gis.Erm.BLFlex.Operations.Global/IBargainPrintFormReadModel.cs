using System.Linq;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Boundaries;

namespace DoubleGis.Erm.BLFlex.Operations.Global
{
    public interface IBargainPrintFormReadModel : IPrintingBoundedContext, IAggregateReadModel<Bargain>
    {
        BargainRelationsDto GetBargainRelationsDto(long bargainId);
        IQueryable<Bargain> GetBargainQuery(long bargainId);
        IQueryable<BranchOffice> GetBranchOfficeQuery(long bargainId);
    }
}