using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Assign
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.Assign201303)]
    public interface IAssignApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(AssignOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Assign201303)]
        AssignResult Execute(IEntityType entityName, long entityId, long? ownerCode, bool? isPartialAssign, bool? bypassValidation); 
    }
}