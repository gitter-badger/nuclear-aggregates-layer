using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.ActionHistory;
using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.ActionsHistory
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.ActionsHistory201303)]
    public interface IActionsHistoryApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(ActionsHistoryOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.ActionsHistory201303)]
        ActionsHistoryDto GetActionsHistory(IEntityType entityName, long entityId);
    }
}