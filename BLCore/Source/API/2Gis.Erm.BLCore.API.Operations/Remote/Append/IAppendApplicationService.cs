using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Append
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.Append201303)]
    public interface IAppendApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(AppendOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Append201303)]
        void Execute(EntityName entityName, long entityId, EntityName appendedEntityName, long appendedEntityId);
    }
}