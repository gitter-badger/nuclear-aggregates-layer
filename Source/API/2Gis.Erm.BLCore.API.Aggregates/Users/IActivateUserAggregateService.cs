using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Users
{
    public interface IActivateUserAggregateService : IAggregateSpecificOperation<User, ActivateIdentity>
    {
        void Activate(User user, UserProfile profile);
    }
}