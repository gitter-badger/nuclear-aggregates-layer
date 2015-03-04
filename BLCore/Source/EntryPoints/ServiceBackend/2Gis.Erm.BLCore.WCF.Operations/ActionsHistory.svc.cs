using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.ActionHistory;
using DoubleGis.Erm.BLCore.API.Operations.Remote.ActionsHistory;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class ActionsHistoryApplicationService : IActionsHistoryApplicationService, IActionsHistoryApplicationRestService
    {
        private readonly ITracer _tracer;
        private readonly IOperationServicesManager _operationServicesManager;

        public ActionsHistoryApplicationService(ITracer tracer,
                                                IOperationServicesManager operationServicesManager,
                                                IUserContext userContext,
                                                IResourceGroupManager resourceGroupManager)
        {
            _tracer = tracer;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public ActionsHistoryDto GetActionsHistory(string specifiedEntityName, string specifiedEntityId)
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

                return GetActionsHistoryInternal(entityName, entityId);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<ActionsHistoryOperationErrorDescription>(new ActionsHistoryOperationErrorDescription(entityName, ex.Message),
                                                                               HttpStatusCode.BadRequest);
            }
        }

        public ActionsHistoryDto GetActionsHistory(EntityName entityName, long entityId)
        {
            try
            {
                return GetActionsHistoryInternal(entityName, entityId);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<ActionsHistoryOperationErrorDescription>(new ActionsHistoryOperationErrorDescription(entityName, ex.Message));
            }
        }

        private ActionsHistoryDto GetActionsHistoryInternal(EntityName entityName, long entityId)
        {
            var actionsHistoryService = _operationServicesManager.GetActionHistoryService(entityName);
            return actionsHistoryService.GetActionHistory(entityName, entityId);
        }
    }
}
