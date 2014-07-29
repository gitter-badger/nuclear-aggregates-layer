using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Qualify
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.Qualify201303)]
    public interface IQualifyApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(QualifyOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Qualify201303)]
        long? Execute(EntityName entityName, long entityId, long? ownerCode, long? relatedEntityId);
    }
}