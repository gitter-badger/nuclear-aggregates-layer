using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Deactivate
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.Deactivate201303)]
    public interface IDeactivateApplicationService
    {
         [OperationContract]
         [FaultContract(typeof(DeactivateOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Deactivate201303)]
         DeactivateConfirmation Execute(IEntityType entityName, long entityId, long? ownerCode);
    }
}