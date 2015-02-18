using System.Collections.Generic;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Read;
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
        private readonly IActivityReadService _activityReadService;
        private readonly IBusinessModelEntityObtainer<Task> _activityObtainer;
        private readonly ICreateTaskAggregateService _createOperationService;
        private readonly IUpdateTaskAggregateService _updateOperationService;

        public ModifyTaskService(
            ITaskReadModel readModel,
            IActivityReadService activityReadService,
            IBusinessModelEntityObtainer<Task> obtainer,
            ICreateTaskAggregateService createOperationService,
            IUpdateTaskAggregateService updateOperationService)
        {
            _readModel = readModel;
            _activityReadService = activityReadService;
            _activityObtainer = obtainer;
            _createOperationService = createOperationService;
            _updateOperationService = updateOperationService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var taskDto = (TaskDomainEntityDto)domainEntityDto;
            var task = _activityObtainer.ObtainBusinessModelEntity(domainEntityDto);

            _activityReadService.CheckIfAnyEntityReferencesContainsReserve(taskDto.RegardingObjects);            

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
                    _updateOperationService.Update(task);
                    oldRegardingObjects = _readModel.GetRegardingObjects(task.Id);
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
