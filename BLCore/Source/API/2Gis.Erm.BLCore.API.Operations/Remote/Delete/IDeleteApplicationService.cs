using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Delete
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.Delete201303)]
    public interface IDeleteApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(DeleteOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Delete201303)]
        DeleteConfirmation Execute(IEntityType entityName, long entityId);
    }
}