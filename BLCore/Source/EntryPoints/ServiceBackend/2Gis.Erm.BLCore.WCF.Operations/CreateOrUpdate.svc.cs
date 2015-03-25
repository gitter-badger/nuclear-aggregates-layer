using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Remote.CreateOrUpdate;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class CreateOrUpdateApplicationService : ICreateOrUpdateApplicationService
    {
        private readonly IOperationServicesManager _operationServicesManager;
        private readonly ITracer _tracer;

        public CreateOrUpdateApplicationService(IOperationServicesManager operationServicesManager, ITracer tracer, IUserContext userContext, IResourceGroupManager resourceGroupManager)
        {
            _operationServicesManager = operationServicesManager;
            _tracer = tracer;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public long Execute(EntityName entityName, IDomainEntityDto dto)
        {
            try
            {
                var modifyService = _operationServicesManager.GetModifyDomainEntityService(entityName);
                var entityId = modifyService.Modify(dto);
                return entityId;
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}. Entity details: type [{1}], id [{2}]", GetType().Name, entityName, dto != null ? dto.Id.ToString() : "not defined");
                throw new FaultException<CreateOrUpdateOperationErrorDescription>(new CreateOrUpdateOperationErrorDescription(entityName, ex.Message),
                                                                                  ex.Message);
            }
        }
    }
}
