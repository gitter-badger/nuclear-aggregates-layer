using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Cancel;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Cancel;
using DoubleGis.Erm.BLCore.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Cancel
{
    public class CancelTaskOperationService : ICancelGenericOperationService<Task>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ITaskReadModel _taskReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;

        private readonly ICancelTaskAggregateService _cancelTaskAggregateService;

        public CancelTaskOperationService(
            IOperationScopeFactory operationScopeFactory,
            ITaskReadModel taskReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext,
            ICancelTaskAggregateService cancelTaskAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _taskReadModel = taskReadModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _cancelTaskAggregateService = cancelTaskAggregateService;
        }

        public void Cancel(long entityId)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<CancelIdentity, Task>())
            {
                var task = _taskReadModel.GetTask(entityId);                

                if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, task.OwnerCode))
                {
                    throw new SecurityException(string.Format("{0}: {1}", task.Header, BLResources.SecurityAccessDenied));
                }   

                _cancelTaskAggregateService.Cancel(task);

                scope.Updated<Task>(entityId);
                scope.Complete();
            }
        }
    }
}
