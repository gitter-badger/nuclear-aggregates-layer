using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Assign;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class AssignApplicationService : IAssignApplicationService, IAssignApplicationRestService
    {
        private readonly ITracer _logger;
        private readonly IUserContext _userContext;
        private readonly IOperationServicesManager _operationServicesManager;

        public AssignApplicationService(ITracer logger, IUserContext userContext, IOperationServicesManager operationServicesManager, IResourceGroupManager resourceGroupManager)
        {
            _logger = logger;
            _userContext = userContext;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public AssignResult Execute(string specifiedEntityName, string specifiedEntityId, string specifiedOwnerCode, string specifiedIsPartialAssign, string specifiedBypassValidation)
        {
            var entityName = EntityName.None;

            long ownerCode;
            if (!long.TryParse(specifiedOwnerCode, out ownerCode))
            {
                ownerCode = _userContext.Identity.Code;
            }

            bool isPartialAssign;
            if (!bool.TryParse(specifiedIsPartialAssign, out isPartialAssign))
            {
                isPartialAssign = false;
            }

            bool bypassValidation;
            if (!bool.TryParse(specifiedBypassValidation, out bypassValidation))
            {
                bypassValidation = false;
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

                return ExecuteInternal(entityName, entityId, ownerCode, isPartialAssign, bypassValidation);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<AssignOperationErrorDescription>(
                    new AssignOperationErrorDescription(entityName, ex.Message, ownerCode, bypassValidation, isPartialAssign),
                    HttpStatusCode.BadRequest);
            }
        }

        public AssignResult Execute(EntityName entityName, long entityId, long? ownerCode, bool? isPartialAssign, bool? bypassValidation)
        {
            var actualOwnerCode = ownerCode ?? _userContext.Identity.Code;
            var actualIsPartialAssign = isPartialAssign ?? false;
            var actualBypassValidation = bypassValidation ?? false;

            try
            {
                return ExecuteInternal(entityName, entityId, actualOwnerCode, actualIsPartialAssign, actualBypassValidation);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<AssignOperationErrorDescription>(new AssignOperationErrorDescription(entityName, ex.Message, actualOwnerCode, actualIsPartialAssign, actualBypassValidation));
            }
        }

        private AssignResult ExecuteInternal(EntityName entityName, long entityId, long ownerCode, bool isPartialAssign, bool bypassValidation)
        {
            var assignEntityService = _operationServicesManager.GetAssignEntityService(entityName);
            return assignEntityService.Assign(entityId, ownerCode, bypassValidation, isPartialAssign);
        }
    }
}
