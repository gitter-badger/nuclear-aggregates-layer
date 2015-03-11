﻿using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Complete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Complete;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.Operations.Complete
{
    public class CompleteTaskAggregateService : ICompleteTaskAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;

        private readonly IActionLogger _actionLogger;

        private readonly IRepository<Task> _repository;

        public CompleteTaskAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IActionLogger actionLogger,
            IRepository<Task> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _actionLogger = actionLogger;
            _repository = repository;
        }

        public void Complete(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            if (task.Status != ActivityStatus.InProgress)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCompleteFinishedOrClosedActivity, task.Header));
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CompleteIdentity, Task>())
            {
                var originalValue = task.Status;
                task.Status = ActivityStatus.Completed;

                _repository.Update(task);
                _repository.Save();

                _actionLogger.LogChanges(task, x => x.Status, originalValue, task.Status);

                operationScope.Updated<Task>(task.Id);
                operationScope.Complete();
            }
        }
    }
}
