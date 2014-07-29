using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel
{
    public interface IActivityReadModel : IAggregateReadModel<ActivityBase>
    {
        Task GetTask(long taskId);
        Phonecall GetPhonecall(long phonecallId);
        Appointment GetAppointment(long appointmentId);
        
		bool CheckIfRelatedActivitiesExists(long clientId);
    }
}