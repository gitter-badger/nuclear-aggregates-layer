using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Qualify
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.Qualify201303)]
    public interface IQualifyApplicationRestService
    {
        [OperationContract(Name = "ExecuteRest")]
        [WebInvoke(Method = "POST", UriTemplate = "/{entityName}/{entityId}/{ownerCode}/{relatedEntityId}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(QualifyOperationErrorDescription))]
        long? Execute(string entityName, string entityId, string ownerCode, string relatedEntityId);
    }
}