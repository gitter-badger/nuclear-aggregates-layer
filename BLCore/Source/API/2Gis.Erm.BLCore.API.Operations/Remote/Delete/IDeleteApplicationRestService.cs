using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Delete
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.Delete201303)]
    public interface IDeleteApplicationRestService
    {
        [OperationContract(Name = "ExecuteRest")]
        [WebInvoke(Method = "POST", UriTemplate = "/{entityName}/{entityId}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(DeleteOperationErrorDescription))]
        DeleteConfirmation Execute(string entityName, string entityId);
    }
}