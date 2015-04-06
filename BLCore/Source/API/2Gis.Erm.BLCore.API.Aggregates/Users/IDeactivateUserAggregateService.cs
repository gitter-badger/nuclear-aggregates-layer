using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Users
{
    public interface IDeactivateUserAggregateService : IAggregateSpecificOperation<User, DeactivateIdentity>
    {
        void Deactivate(User user, UserProfile profile, IEnumerable<UserRole> userRoleRelations);
    }
}