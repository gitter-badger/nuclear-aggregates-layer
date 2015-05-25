using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.Operations.Generic.Deactivate
{
    public class DeactivateLimitOperationService : IDeactivateGenericEntityService<Limit>
    {
        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            throw new NotSupportedException(BLResources.OperationIsDisabled);
        }
    }
}