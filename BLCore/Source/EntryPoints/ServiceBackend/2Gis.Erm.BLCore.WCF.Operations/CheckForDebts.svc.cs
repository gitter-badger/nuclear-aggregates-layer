﻿using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.CheckForDebts;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Activate;
using DoubleGis.Erm.BLCore.API.Operations.Remote.CheckForDebts;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils.Resources;

using Newtonsoft.Json;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class CheckForDebtsApplicationService : ICheckForDebtsApplicationService, ICheckForDebtsApplicationRestService
    {
        private readonly ICommonLog _logger;
        private readonly IOperationServicesManager _operationServicesManager;

        public CheckForDebtsApplicationService(ICommonLog logger, IOperationServicesManager operationServicesManager, IUserContext userContext, IResourceGroupManager resourceGroupManager)
        {
            _logger = logger;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public CheckForDebtsResult Execute(string specifiedEntityName, string specifiedEntityIds)
        {
            IEntityType entityName = EntityType.Instance.None();
            try
            {
                if (!EntityType.Instance.TryParse(specifiedEntityName, out entityName))
                {
                    throw new ArgumentException("Entity Name cannot be parsed");
                }

                var entityIds = JsonConvert.DeserializeObject<long[]>(specifiedEntityIds);
                if (entityIds == null)
                {
                    throw new ArgumentException("Entity Ids cannot be parsed");
                }

                return ExecuteInternal(entityName, entityIds);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<ActivateOperationErrorDescription>(new ActivateOperationErrorDescription(entityName, ex.Message),
                                                                               HttpStatusCode.BadRequest);
            }
        }

        public CheckForDebtsResult Execute(IEntityType entityName, IEnumerable<long> entityIds)
        {
            try
            {
                return ExecuteInternal(entityName, entityIds);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<ActivateOperationErrorDescription>(new ActivateOperationErrorDescription(entityName, ex.Message));
            }
        }

        private CheckForDebtsResult ExecuteInternal(IEntityType entityName, IEnumerable<long> entityIds)
        {
            var checkEntityForDebtsService = _operationServicesManager.GetCheckEntityForDebtsService(entityName);
            foreach (var entityId in entityIds)
            {
                var checkForDebtsResult = checkEntityForDebtsService.CheckForDebts(entityId);
                if (checkForDebtsResult.DebtsExist)
                {
                    return checkForDebtsResult;
                }
            }
            return new CheckForDebtsResult();
        }
    }
}
