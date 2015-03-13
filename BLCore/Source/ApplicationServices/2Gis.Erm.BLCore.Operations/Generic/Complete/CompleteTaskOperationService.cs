using System;
using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Complete;
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
    public class CompleteTaskOperationService : ICompleteGenericOperationService<Task>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ITaskReadModel _taskReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;

        private readonly ICompleteTaskAggregateService _completeTaskAggregateService;

        public CompleteTaskOperationService(
            IOperationScopeFactory operationScopeFactory,
            ITaskReadModel taskReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext,
            ICompleteTaskAggregateService completeTaskAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _taskReadModel = taskReadModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _completeTaskAggregateService = completeTaskAggregateService;
        }

        public void Complete(long entityId)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<CompleteIdentity, Task>())
            {
                var task = _taskReadModel.GetTask(entityId);

                if (task.ScheduledOn.Date > DateTime.Now.Date)
                {
                    throw new BusinessLogicException(BLResources.ActivityClosingInFuturePeriodDenied);
                }

                if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, task.OwnerCode))
                {
                    throw new SecurityException(string.Format("{0}: {1}", task.Header, BLResources.SecurityAccessDenied));
                }   

                _completeTaskAggregateService.Complete(task);

                scope.Updated<Task>(entityId);
                scope.Complete();
            }
        }
    }
}
