using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.CreateOrUpdate
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.CreateOrUpdate201304)]
    public interface ICreateOrUpdateApplicationService 
    {
        [OperationContract]
        [FaultContract(typeof(CreateOrUpdateOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.CreateOrUpdate201304)]
        long Execute(IEntityType entityName, IDomainEntityDto dto);
    }
}
