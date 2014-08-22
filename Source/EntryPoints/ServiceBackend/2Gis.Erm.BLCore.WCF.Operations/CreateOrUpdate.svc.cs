﻿using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Remote.CreateOrUpdate;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class CreateOrUpdateApplicationService : ICreateOrUpdateApplicationService
    {
        private readonly IOperationServicesManager _operationServicesManager;
        private readonly ICommonLog _logger;

        public CreateOrUpdateApplicationService(IOperationServicesManager operationServicesManager, ICommonLog logger, IUserContext userContext, IResourceGroupManager resourceGroupManager)
        {
            _operationServicesManager = operationServicesManager;
            _logger = logger;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public long Execute(EntityName entityName, IDomainEntityDto dto)
        {
            try
            {
                var modifyService = _operationServicesManager.GetModifyDomainEntityService(entityName);
                var entityId = modifyService.Modify(dto);
                return entityId;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(ex, "Error has occured in {0}. Entity details: type [{1}], id [{2}]", GetType().Name, entityName, dto != null ? dto.Id.ToString() : "not defined");
                throw new FaultException<CreateOrUpdateOperationErrorDescription>(new CreateOrUpdateOperationErrorDescription(entityName, ex.Message),
                                                                                  ex.Message);
            }
        }
    }
}
