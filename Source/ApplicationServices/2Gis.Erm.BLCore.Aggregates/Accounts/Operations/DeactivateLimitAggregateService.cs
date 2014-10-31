using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    // TODO {y.baranihin, 31.10.2014}: перенести в BL
    public sealed class DeactivateLimitAggregateService : IDeactivateLimitAggregateService
    {
        public void Deactivate(Limit limit)
        {
            throw new NotSupportedException(BLResources.OperationIsDiabled);
        }
    }
}
