﻿using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Remote.ChangeTerritory;

using NuClear.Model.Common.Entities;
using NuClear.ResourceUtilities;
using NuClear.Security.API.UserContext;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class ChangeTerritoryApplicationService : IChangeTerritoryApplicationService, IChangeTerritoryApplicationRestService
    {
        private readonly ITracer _tracer;
        private readonly IOperationServicesManager _operationServicesManager;

        public ChangeTerritoryApplicationService(ITracer tracer, IOperationServicesManager operationServicesManager, IUserContext userContext, IResourceGroupManager resourceGroupManager)
        {
            _tracer = tracer;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public void Execute(string specifiedEntityName, string specifiedEntityId, string specifiedTerritoryId)
        {
            IEntityType entityName = EntityType.Instance.None();
            var entityId = 0L;
            var territoryId = 0L;

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

                if (!long.TryParse(specifiedTerritoryId, out territoryId))
                {
                    throw new ArgumentException("Territory Id cannot be parsed");
                }

                ExecuteInternal(entityName, entityId, territoryId);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<ChangeTerritoryOperationErrorDescription>(
                    new ChangeTerritoryOperationErrorDescription(entityName, ex.Message, entityId, territoryId),
                    HttpStatusCode.BadRequest);
            }
        }

        public void Execute(IEntityType entityName, long entityId, long territoryId)
        {
            try
            {
                ExecuteInternal(entityName, entityId, territoryId);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<ChangeTerritoryOperationErrorDescription>(
                    new ChangeTerritoryOperationErrorDescription(entityName, ex.Message, entityId, territoryId));
            }
        }

        private void ExecuteInternal(IEntityType entityName, long entityId, long territoryId)
        {
            var changeEntityTerritoryService = _operationServicesManager.GetChangeEntityTerritoryService(entityName);
            changeEntityTerritoryService.ChangeTerritory(entityId, territoryId);
        }
    }
}
