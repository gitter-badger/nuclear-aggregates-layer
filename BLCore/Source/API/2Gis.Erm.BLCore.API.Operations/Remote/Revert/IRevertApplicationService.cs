using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Revert
{  
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.Revert201502)]
    public interface IRevertApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(RevertOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Revert201502)]
        void Execute(EntityName entityName, long entityId);
    }
}
