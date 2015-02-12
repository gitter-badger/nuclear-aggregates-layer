using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations.Generic.CancelActivity;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.CancelActivity
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.CancelActivity201502)]
    public interface ICancelActivityApplicationRestService
    {
        [OperationContract(Name = "ExecuteRest")]
        [WebInvoke(Method = "POST", UriTemplate = "/{entityName}/{entityId}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(CancelActivityOperationErrorDescription))]
        CancelActivityResult Execute(string entityName, string entityId);
    }
}
