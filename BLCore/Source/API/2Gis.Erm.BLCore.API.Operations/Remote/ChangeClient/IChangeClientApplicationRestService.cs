using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeClient;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.ChangeClient
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.ChangeClient201303)]
    public interface IChangeClientApplicationRestService
    {
        [OperationContract(Name = "ValidateRest")]
        [WebInvoke(Method = "POST", UriTemplate = "/Validate/{entityName}/{entityId}/{clientId}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ChangeClientOperationErrorDescription))]
        ChangeEntityClientValidationResult Validate(string entityName, string entityId, string clientId);

        [OperationContract(Name = "ExecuteRest")]
        [WebInvoke(Method = "POST", UriTemplate = "/{entityName}/{entityId}/{clientId}/{bypassValidation}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ChangeClientOperationErrorDescription))]
        ChangeEntityClientResult Execute(string entityName, string entityId, string clientId, string bypassValidation);
    }
}