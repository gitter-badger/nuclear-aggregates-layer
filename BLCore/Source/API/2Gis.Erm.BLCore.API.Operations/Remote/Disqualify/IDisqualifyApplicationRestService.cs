using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Disqualify
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.Disqualify201303)]
    public interface IDisqualifyApplicationRestService
    {
        [OperationContract(Name = "ExecuteRest")]
        [WebInvoke(Method = "POST", UriTemplate = "/{entityName}/{entityId}/{bypassValidation}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(DisqualifyOperationErrorDescription))]
        DisqualifyResult Execute(string entityName, string entityId, string bypassValidation);
    }
}