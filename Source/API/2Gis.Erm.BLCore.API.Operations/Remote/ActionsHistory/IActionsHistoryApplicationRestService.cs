using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations.Generic.ActionHistory;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.ActionsHistory
{
    // TODO {d.ivanov, 05.03.2013}: Мне кажется для rest api xml namespace указывать не надо, т.к. в rest нет никакого xml
    // DONE {m.pashuk, 27.03.2013}: Согласен
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.ActionsHistory201303)]
    public interface IActionsHistoryApplicationRestService
    {
        [OperationContract(Name = "GetActionHistoryRest")]
        [WebGet(UriTemplate = "/{entityName}/{entityId}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ActionsHistoryOperationErrorDescription))]
        ActionsHistoryDto GetActionsHistory(string entityName, string entityId);
    }
}