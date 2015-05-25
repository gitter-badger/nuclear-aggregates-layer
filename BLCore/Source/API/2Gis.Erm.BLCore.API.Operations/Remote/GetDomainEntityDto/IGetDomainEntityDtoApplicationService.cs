using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.GetDomainEntityDto
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.GetDomainEntityDto201306)]
    public interface IGetDomainEntityDtoApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(GetDomainEntityDtoOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.GetDomainEntityDto201306)]
        IDomainEntityDto GetDomainEntityDto(IEntityType entityName, long entityId);
    }
}