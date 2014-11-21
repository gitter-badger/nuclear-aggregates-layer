using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Assign
{
    [ServiceContract(
        SessionMode = SessionMode.Required, 
        Namespace = ServiceNamespaces.BasicOperations.Assign201303,
        CallbackContract = typeof(IOperationProgressCallback))]
    public interface IGroupAssignApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(AssignOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Assign201303)]
        AssignResult[] Assign(AssignCommonParameter operationParameter, AssignEntityParameter[] operationItemParameters);
    }
}