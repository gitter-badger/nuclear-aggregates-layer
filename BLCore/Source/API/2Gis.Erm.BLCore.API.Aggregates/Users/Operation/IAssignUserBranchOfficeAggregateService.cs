using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Users.Operation
{
    public interface IAssignUserBranchOfficeAggregateService : IAggregateSpecificOperation<User, AssignIdentity>
    {
        void Assign(IEnumerable<UserBranchOffice> userBranchOffices, long ownerCode);
    }
}
