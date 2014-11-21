using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations.Generic.ActionHistory;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.ActionsHistory
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.ActionsHistory201303)]
    public interface IActionsHistoryApplicationRestService
    {
        [OperationContract(Name = "GetActionHistoryRest")]
        [WebGet(UriTemplate = "/{entityName}/{entityId}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ActionsHistoryOperationErrorDescription))]
        ActionsHistoryDto GetActionsHistory(string entityName, string entityId);
    }
}