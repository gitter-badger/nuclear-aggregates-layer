using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Cancel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Cancel;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.Operations.Cancel
{
    public class CancelTaskAggregateService : ICancelTaskAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<Task> _repository;

        public CancelTaskAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<Task> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
        }

        public void Cancel(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CancelIdentity, Task>())
            {
                if (task.Status != ActivityStatus.InProgress)
                {
                    throw new BusinessLogicException(string.Format(BLResources.CannotCancelFinishedOrClosedActivity, task.Header));
                }

                task.Status = ActivityStatus.Canceled;

                _repository.Update(task);
                _repository.Save();

                operationScope.Updated<Task>(task.Id);
                operationScope.Complete();
            }
        }
    }
}
