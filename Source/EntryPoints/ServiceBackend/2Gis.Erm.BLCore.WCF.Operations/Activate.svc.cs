﻿using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Activate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class ActivateApplicationService : IActivateApplicationService, IActivateApplicationRestService
    {
        private readonly ICommonLog _logger;
        private readonly IOperationServicesManager _operationServicesManager;

        public ActivateApplicationService(ICommonLog logger, IOperationServicesManager operationServicesManager, IUserContext userContext)
        {
            _logger = logger;
            _operationServicesManager = operationServicesManager;

            BLResources.Culture = userContext.Profile.UserLocaleInfo.UserCultureInfo;
            MetadataResources.Culture = userContext.Profile.UserLocaleInfo.UserCultureInfo;
            EnumResources.Culture = userContext.Profile.UserLocaleInfo.UserCultureInfo;
        }

        public void Execute(string specifiedEntityName, string specifiedEntityId)
        {
            var entityName = EntityName.None;
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

                ExecuteInternal(entityName, entityId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<ActivateOperationErrorDescription>(new ActivateOperationErrorDescription(entityName, ex.Message),
                                                                               HttpStatusCode.BadRequest);
            }
        }

        public void Execute(EntityName entityName, long entityId)
        {
            try
            {
                ExecuteInternal(entityName, entityId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<ActivateOperationErrorDescription>(new ActivateOperationErrorDescription(entityName, ex.Message));
            }
        }

        private void ExecuteInternal(EntityName entityName, long entityId)
        {
            var activateEntityService = _operationServicesManager.GetActivateEntityService(entityName);
            activateEntityService.Activate(entityId);
        }
    }
}
