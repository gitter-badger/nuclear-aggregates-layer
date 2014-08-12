using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
	public class ModifyTaskService : IModifyBusinessModelEntityService<Task> 
	{
		private readonly IBusinessModelEntityObtainer<Task> _activityObtainer;
		private readonly ITaskRepository _repository;

		public ModifyTaskService(IBusinessModelEntityObtainer<Task> obtainer, ITaskRepository repository)
		{
			_activityObtainer = obtainer;
			_repository = repository;
		}

		public long Modify(IDomainEntityDto domainEntityDto)
		{
			var activity = _activityObtainer.ObtainBusinessModelEntity(domainEntityDto);

			if (activity.IsNew())
			{
				_repository.Add(activity);
			}
			else
			{
				_repository.UpdateContent(activity);
				_repository.UpdateRegardingObjects(activity);
			}

			return activity.Id;
		}
    }
}
