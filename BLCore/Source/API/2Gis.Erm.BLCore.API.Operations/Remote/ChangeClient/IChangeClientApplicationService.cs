using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeClient;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.ChangeClient
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.ChangeClient201303)]
    public interface IChangeClientApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(ChangeClientOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.ChangeClient201303)]
        ChangeEntityClientValidationResult Validate(EntityName entityName, long entityId, long clientId);

        [OperationContract]
        [FaultContract(typeof(ChangeClientOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.ChangeClient201303)]
        ChangeEntityClientResult Execute(EntityName entityName, long entityId, long clientId, bool? bypassValidation);
    }
}