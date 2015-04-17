using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Reopen;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Reopen;
using DoubleGis.Erm.BLCore.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Reopen
{
    public class ReopenTaskOperationService : IReopenGenericOperationService<Task>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ITaskReadModel _taskReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;

        private readonly IActionLogger _actionLogger;

        private readonly IReopenTaskAggregateService _reopenTaskAggregateService;

        public ReopenTaskOperationService(
            IOperationScopeFactory operationScopeFactory,
            ITaskReadModel taskReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext,
            IActionLogger actionLogger,
            IReopenTaskAggregateService reopenTaskAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _taskReadModel = taskReadModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _actionLogger = actionLogger;
            _reopenTaskAggregateService = reopenTaskAggregateService;
        }

        public void Reopen(long entityId)
        {            
            using (var scope = _operationScopeFactory.CreateSpecificFor<ReopenIdentity, Task>())
            {
                var task = _taskReadModel.GetTask(entityId);
                var originalStatus = task.Status;

                if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, task.OwnerCode))
                {
                    throw new SecurityException(string.Format("{0}: {1}", task.Header, BLResources.SecurityAccessDenied));
                }   

                _reopenTaskAggregateService.Reopen(task);

                _actionLogger.LogChanges(task, x => x.Status, originalStatus, ActivityStatus.InProgress);

                scope.Updated<Task>(entityId);
                scope.Complete();
            }
        }
    }
}
