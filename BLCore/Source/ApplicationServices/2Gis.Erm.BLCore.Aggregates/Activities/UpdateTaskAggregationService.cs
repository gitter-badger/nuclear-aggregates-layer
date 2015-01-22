using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
    public sealed class UpdateTaskAggregationService : IUpdateTaskAggregateService
    {
        private const string ActivityHasNoTheIdentityMessage = "The task has no the identity.";

        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Task> _repository;
        private readonly IRepository<TaskRegardingObject> _referenceRepository;

        public UpdateTaskAggregationService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<Task> repository,
            IRepository<TaskRegardingObject> referenceRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
            _referenceRepository = referenceRepository;
        }

        public void Update(Task task)
        {
            CheckTask(task);

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Task>())
            {
                _repository.Update(task);
                _repository.Save();
                
                operationScope.Updated<Task>(task.Id);
                operationScope.Complete();
            }
        }

        public void ChangeRegardingObjects(Task task, IEnumerable<TaskRegardingObject> oldReferences, IEnumerable<TaskRegardingObject> newReferences)
        {
            CheckTask(task);

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Task>())
            {
                _referenceRepository.Update<Task, TaskRegardingObject>(oldReferences, newReferences);

                operationScope.Updated<Task>(task.Id);
                operationScope.Complete();
            }
        }

        private static void CheckTask(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            if (task.Id == 0)
            {
                throw new ArgumentException(ActivityHasNoTheIdentityMessage, "task");
            }
        }
    }
}