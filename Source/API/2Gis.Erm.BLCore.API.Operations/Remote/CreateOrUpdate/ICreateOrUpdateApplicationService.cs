using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.CreateOrUpdate
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.CreateOrUpdate201304)]
    public interface ICreateOrUpdateApplicationService 
    {
        [OperationContract]
        [FaultContract(typeof(CreateOrUpdateOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.CreateOrUpdate201304)]
        long Execute(EntityName entityName, IDomainEntityDto dto);
    }
}
