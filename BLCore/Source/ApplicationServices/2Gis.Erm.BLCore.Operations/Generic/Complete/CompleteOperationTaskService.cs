using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Complete;
using DoubleGis.Erm.BLCore.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Complete;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Complete
{
    public class CompleteOperationTaskService : ICompleteOperationGenericService<Task>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ITaskReadModel _taskReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;
        private readonly IChangeTaskStatusAggregateService _changeTaskStatusAggregateService;

        public CompleteOperationTaskService(
            IOperationScopeFactory operationScopeFactory,
            ITaskReadModel taskReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext,
            IChangeTaskStatusAggregateService changeTaskStatusAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _taskReadModel = taskReadModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _changeTaskStatusAggregateService = changeTaskStatusAggregateService;
        }

        public void Complete(long entityId)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<CompleteIdentity, Task>())
            {
                var task = _taskReadModel.GetTask(entityId);

                if (task.Status != ActivityStatus.InProgress)
                {
                    throw new BusinessLogicException(string.Format(BLResources.CannotCompleteFinishedOrClosedActivity, task.Header));
                }

                if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, task.OwnerCode))
                {
                    throw new SecurityException(string.Format("{0}: {1}", task.Header, BLResources.SecurityAccessDenied));
                }   

                _changeTaskStatusAggregateService.Change(task, ActivityStatus.Completed);

                scope.Updated<Task>(entityId);
                scope.Complete();
            }
        }
    }
}
