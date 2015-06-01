using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Reopen;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.Operations.Reopen
{
    public class ReopenTaskAggregateService : IReopenTaskAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Task> _repository;

        public ReopenTaskAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<Task> repository)
        {
            _operationScopeFactory = operationScopeFactory;
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
                task.Status = ActivityStatus.InProgress;

                _repository.Update(task);
                _repository.Save();

                operationScope.Updated<Task>(task.Id);
                operationScope.Complete();
            }
        }
    }
}
