using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.Operation
{
    public interface IDeleteUserBranchOfficesAggregateService : IAggregateSpecificOperation<BranchOffice, DeleteIdentity>
    {
        void Delete(IEnumerable<UserBranchOffice> userBranchOffices);
    }
}