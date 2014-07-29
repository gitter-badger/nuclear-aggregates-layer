using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
	public class ModifyTaskService : ModifyActivityService<Task>
	{
        public ModifyTaskService(IBusinessModelEntityObtainer<Task> obtainer,
                                 IActivityAggregateService<Task> activityAggregateService) 
			: base(obtainer, activityAggregateService)
        {
        }
    }
}
