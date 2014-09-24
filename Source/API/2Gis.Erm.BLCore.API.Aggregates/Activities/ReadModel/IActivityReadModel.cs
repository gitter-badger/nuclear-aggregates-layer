using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel
{
    public interface IActivityReadModel : ISimplifiedModelConsumerReadModel
    {
		Appointment GetAppointment(long appointmentId);
			
		Phonecall GetPhonecall(long phonecallId);

		Task GetTask(long taskId);

		IEnumerable<RegardingObject<TEntity>> GetRegardingObjects<TEntity>(long entityId) where TEntity : class, IEntity;

		bool CheckIfActivityExistsRegarding(long clientId);
        bool CheckIfOpenActivityExistsRegarding(EntityName entityName, long entityId);
    }
}