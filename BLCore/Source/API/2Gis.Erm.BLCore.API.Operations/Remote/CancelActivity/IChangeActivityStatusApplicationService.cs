using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.CancelActivity;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.CancelActivity
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.CancelActivity201502)]
    public interface ICancelActivityApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(CancelActivityOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.CancelActivity201502)]
        CancelActivityResult Execute(EntityName entityName, long entityId);
    }
}
