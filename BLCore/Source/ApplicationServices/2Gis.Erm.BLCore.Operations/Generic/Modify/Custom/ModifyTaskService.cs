using System.Collections.Generic;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
    public sealed class ModifyTaskService : IModifyBusinessModelEntityService<Task>
    {
        private readonly ITaskReadModel _readModel;
        private readonly IBusinessModelEntityObtainer<Task> _activityObtainer;
        private readonly IActionLogger _actionLogger;
        private readonly ICreateTaskAggregateService _createOperationService;
        private readonly IUpdateTaskAggregateService _updateOperationService;

        public ModifyTaskService(
            ITaskReadModel readModel,
            IBusinessModelEntityObtainer<Task> obtainer,
            IActionLogger actionLogger,
            ICreateTaskAggregateService createOperationService,
            IUpdateTaskAggregateService updateOperationService)
        {
            _readModel = readModel;
            _activityObtainer = obtainer;
            _actionLogger = actionLogger;
            _createOperationService = createOperationService;
            _updateOperationService = updateOperationService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var taskDto = (TaskDomainEntityDto)domainEntityDto;
            var task = _activityObtainer.ObtainBusinessModelEntity(domainEntityDto);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                IEnumerable<TaskRegardingObject> oldRegardingObjects;
                if (task.IsNew())
                {
                    _createOperationService.Create(task);
                    oldRegardingObjects = null;
                }
                else
                {
                    var originalTask = _readModel.GetTask(task.Id);
                    _updateOperationService.Update(task);
                    oldRegardingObjects = _readModel.GetRegardingObjects(task.Id);
                    if (originalTask.ScheduledOn != task.ScheduledOn)
                    {
                        _actionLogger.LogChanges(task, x => x.ScheduledOn, originalTask.ScheduledOn, task.ScheduledOn);
                    }
                }

                _updateOperationService.ChangeRegardingObjects(task,
                                                               oldRegardingObjects,
                                                               task.ReferencesIfAny<Task, TaskRegardingObject>(taskDto.RegardingObjects));

                transaction.Complete();
                
                return task.Id;
            }
        }
    }
}
