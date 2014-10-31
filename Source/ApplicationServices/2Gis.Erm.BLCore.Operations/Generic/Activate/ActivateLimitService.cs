using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Activate
{
    // TODO {y.baranihin, 31.10.2014}: перенести в BL
    public class ActivateLimitService : IActivateGenericEntityService<Limit>
    {
        public int Activate(long entityId)
        {
            throw new NotSupportedException(BLResources.OperationIsDiabled);
        }
    }
}