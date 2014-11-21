using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.ChangeTerritory
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.ChangeTerritory201303)]
    public interface IChangeTerritoryApplicationRestService
    {
        [OperationContract(Name = "ExecuteRest")]
        [WebInvoke(Method = "POST", UriTemplate = "/{entityName}/{entityId}/{territoryId}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ChangeTerritoryOperationErrorDescription))]
        void Execute(string entityName, string entityId, string territoryId);
    }
}