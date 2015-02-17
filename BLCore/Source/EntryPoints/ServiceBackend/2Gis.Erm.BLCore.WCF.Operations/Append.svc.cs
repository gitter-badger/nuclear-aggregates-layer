using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Append;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Append;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils.Resources;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class AppendApplicationService : IAppendApplicationService, IAppendApplicationRestService
    {
        private readonly ICommonLog _logger;
        private readonly IOperationServicesManager _operationServicesManager;

        public AppendApplicationService(ICommonLog logger, IOperationServicesManager operationServicesManager, IUserContext userContext, IResourceGroupManager resourceGroupManager)
        {
            _logger = logger;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public void Execute(string specifiedEntityName, string specifiedEntityId, string specifiedAppendedEntityName, string specifiedAppendedEntityId)
        {
            IEntityType entityName = EntityType.Instance.None();
            var entityId = 0L;
            IEntityType appenedEntityName = EntityType.Instance.None();
            var appenedEntityId = 0L;

            try
            {
                if (!EntityType.Instance.TryParse(specifiedEntityName, out entityName))
                {
                    throw new ArgumentException("Entity Name cannot be parsed");
                }

                if (!long.TryParse(specifiedEntityId, out entityId))
                {
                    throw new ArgumentException("Entity Id cannot be parsed");
                }

                if (!EntityType.Instance.TryParse(specifiedAppendedEntityName, out appenedEntityName))
                {
                    throw new ArgumentException("Appended Entity Name cannot be parsed");
                }

                if (!long.TryParse(specifiedAppendedEntityId, out appenedEntityId))
                {
                    throw new ArgumentException("Appended Entity Id cannot be parsed");
                }

                ExecuteInternal(entityName, entityId, appenedEntityName, appenedEntityId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<AppendOperationErrorDescription>(
                    new AppendOperationErrorDescription(entityName, entityId, appenedEntityName, appenedEntityId, ex.Message), HttpStatusCode.BadRequest);
            }
        }

        public void Execute(IEntityType entityName, long entityId, IEntityType appendedEntityName, long appendedEntityId)
        {
            try
            {
                ExecuteInternal(entityName, entityId, appendedEntityName, appendedEntityId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<AppendOperationErrorDescription>(
                    new AppendOperationErrorDescription(entityName, entityId, appendedEntityName, appendedEntityId, ex.Message));
            }
        }

        private void ExecuteInternal(IEntityType entityName, long entityId, IEntityType appendedEntityName, long appendedEntityId)
        {
            var actionsHistoryService = _operationServicesManager.GetAppendEntityService(entityName, appendedEntityName);
            actionsHistoryService.Append(new AppendParams
                {
                    ParentType = entityName,
                    ParentId = entityId,
                    AppendedType = appendedEntityName,
                    AppendedId = appendedEntityId
                });
        }
    }
}
