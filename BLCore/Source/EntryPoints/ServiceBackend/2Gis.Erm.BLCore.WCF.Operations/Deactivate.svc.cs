using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Deactivate;

using NuClear.Model.Common.Entities;
using NuClear.ResourceUtilities;
using NuClear.Security.API.UserContext;
using NuClear.Tracing.API;

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
            IEntityType entityName = EntityType.Instance.None();
            long ownerCode;
            if (!long.TryParse(specifiedOwnerCode, out ownerCode))
            {
                ownerCode = _userContext.Identity.Code;
            }
            try
            {
                if (!EntityType.Instance.TryParse(specifiedEntityName, out entityName))
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

        public DeactivateConfirmation Execute(IEntityType entityName, long entityId, long? ownerCode)
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

        private DeactivateConfirmation ExecuteInternal(IEntityType entityName, long entityId, long ownerCode)
        {
            var deactivateEntityService = _operationServicesManager.GetDeactivateEntityService(entityName);
            return deactivateEntityService.Deactivate(entityId, ownerCode);
        }
    }
}
