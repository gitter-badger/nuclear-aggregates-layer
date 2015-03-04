using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Deactivate;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class DeactivateApplicationService : IDeactivateApplicationService, IDeactivateApplicationRestService
    {
        private readonly ITracer _tracer;
        private readonly IUserContext _userContext;
        private readonly IOperationServicesManager _operationServicesManager;

        public DeactivateApplicationService(ITracer tracer, IUserContext userContext, IOperationServicesManager operationServicesManager, IResourceGroupManager resourceGroupManager)
        {
            _tracer = tracer;
            _userContext = userContext;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public DeactivateConfirmation Execute(string specifiedEntityName, string specifiedEntityId, string specifiedOwnerCode)
        {
            var entityName = EntityName.None;
            long ownerCode;
            if (!long.TryParse(specifiedOwnerCode, out ownerCode))
            {
                ownerCode = _userContext.Identity.Code;
            }
            try
            {
                if (!Enum.TryParse(specifiedEntityName, out entityName))
                {
                    throw new ArgumentException("Entity Name cannot be parsed");
                }

                long entityId;
                if (!long.TryParse(specifiedEntityId, out entityId))
                {
                    throw new ArgumentException("Entity Id cannot be parsed");
                }

                return ExecuteInternal(entityName, entityId, ownerCode);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<DeactivateOperationErrorDescription>(new DeactivateOperationErrorDescription(entityName, ex.Message, ownerCode),
                                                                                 HttpStatusCode.BadRequest);
            }
        }

        public DeactivateConfirmation Execute(EntityName entityName, long entityId, long? ownerCode)
        {
            var actualOwnerCode = ownerCode ?? _userContext.Identity.Code;
            try
            {
                return ExecuteInternal(entityName, entityId, actualOwnerCode);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<DeactivateOperationErrorDescription>(new DeactivateOperationErrorDescription(entityName, ex.Message, actualOwnerCode));
            }
        }

        private DeactivateConfirmation ExecuteInternal(EntityName entityName, long entityId, long ownerCode)
        {
            var deactivateEntityService = _operationServicesManager.GetDeactivateEntityService(entityName);
            return deactivateEntityService.Deactivate(entityId, ownerCode);
        }
    }
}
