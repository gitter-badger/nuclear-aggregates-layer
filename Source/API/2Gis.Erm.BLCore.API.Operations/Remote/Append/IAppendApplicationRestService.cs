using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Append
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.Append201303)]
    public interface IAppendApplicationRestService
    {
        [OperationContract(Name = "ExecuteRest")]
        [WebInvoke(Method = "POST", UriTemplate = "/{entityName}/{entityId}/{appendedEntityName}/{appendedEntityId}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(AppendOperationErrorDescription))]
        void Execute(string entityName, string entityId, string appendedEntityName, string appendedEntityId);
    }
}