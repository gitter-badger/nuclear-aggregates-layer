using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Roles.ReadModel
{
    public interface IRoleReadModel : IAggregateReadModel<Role>
    {
        Role GetRole(long roleId);
    }
}
