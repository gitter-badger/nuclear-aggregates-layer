using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Limit;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.Limits
{
    public interface ISetLimitStatusOperationService : IOperation<SetLimitStatusIdentity>
    {
        void SetStatus(long limitId, LimitStatus status, params Guid[] limitReplicationCodes);
    }
}