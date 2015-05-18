using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Cancel
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.Cancel201502)]
    public interface ICancelApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(CancelOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Cancel201502)]
        void Execute(IEntityType entityName, long entityId);
    }
}
