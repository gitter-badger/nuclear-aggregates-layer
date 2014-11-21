using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Assign
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.Assign201303)]
    public interface IAssignApplicationRestService
    {
        [OperationContract(Name = "ExecuteRest")]
        [WebInvoke(Method = "POST", UriTemplate = "/{entityName}/{entityId}/{ownerCode}/{isPartialAssign}/{bypassValidation}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(AssignOperationErrorDescription))]
        AssignResult Execute(string entityName, string entityId, string ownerCode, string isPartialAssign, string bypassValidation);
    }
}