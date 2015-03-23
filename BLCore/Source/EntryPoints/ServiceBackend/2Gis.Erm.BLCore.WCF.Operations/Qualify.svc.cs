using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Qualify;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Qualify;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Resources;

using NuClear.Model.Common.Entities;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class QualifyApplicationService : IQualifyApplicationService, IQualifyApplicationRestService
    {
        private readonly ITracer _tracer;
        private readonly IUserContext _userContext;
        private readonly IOperationServicesManager _operationServicesManager;

        public QualifyApplicationService(ITracer tracer, IUserContext userContext, IOperationServicesManager operationServicesManager, IResourceGroupManager resourceGroupManager)
        {
            _tracer = tracer;
            _userContext = userContext;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public QualifyResult Execute(string specifiedEntityName, string specifiedEntityId, string specifiedOwnerCode, string specifiedRelatedEntityId)
        {
            IEntityType entityName = EntityType.Instance.None();

            long ownerCode;
            if (!long.TryParse(specifiedOwnerCode, out ownerCode))
            {
                ownerCode = _userContext.Identity.Code;
            }

            long relatedEntityId;
            long? actualRelatedEntityId = null;
            if (long.TryParse(specifiedRelatedEntityId, out relatedEntityId))
            {
                actualRelatedEntityId = relatedEntityId;
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

                return ExecuteInternal(entityName, entityId, ownerCode, actualRelatedEntityId);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<QualifyOperationErrorDescription>(
                    new QualifyOperationErrorDescription(entityName, ex.Message, ownerCode, actualRelatedEntityId),
                    HttpStatusCode.BadRequest);
            }
        }

        public QualifyResult Execute(IEntityType entityName, long entityId, long? ownerCode, long? relatedEntityId)
        {
            var actualOwnerCode = ownerCode ?? _userContext.Identity.Code;
            try
            {
                return ExecuteInternal(entityName, entityId, actualOwnerCode, relatedEntityId);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<QualifyOperationErrorDescription>(
                    new QualifyOperationErrorDescription(entityName, ex.Message, actualOwnerCode, relatedEntityId));
            }
        }

        private QualifyResult ExecuteInternal(IEntityType entityName, long entityId, long ownerCode, long? relatedEntityId)
        {
            var qualifyEntityService = _operationServicesManager.GetQualifyEntityService(entityName);
            return qualifyEntityService.Qualify(entityId, ownerCode, relatedEntityId);
        }
    }
}
