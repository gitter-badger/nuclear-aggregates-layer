using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
	public class ModifyPhonecallService : IModifyBusinessModelEntityService<Phonecall>
	{
		private readonly IBusinessModelEntityObtainer<Phonecall> _activityObtainer;
		private readonly IPhonecallRepository _repository;

		public ModifyPhonecallService(IBusinessModelEntityObtainer<Phonecall> obtainer, IPhonecallRepository repository)
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
