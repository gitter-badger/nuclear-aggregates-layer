using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Cancel;

using NuClear.Model.Common.Entities;
using NuClear.ResourceUtilities;
using NuClear.Security.API.UserContext;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class CancelApplicationService : ICancelApplicationService, ICancelApplicationRestService
    {
        private readonly ITracer _logger;
        private readonly IOperationServicesManager _operationServicesManager;

        public CancelApplicationService(ITracer logger, IOperationServicesManager operationServicesManager, IUserContext userContext, IResourceGroupManager resourceGroupManager)
        {
            _logger = logger;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public void Execute(IEntityType entityName, long entityId)
        {
            try
            {
                ExecuteInternal(entityName, entityId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<CancelOperationErrorDescription>(new CancelOperationErrorDescription(entityName, ex.Message),
                                                                               HttpStatusCode.BadRequest);
            }
        }

        public void Execute(string specifiedEntityName, string specifiedEntityId)
        {
            IEntityType entityName = EntityType.Instance.None();
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
              
                ExecuteInternal(entityName, entityId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<CancelOperationErrorDescription>(new CancelOperationErrorDescription(entityName, ex.Message),
                                                                               HttpStatusCode.BadRequest);
            }
        }

        private void ExecuteInternal(IEntityType entityName, long entityId)
        {
            var cancelService = _operationServicesManager.GetCancelService(entityName);
            cancelService.Cancel(entityId);
        }
    }
}
