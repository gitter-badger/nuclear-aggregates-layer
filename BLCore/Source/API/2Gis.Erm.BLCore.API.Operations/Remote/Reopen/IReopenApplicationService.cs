using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Reopen
{  
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.Reopen201502)]
    public interface IReopenApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(ReopenOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Reopen201502)]
        void Execute(IEntityType entityName, long entityId);
    }
}
