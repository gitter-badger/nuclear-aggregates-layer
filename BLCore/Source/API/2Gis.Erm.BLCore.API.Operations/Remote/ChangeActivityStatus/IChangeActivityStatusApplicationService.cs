using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.ChangeActivityStatus
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.ChangeActivityStatus201302)]
    public interface IChangeActivityStatusApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(ChangeActivityStatusOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.ChangeActivityStatus201302)]
        void Execute(EntityName entityName, long entityId, ActivityStatus status);
    }
}
