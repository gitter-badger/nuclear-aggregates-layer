﻿using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Qualify;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class QualifyApplicationService : IQualifyApplicationService, IQualifyApplicationRestService
    {
        private readonly ICommonLog _logger;
        private readonly IUserContext _userContext;
        private readonly IOperationServicesManager _operationServicesManager;

        public QualifyApplicationService(ICommonLog logger, IUserContext userContext, IOperationServicesManager operationServicesManager, IResourceGroupManager resourceGroupManager)
        {
            _logger = logger;
            _userContext = userContext;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public long? Execute(string specifiedEntityName, string specifiedEntityId, string specifiedOwnerCode, string specifiedRelatedEntityId)
        {
            var entityName = EntityName.None;

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
                if (!Enum.TryParse(specifiedEntityName, out entityName))
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
                _logger.ErrorFormatEx(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<QualifyOperationErrorDescription>(
                    new QualifyOperationErrorDescription(entityName, ex.Message, ownerCode, actualRelatedEntityId),
                    HttpStatusCode.BadRequest);
            }
        }

        public long? Execute(EntityName entityName, long entityId, long? ownerCode, long? relatedEntityId)
        {
            var actualOwnerCode = ownerCode ?? _userContext.Identity.Code;
            try
            {
                return ExecuteInternal(entityName, entityId, actualOwnerCode, relatedEntityId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<QualifyOperationErrorDescription>(
                    new QualifyOperationErrorDescription(entityName, ex.Message, actualOwnerCode, relatedEntityId));
            }
        }

        private long? ExecuteInternal(EntityName entityName, long entityId, long ownerCode, long? relatedEntityId)
        {
            var qualifyEntityService = _operationServicesManager.GetQualifyEntityService(entityName);
            var qualifyResult = qualifyEntityService.Qualify(entityId, ownerCode, relatedEntityId);
            return qualifyResult.RelatedEntityId;
        }
    }
}
