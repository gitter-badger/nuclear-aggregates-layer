using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Delete;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class DeleteApplicationService : IDeleteApplicationService, IDeleteApplicationRestService
    {
        private readonly ICommonLog _logger;
        private readonly IOperationServicesManager _operationServicesManager;

        public DeleteApplicationService(ICommonLog logger, IOperationServicesManager operationServicesManager, IUserContext userContext, IResourceGroupManager resourceGroupManager)
        {
            _logger = logger;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public DeleteConfirmation Execute(string specifiedEntityName, string specifiedEntityId)
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

                return ExecuteInternal(entityName, entityId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<DeleteOperationErrorDescription>(new DeleteOperationErrorDescription(entityName, ex.Message),
                                                                             HttpStatusCode.BadRequest);
            }
        }

        public DeleteConfirmation Execute(EntityName entityName, long entityId)
        {
            try
            {
                return ExecuteInternal(entityName, entityId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<DeleteOperationErrorDescription>(new DeleteOperationErrorDescription(entityName, ex.Message));
            }
        }

        private DeleteConfirmation ExecuteInternal(EntityName entityName, long entityId)
        {
            var deleteEntityService = _operationServicesManager.GetDeleteEntityService(entityName);
            return deleteEntityService.Delete(entityId);
        }
    }
}
