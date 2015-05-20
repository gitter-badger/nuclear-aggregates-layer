using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Assign;

using NuClear.Model.Common.Entities;
using NuClear.ResourceUtilities;
using NuClear.Security.API.UserContext;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class AssignApplicationService : IAssignApplicationService, IAssignApplicationRestService
    {
        private readonly ITracer _tracer;
        private readonly IUserContext _userContext;
        private readonly IOperationServicesManager _operationServicesManager;

        public AssignApplicationService(ITracer tracer, IUserContext userContext, IOperationServicesManager operationServicesManager, IResourceGroupManager resourceGroupManager)
        {
            _tracer = tracer;
            _userContext = userContext;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public AssignResult Execute(string specifiedEntityName, string specifiedEntityId, string specifiedOwnerCode, string specifiedIsPartialAssign, string specifiedBypassValidation)
        {
            IEntityType entityName = EntityType.Instance.None();

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
                if (!EntityType.Instance.TryParse(specifiedEntityName, out entityName))
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
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<AssignOperationErrorDescription>(
                    new AssignOperationErrorDescription(entityName, ex.Message, ownerCode, bypassValidation, isPartialAssign),
                    HttpStatusCode.BadRequest);
            }
        }

        public AssignResult Execute(IEntityType entityName, long entityId, long? ownerCode, bool? isPartialAssign, bool? bypassValidation)
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
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<AssignOperationErrorDescription>(new AssignOperationErrorDescription(entityName, ex.Message, actualOwnerCode, actualIsPartialAssign, actualBypassValidation));
            }
        }

        private AssignResult ExecuteInternal(IEntityType entityName, long entityId, long ownerCode, bool isPartialAssign, bool bypassValidation)
        {
            var assignEntityService = _operationServicesManager.GetAssignEntityService(entityName);
            return assignEntityService.Assign(entityId, ownerCode, bypassValidation, isPartialAssign);
        }
    }
}
