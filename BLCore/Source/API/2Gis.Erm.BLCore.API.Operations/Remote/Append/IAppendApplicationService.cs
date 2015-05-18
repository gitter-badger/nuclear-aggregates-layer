using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Append
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.Append201303)]
    public interface IAppendApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(AppendOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Append201303)]
        void Execute(IEntityType entityName, long entityId, IEntityType appendedEntityName, long appendedEntityId);
    }
}