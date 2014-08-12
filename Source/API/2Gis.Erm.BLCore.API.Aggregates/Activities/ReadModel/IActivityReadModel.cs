using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel
{
    public interface IActivityReadModel : ISimplifiedModelConsumerReadModel
    {
        Task GetTask(long taskId);
        Phonecall GetPhonecall(long phonecallId);
        Appointment GetAppointment(long appointmentId);

		bool CheckIfRelatedActivitiesExists(long clientId);
    }
}