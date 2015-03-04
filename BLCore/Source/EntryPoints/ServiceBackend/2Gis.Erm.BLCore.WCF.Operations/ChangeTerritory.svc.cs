using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Remote.ChangeTerritory;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class ChangeTerritoryApplicationService : IChangeTerritoryApplicationService, IChangeTerritoryApplicationRestService
    {
        private readonly ICommonLog _logger;
        private readonly IOperationServicesManager _operationServicesManager;

        public ChangeTerritoryApplicationService(ICommonLog logger, IOperationServicesManager operationServicesManager, IUserContext userContext, IResourceGroupManager resourceGroupManager)
        {
            _logger = logger;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public void Execute(string specifiedEntityName, string specifiedEntityId, string specifiedTerritoryId)
        {
            var entityName = EntityName.None;
            var entityId = 0L;
            var territoryId = 0L;

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

                if (!long.TryParse(specifiedTerritoryId, out territoryId))
                {
                    throw new ArgumentException("Territory Id cannot be parsed");
                }

                ExecuteInternal(entityName, entityId, territoryId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<ChangeTerritoryOperationErrorDescription>(
                    new ChangeTerritoryOperationErrorDescription(entityName, ex.Message, entityId, territoryId),
                    HttpStatusCode.BadRequest);
            }
        }

        public void Execute(EntityName entityName, long entityId, long territoryId)
        {
            try
            {
                ExecuteInternal(entityName, entityId, territoryId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<ChangeTerritoryOperationErrorDescription>(
                    new ChangeTerritoryOperationErrorDescription(entityName, ex.Message, entityId, territoryId));
            }
        }

        private void ExecuteInternal(EntityName entityName, long entityId, long territoryId)
        {
            var changeEntityTerritoryService = _operationServicesManager.GetChangeEntityTerritoryService(entityName);
            changeEntityTerritoryService.ChangeTerritory(entityId, territoryId);
        }
    }
}
