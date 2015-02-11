using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.ChangeActivityStatus
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.ChangeActivityStatus201302)]
    public interface IChangeActivityStatusApplicationRestService
    {
        [OperationContract(Name = "ExecuteRest")]
        [WebInvoke(Method = "POST", UriTemplate = "/{entityName}/{entityId}/{status}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ChangeActivityStatusOperationErrorDescription))]
        void Execute(string entityName, string entityId, string status);
    }
}
