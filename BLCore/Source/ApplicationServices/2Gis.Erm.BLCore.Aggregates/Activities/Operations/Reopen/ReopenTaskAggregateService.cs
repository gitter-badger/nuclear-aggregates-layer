using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Reopen;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Reopen;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.Operations.Reopen
{
    public class ReopenTaskAggregateService : IReopenTaskAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;

        private readonly IActionLogger _actionLogger;

        private readonly IRepository<Task> _repository;

        public ReopenTaskAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IActionLogger actionLogger,
            IRepository<Task> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _actionLogger = actionLogger;
            _repository = repository;
        }

        public void Reopen(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            if (task.Status == ActivityStatus.InProgress)
            {
                return;
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ReopenIdentity, Task>())
            {
                var originalValue = task.Status;
                task.Status = ActivityStatus.InProgress;

                _repository.Update(task);
                _repository.Save();

                _actionLogger.LogChanges(task, x => x.Status, originalValue, task.Status);

                operationScope.Updated<Task>(task.Id);
                operationScope.Complete();
            }
        }
    }
}
