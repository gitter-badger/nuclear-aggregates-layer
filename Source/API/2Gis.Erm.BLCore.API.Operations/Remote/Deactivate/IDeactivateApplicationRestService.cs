using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Deactivate
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.Deactivate201303)]
    public interface IDeactivateApplicationRestService
    {
        [OperationContract(Name = "ExecuteRest")]
        [WebInvoke(Method = "POST", UriTemplate = "/{entityName}/{entityId}/{ownerCode}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(DeactivateOperationErrorDescription))]
        DeactivateConfirmation Execute(string entityName, string entityId, string ownerCode);
    }
}