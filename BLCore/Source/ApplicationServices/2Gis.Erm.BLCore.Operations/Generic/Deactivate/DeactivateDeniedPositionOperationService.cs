using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public class DeactivateDeniedPositionOperationService : IDeactivateGenericEntityService<DeniedPosition>
    {
        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            throw new NotSupportedException(BLResources.OperationIsDisabled);
        }
    }
}