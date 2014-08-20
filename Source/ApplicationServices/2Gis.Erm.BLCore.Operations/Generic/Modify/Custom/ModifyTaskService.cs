using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
	public sealed class ModifyTaskService : IModifyBusinessModelEntityService<Task>
	{
		private readonly IActivityReadModel _readModel;
		private readonly IBusinessModelEntityObtainer<Task> _activityObtainer;
		private readonly ICreateAggregateRepository<Task> _createOperationService;
		private readonly IUpdateTaskAggregateService _updateOperationService;
		private readonly IUpdateRegardingObjectAggregateService<Task> _updateRegardingObjectService;

		public ModifyTaskService(
			IActivityReadModel readModel,
			IBusinessModelEntityObtainer<Task> obtainer,
			ICreateAggregateRepository<Task> createOperationService,
			IUpdateTaskAggregateService updateOperationService,
			IUpdateRegardingObjectAggregateService<Task> updateRegardingObjectService)
		{
			_readModel = readModel;
			_activityObtainer = obtainer;
			_createOperationService = createOperationService;
			_updateOperationService = updateOperationService;
			_updateRegardingObjectService = updateRegardingObjectService;
		}

		public long Modify(IDomainEntityDto domainEntityDto)
		{
			var taskDto = (TaskDomainEntityDto)domainEntityDto;
			var task = _activityObtainer.ObtainBusinessModelEntity(domainEntityDto);

			IEnumerable<RegardingObject<Task>> oldRegardingObjects;
			if (task.IsNew())
			{
				_createOperationService.Create(task);
				oldRegardingObjects = null;
			}
			else
			{
				_updateOperationService.Update(task);
				oldRegardingObjects = _readModel.GetRegardingObjects<Task>(task.Id);
			}

			_updateRegardingObjectService.ChangeRegardingObjects(oldRegardingObjects, new[]
				{
					task.ReferenceIfAny(EntityName.Client, taskDto.ClientRef.Id),
					task.ReferenceIfAny(EntityName.Contact, taskDto.ContactRef.Id),
					task.ReferenceIfAny(EntityName.Deal, taskDto.DealRef.Id),
					task.ReferenceIfAny(EntityName.Firm, taskDto.FirmRef.Id)
				}.Where(x => x != null));

			return task.Id;
		}
	}
}
