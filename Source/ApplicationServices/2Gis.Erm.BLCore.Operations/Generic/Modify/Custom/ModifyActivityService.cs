using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
	public abstract class ModifyActivityService<TActivity> : IModifyBusinessModelEntityService<TActivity> where TActivity : ActivityBase
	{
		private readonly IActivityAggregateService<TActivity> _activityAggregateService;
		private readonly IBusinessModelEntityObtainer<TActivity> _activityObtainer;

		protected ModifyActivityService(IBusinessModelEntityObtainer<TActivity> activityObtainer,
		                                IActivityAggregateService<TActivity> activityAggregateService)
		{
			_activityObtainer = activityObtainer;
			_activityAggregateService = activityAggregateService;
		}

		public long Modify(IDomainEntityDto domainEntityDto)
		{
			var activity = _activityObtainer.ObtainBusinessModelEntity(domainEntityDto);

			if (activity.IsNew())
			{
				_activityAggregateService.Create(activity);
			}
			else
			{
				_activityAggregateService.Update(activity);
			}

			return activity.Id;
		}
	}
}