using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.CheckForDebts;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Activate;
using DoubleGis.Erm.BLCore.API.Operations.Remote.CheckForDebts;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities;

using Newtonsoft.Json;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class CheckForDebtsApplicationService : ICheckForDebtsApplicationService, ICheckForDebtsApplicationRestService
    {
        private readonly ITracer _tracer;
        private readonly IOperationServicesManager _operationServicesManager;

        public CheckForDebtsApplicationService(ITracer tracer, IOperationServicesManager operationServicesManager, IUserContext userContext, IResourceGroupManager resourceGroupManager)
        {
            _tracer = tracer;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public CheckForDebtsResult Execute(string specifiedEntityName, string specifiedEntityIds)
        {
            var entityName = EntityName.None;
            try
            {
                if (!Enum.TryParse(specifiedEntityName, out entityName))
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
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<ActivateOperationErrorDescription>(new ActivateOperationErrorDescription(entityName, ex.Message),
                                                                               HttpStatusCode.BadRequest);
            }
        }

        public CheckForDebtsResult Execute(EntityName entityName, IEnumerable<long> entityIds)
        {
            try
            {
                return ExecuteInternal(entityName, entityIds);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<ActivateOperationErrorDescription>(new ActivateOperationErrorDescription(entityName, ex.Message));
            }
        }

        private CheckForDebtsResult ExecuteInternal(EntityName entityName, IEnumerable<long> entityIds)
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
