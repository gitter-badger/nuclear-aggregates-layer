using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Complete;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Resources;

using NuClear.Model.Common.Entities;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class CompleteApplicationService : ICompleteApplicationService, ICompleteApplicationRestService
    {
        private readonly ITracer _logger;
        private readonly IOperationServicesManager _operationServicesManager;

        public CompleteApplicationService(ITracer logger, IOperationServicesManager operationServicesManager, IUserContext userContext, IResourceGroupManager resourceGroupManager)
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
                throw new WebFaultException<CompleteOperationErrorDescription>(new CompleteOperationErrorDescription(entityName, ex.Message),
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
                throw new WebFaultException<CompleteOperationErrorDescription>(new CompleteOperationErrorDescription(entityName, ex.Message),
                                                                               HttpStatusCode.BadRequest);
            }
        }

        private void ExecuteInternal(IEntityType entityName, long entityId)
        {
            var completeService = _operationServicesManager.GetCompleteService(entityName);
            completeService.Complete(entityId);
        }
    }
}
