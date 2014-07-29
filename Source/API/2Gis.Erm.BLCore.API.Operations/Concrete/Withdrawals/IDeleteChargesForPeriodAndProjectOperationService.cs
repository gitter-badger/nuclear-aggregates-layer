using System;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Charge;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals
{
    public interface IDeleteChargesForPeriodAndProjectOperationService : IOperation<DeleteChargesForPeriodAndProjectIdentity>
    {
        void Delete(long projectId, TimePeriod timePeriod, Guid sessionId);
    }
}