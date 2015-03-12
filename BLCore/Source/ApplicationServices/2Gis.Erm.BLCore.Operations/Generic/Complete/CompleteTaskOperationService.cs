using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Complete;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Complete;
using DoubleGis.Erm.BLCore.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Complete;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Complete
{
    public class CompleteTaskOperationService : ICompleteGenericOperationService<Task>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ITaskReadModel _taskReadModel;

        private readonly IActionLogger _actionLogger;

        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;

        private readonly ICompleteTaskAggregateService _completeTaskAggregateService;

        public CompleteTaskOperationService(
            IOperationScopeFactory operationScopeFactory,
            ITaskReadModel taskReadModel,
            IActionLogger actionLogger,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext,
            ICompleteTaskAggregateService completeTaskAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _taskReadModel = taskReadModel;
            _actionLogger = actionLogger;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _completeTaskAggregateService = completeTaskAggregateService;
        }

        public void Complete(long entityId)
        {
            var task = _taskReadModel.GetTask(entityId);
            var originalStatus = task.Status;

            if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, task.OwnerCode))
            {
                throw new SecurityException(string.Format("{0}: {1}", task.Header, BLResources.SecurityAccessDenied));
            }   

            using (var scope = _operationScopeFactory.CreateSpecificFor<CompleteIdentity, Task>())
            {                
                _completeTaskAggregateService.Complete(task);

                _actionLogger.LogChanges(task, x => x.Status, originalStatus, task.Status);

                scope.Updated<Task>(entityId);
                scope.Complete();
            }
        }
    }
}
