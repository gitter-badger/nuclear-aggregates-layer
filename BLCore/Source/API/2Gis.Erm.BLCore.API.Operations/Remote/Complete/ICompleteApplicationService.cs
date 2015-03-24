using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Complete
{   
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.Complete201502)]
    public interface ICompleteApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(CompleteOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Complete201502)]
        void Execute(EntityName entityName, long entityId);
    }
}
