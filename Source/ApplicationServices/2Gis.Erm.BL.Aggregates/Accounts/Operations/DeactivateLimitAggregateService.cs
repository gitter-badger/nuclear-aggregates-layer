using System;

using DoubleGis.Erm.BL.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.Aggregates.Accounts.Operations
{
    public sealed class DeactivateLimitAggregateService : IDeactivateLimitAggregateService
    {
        public void Deactivate(Limit limit)
        {
            throw new NotSupportedException(BLResources.OperationIsDiabled);
        }
    }
}
