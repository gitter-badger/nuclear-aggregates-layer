using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel
{
    public interface IOrganizationUnitReadModel : IAggregateReadModel<OrganizationUnit>
    {
        string GetName(long organizationUnitId);
        long GetCurrencyId(long organizationUnitId);
    }
}