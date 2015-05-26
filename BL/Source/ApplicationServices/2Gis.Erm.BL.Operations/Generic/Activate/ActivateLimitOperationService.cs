using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.Operations.Generic.Activate
{
    public class ActivateLimitOperationService : IActivateGenericEntityService<Limit>
    {
        public int Activate(long entityId)
        {
            throw new NotSupportedException(BLResources.OperationIsDisabled);
        }
    }
}