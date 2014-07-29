using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel
{
    public interface IActivityReadModel : IAggregateReadModel<ActivityBase>
    {
        [Obsolete("Use typed GetTask, GetPhonecall or GetAppointment. Will be deleted soon")]
        TActivity GetActivity<TActivity>(long activityId) where TActivity : ActivityBase, new();

        Task GetTask(long taskId);
        Phonecall GetPhonecall(long phonecallId);
        Appointment GetAppointment(long appointmentId);
        ActivityInstanceDto GetActivityInstanceDto(Task task);
        ActivityInstanceDto GetActivityInstanceDto(Phonecall phonecall);
        ActivityInstanceDto GetActivityInstanceDto(Appointment appointment);
        bool CheckIfRelatedActivitiesExists(long clientId);
        bool TryGetRelatedActivities(long clientId, out IEnumerable<ActivityInstance> activities);
    }
}