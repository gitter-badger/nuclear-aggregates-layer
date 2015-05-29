using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Users
{
    public interface IDeactivateUserAggregateService : IAggregateSpecificService<User, DeactivateIdentity>
    {
        void Deactivate(User user, UserProfile profile, IEnumerable<UserRole> userRoleRelations);
    }
}