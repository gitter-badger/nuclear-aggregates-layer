﻿using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Assign
{
    public interface IAssignEntityService : IOperation<AssignIdentity>
    {
        AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign);
    }
}
