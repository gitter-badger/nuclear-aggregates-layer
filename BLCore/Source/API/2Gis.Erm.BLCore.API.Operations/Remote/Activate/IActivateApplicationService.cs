using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Activate
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.Activate201303)]
    public interface IActivateApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(ActivateOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.Activate201303)]
        void Execute(EntityName entityName, long entityId);
    }
}