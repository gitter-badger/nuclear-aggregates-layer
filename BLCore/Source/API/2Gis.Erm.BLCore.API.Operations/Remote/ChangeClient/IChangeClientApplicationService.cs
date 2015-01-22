using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeClient;
using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.ChangeClient
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.ChangeClient201303)]
    public interface IChangeClientApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(ChangeClientOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.ChangeClient201303)]
        ChangeEntityClientValidationResult Validate(IEntityType entityName, long entityId, long clientId);

        [OperationContract]
        [FaultContract(typeof(ChangeClientOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.ChangeClient201303)]
        ChangeEntityClientResult Execute(IEntityType entityName, long entityId, long clientId, bool? bypassValidation);
    }
}