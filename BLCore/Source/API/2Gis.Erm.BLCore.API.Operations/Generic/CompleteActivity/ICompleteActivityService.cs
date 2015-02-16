﻿using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Cancel;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Complete
{
    public interface ICompleteService : IOperation<CompleteIdentity>
    {
        void Complete(long entityId);
    }
}
