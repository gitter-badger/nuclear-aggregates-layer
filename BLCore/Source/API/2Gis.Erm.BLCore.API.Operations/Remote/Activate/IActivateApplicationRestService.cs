using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Activate
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.Activate201303)]
    public interface IActivateApplicationRestService
    {
         [OperationContract(Name = "ExecuteRest")]
         [WebInvoke(Method = "POST", UriTemplate = "/{entityName}/{entityId}", ResponseFormat = WebMessageFormat.Json)]
         [FaultContract(typeof(ActivateOperationErrorDescription))]
         void Execute(string entityName, string entityId);
    }
}