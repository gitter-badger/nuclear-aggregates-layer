using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.Operation
{
    public interface IDeleteUserBranchOfficesAggregateService : IAggregateSpecificService<BranchOffice, DeleteIdentity>
    {
        void Delete(IEnumerable<UserBranchOffice> userBranchOffices);
    }
}