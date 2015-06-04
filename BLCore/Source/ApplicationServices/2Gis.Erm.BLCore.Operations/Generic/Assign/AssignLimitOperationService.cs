using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignLimitOperationService : IAssignGenericEntityService<Limit>
    {
        public AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            throw new NotSupportedException();
        }
    }
}