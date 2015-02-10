using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Activate;
using DoubleGis.Erm.BLCore.API.Operations.Remote.ChangeActivityStatus;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class ChangeActivityStatus : IChangeActivityStatusApplicationService, IChangeActivityStatusApplicationRestService
    {
        private readonly ICommonLog _logger;
        private readonly IOperationServicesManager _operationServicesManager;

        public ChangeActivityStatus(ICommonLog logger, IOperationServicesManager operationServicesManager, IUserContext userContext, IResourceGroupManager resourceGroupManager)
        {
            _logger = logger;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public void Execute(EntityName entityName, long entityId, ActivityStatus status)
        {
            try
            {
                this.ExecuteInternal(entityName, entityId, status);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<ChangeActivityStatusOperationErrorDescription>(new ChangeActivityStatusOperationErrorDescription(entityName, ex.Message),
                                                                               HttpStatusCode.BadRequest);
            }
        }

        public void Execute(string specifiedEntityName, string entityId, string status)
        {
            var entityName = EntityName.None;
            try
            {
                if (!Enum.TryParse(specifiedEntityName, out entityName))
                {
                    throw new ArgumentException("Entity Name cannot be parsed");
                }

                ActivityStatus activityStatus;
                if (!Enum.TryParse(status, out activityStatus))
                {
                    throw new ArgumentException("ActivityStatus cannot be parsed");
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<ChangeActivityStatusOperationErrorDescription>(new ChangeActivityStatusOperationErrorDescription(entityName, ex.Message),
                                                                               HttpStatusCode.BadRequest);
            }
        }

        private void ExecuteInternal(EntityName entityName, long entityId, ActivityStatus status)
        {
            var changeActivityService = _operationServicesManager.GetChangeActivityStatusService(entityName);
            changeActivityService.ChangeStatus(entityId, status);
        }
    }
}
