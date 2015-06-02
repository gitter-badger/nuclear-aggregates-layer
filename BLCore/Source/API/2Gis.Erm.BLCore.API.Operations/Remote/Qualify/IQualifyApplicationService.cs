using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Qualify;
using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Qualify
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.Qualify201303)]
    public interface IQualifyApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(QualifyOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Qualify201303)]
        QualifyResult Execute(IEntityType entityName, long entityId, long? ownerCode, long? relatedEntityId);
    }
}