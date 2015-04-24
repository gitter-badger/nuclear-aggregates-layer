using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Remote.GetDomainEntityDto;

using NuClear.Security.API.UserContext;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.ResourceUtilities;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class GetDomainEntityDtoApplicationService : IGetDomainEntityDtoApplicationService
    {
        private readonly ITracer _tracer;
        private readonly IOperationServicesManager _operationServicesManager;

        public GetDomainEntityDtoApplicationService(ITracer tracer, IOperationServicesManager operationServicesManager, IUserContext userContext, IResourceGroupManager resourceGroupManager)
        {
            _tracer = tracer;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public IDomainEntityDto GetDomainEntityDto(IEntityType entityName, long entityId)
        {
            try
            {
                var modifyService = _operationServicesManager.GetDomainEntityDtoService(entityName);
                var domainEntityDto = modifyService.GetDomainEntityDto(entityId, false, null, EntityType.Instance.None(), string.Empty);
                return domainEntityDto;
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}. Entity details: type [{1}], id [{2}]", GetType().Name, entityName, entityId);
                throw new FaultException<GetDomainEntityDtoOperationErrorDescription>(new GetDomainEntityDtoOperationErrorDescription(entityName, ex.Message),
                                                                                      ex.Message);
            }
        }
    }
}
