using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{

    public sealed class AssignTaskAggregateService : IAssignTaskAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;

        private readonly IActionLogger _actionLogger;

        private readonly IRepository<Task> _repository;

        public AssignTaskAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IActionLogger actionLogger,
            IRepository<Task> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _actionLogger = actionLogger;
            _repository = repository;
        }

        public void Assign(Task task, long ownerCode)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            if (task.Status != ActivityStatus.InProgress)
            {
                throw new BusinessLogicException(BLResources.CannotAssignActivityNotInProgress);
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignIdentity, Task>())
            {
                var originalOwnerCode = task.OwnerCode;
                task.OwnerCode = ownerCode;

                _repository.Update(task);
                _repository.Save();

                _actionLogger.LogChanges(task, x => x.OwnerCode, originalOwnerCode, ownerCode);

                operationScope.Updated<Task>(task.Id);
                operationScope.Complete();
            }
        }
    }
}