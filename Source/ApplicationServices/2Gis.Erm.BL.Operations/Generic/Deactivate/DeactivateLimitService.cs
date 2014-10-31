using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    // TODO {y.baranihin, 31.10.2014}: перенести в BL
    public class DeactivateLimitService : IDeactivateGenericEntityService<Limit>
    {
        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            throw new NotSupportedException(BLResources.OperationIsDiabled);
        }
    }
}