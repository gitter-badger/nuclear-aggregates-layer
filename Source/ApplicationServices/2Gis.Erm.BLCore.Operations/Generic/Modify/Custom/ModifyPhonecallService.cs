using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
	public class ModifyPhonecallService : ModifyActivityService<Phonecall>
	{
		public ModifyPhonecallService(IBusinessModelEntityObtainer<Phonecall> obtainer,
		                              IActivityAggregateService<Phonecall> activityAggregateService)
			: base(obtainer, activityAggregateService)
		{
		}
	}
}
