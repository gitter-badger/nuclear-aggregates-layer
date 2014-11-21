using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Delete
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.Delete201303)]
    public interface IDeleteApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(DeleteOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Delete201303)]
        DeleteConfirmation Execute(EntityName entityName, long entityId);
    }
}