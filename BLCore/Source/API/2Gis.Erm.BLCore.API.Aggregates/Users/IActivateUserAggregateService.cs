using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Users
{
    public interface IActivateUserAggregateService : IAggregateSpecificOperation<User, ActivateIdentity>
    {
        void Activate(User user, UserProfile profile);
    }
}