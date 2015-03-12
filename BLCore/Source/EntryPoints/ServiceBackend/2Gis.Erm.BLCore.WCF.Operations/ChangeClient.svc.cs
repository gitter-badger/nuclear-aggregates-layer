using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeClient;
using DoubleGis.Erm.BLCore.API.Operations.Remote.ChangeClient;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class ChangeClientApplicationService : IChangeClientApplicationService, IChangeClientApplicationRestService
    {
        private readonly ITracer _tracer;
        private readonly IOperationServicesManager _operationServicesManager;

        public ChangeClientApplicationService(ITracer tracer, IOperationServicesManager operationServicesManager, IUserContext userContext, IResourceGroupManager resourceGroupManager)
        {
            _tracer = tracer;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public ChangeEntityClientValidationResult Validate(string specifiedEntityName, string specifiedEntityId, string specifiedClientId)
        {
            var entityName = EntityName.None;
            var entityId = 0L;
            var clientId = 0L;

            try
            {
                if (!Enum.TryParse(specifiedEntityName, out entityName))
                {
                    throw new ArgumentException("Entity Name cannot be parsed");
                }

                if (!long.TryParse(specifiedEntityId, out entityId))
                {
                    throw new ArgumentException("Entity Id cannot be parsed");
                }

                if (!long.TryParse(specifiedClientId, out clientId))
                {
                    throw new ArgumentException("Client Id cannot be parsed");
                }

                return ValidateInternal(entityName, entityId, clientId);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<ChangeClientOperationErrorDescription>(
                    new ChangeClientOperationErrorDescription(entityName, ex.Message, entityId, clientId, null),
                    HttpStatusCode.BadRequest);
            }
        }

        public ChangeEntityClientResult Execute(string specifiedEntityName, string specifiedEntityId, string specifiedClientId, string specifiedBypassValidation)
        {
            var entityName = EntityName.None;
            var entityId = 0L;
            var clientId = 0L;

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

                if (!long.TryParse(specifiedEntityId, out entityId))
                {
                    throw new ArgumentException("Entity Id cannot be parsed");
                }

                if (!long.TryParse(specifiedClientId, out clientId))
                {
                    throw new ArgumentException("Client Id cannot be parsed");
                }

                return ExecuteInternal(entityName, entityId, clientId, bypassValidation);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<ChangeClientOperationErrorDescription>(
                    new ChangeClientOperationErrorDescription(entityName, ex.Message, entityId, clientId, bypassValidation),
                    HttpStatusCode.BadRequest);
            }
        }

        public ChangeEntityClientValidationResult Validate(EntityName entityName, long entityId, long clientId)
        {
            try
            {
                return ValidateInternal(entityName, entityId, clientId);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<ChangeClientOperationErrorDescription>(
                    new ChangeClientOperationErrorDescription(entityName, ex.Message, entityId, clientId, null));
            }
        }

        public ChangeEntityClientResult Execute(EntityName entityName, long entityId, long clientId, bool? bypassValidation)
        {
            var actualBypassValidation = bypassValidation ?? false;
            try
            {
                return ExecuteInternal(entityName, entityId, clientId, actualBypassValidation);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<ChangeClientOperationErrorDescription>(
                    new ChangeClientOperationErrorDescription(entityName, ex.Message, entityId, clientId, actualBypassValidation));
            }
        }

        private ChangeEntityClientValidationResult ValidateInternal(EntityName entityName, long entityId, long clientId)
        {
            var changeEntityClientService = _operationServicesManager.GetChangeEntityClientService(entityName);
            return changeEntityClientService.Validate(entityId, clientId);
        }

        private ChangeEntityClientResult ExecuteInternal(EntityName entityName, long entityId, long clientId, bool bypassValidation)
        {
            var changeEntityClientService = _operationServicesManager.GetChangeEntityClientService(entityName);
            return changeEntityClientService.Execute(entityId, clientId, bypassValidation);
        }
    }
}
