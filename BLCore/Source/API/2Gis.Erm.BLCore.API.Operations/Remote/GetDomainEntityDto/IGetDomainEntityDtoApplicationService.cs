using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.GetDomainEntityDto
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.GetDomainEntityDto201306)]
    public interface IGetDomainEntityDtoApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(GetDomainEntityDtoOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.GetDomainEntityDto201306)]
        IDomainEntityDto GetDomainEntityDto(EntityName entityName, long entityId);
    }
}