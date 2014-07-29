using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
	public class ModifyAppointmentService : ModifyActivityService<Appointment>
	{
		public ModifyAppointmentService(IBusinessModelEntityObtainer<Appointment> obtainer,
		                                IActivityAggregateService<Appointment> activityAggregateService)
			: base(obtainer, activityAggregateService)
		{
		}
	}
}
