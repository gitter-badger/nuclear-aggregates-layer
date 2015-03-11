using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Disqualify;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class DisqualifyApplicationService : IDisqualifyApplicationService, IDisqualifyApplicationRestService
    {
        private readonly ITracer _tracer;
        private readonly IOperationServicesManager _operationServicesManager;

        public DisqualifyApplicationService(ITracer tracer, IOperationServicesManager operationServicesManager, IUserContext userContext, IResourceGroupManager resourceGroupManager)
        {
            _tracer = tracer;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public DisqualifyResult Execute(string specifiedEntityName, string specifiedEntityId, string specifiedBypassValidation)
        {
            var entityName = EntityName.None;

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

                return ExecuteInternal(entityName, entityId, bypassValidation);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<DisqualifyOperationErrorDescription>(
                    new DisqualifyOperationErrorDescription(entityName, ex.Message, bypassValidation), HttpStatusCode.BadRequest);
            }
        }

        public DisqualifyResult Execute(EntityName entityName, long entityId, bool? bypassValidation)
        {
            var actualBypassValidatione = bypassValidation ?? false;
            try
            {
                return ExecuteInternal(entityName, entityId, actualBypassValidatione);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<DisqualifyOperationErrorDescription>(
                    new DisqualifyOperationErrorDescription(entityName, ex.Message, actualBypassValidatione));
            }
        }

        private DisqualifyResult ExecuteInternal(EntityName entityName, long entityId, bool bypassValidation)
        {
            var disqualifyEntityService = _operationServicesManager.GetDisqualifyEntityService(entityName);
            return disqualifyEntityService.Disqualify(entityId, bypassValidation);
        }
    }
}
