using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Complete
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.Complete201502)]
    public interface ICompleteApplicationRestService
    {
        [OperationContract(Name = "ExecuteRest")]
        [WebInvoke(Method = "POST", UriTemplate = "/{entityName}/{entityId}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(CompleteOperationErrorDescription))]
        void Execute(string entityName, string entityId);
    }
}
