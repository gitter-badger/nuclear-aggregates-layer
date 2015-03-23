using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
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
        private readonly IClientReadModel _clientReadModel;
        private readonly IFirmReadModel _firmReadModel;
        private readonly ICreateTaskAggregateService _createOperationService;
        private readonly IUpdateTaskAggregateService _updateOperationService;

        public ModifyTaskService(
            ITaskReadModel readModel,
            IBusinessModelEntityObtainer<Task> obtainer,
            IClientReadModel clientReadModel,
            IFirmReadModel firmReadModel,
            ICreateTaskAggregateService createOperationService,
            IUpdateTaskAggregateService updateOperationService)
        {
            _readModel = readModel;            
            _activityObtainer = obtainer;
            _clientReadModel = clientReadModel;
            _firmReadModel = firmReadModel;
            _createOperationService = createOperationService;
            _updateOperationService = updateOperationService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var taskDto = (TaskDomainEntityDto)domainEntityDto;
            if (taskDto.RegardingObjects == null || !taskDto.RegardingObjects.Any())
            {
                throw new BusinessLogicException(BLResources.NoRegardingObjectValidationError);
            }

            var task = _activityObtainer.ObtainBusinessModelEntity(domainEntityDto);

            if (taskDto.RegardingObjects.HasReferenceInReserve(EntityName.Client, _clientReadModel.IsClientInReserve))
            {
                throw new BusinessLogicException(BLResources.CannotSaveActivityForClientInReserve);
            }

            if (taskDto.RegardingObjects.HasReferenceInReserve(EntityName.Firm, _firmReadModel.IsFirmInReserve))
            {
                throw new BusinessLogicException(BLResources.CannotSaveActivityForFirmInReserve);
            }
            
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
