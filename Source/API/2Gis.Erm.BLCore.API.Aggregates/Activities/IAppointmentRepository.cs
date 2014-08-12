using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
	public interface IAppointmentRepository : IAggregateRootRepository<Appointment>, IDeleteAggregateRepository<Appointment>
	{
		long Add(Appointment appointment);
		int Delete(Appointment appointment);
		void UpdateContent(Appointment appointment);
		void UpdateRegardingObjects(Appointment appointment);
	}
}