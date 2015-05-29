using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Activate
{
    public class ActivateDeniedPositionOperationService : IActivateGenericEntityService<DeniedPosition>
    {
        public int Activate(long entityId)
        {
            throw new NotSupportedException(BLResources.OperationIsDisabled);
        }
    }
}